using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Core.ImageProcessing;
using OcrConfiguration = ScanbotSDK.MAUI.Core.Ocr.OcrConfiguration;
using ScanbotSDK.MAUI.Core.PdfGeneration;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Results;
using ScanbotSdkExample.Maui.ReadyToUseUI;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui;

public partial class HomePage
{
    private const string ViewLicenseInfo = "View License Info";
    private const string LicenseInvalidMessage = "The license is invalid or expired.";

    /// <summary>
    /// Binding property configured with the Scanbot activity loader.
    /// </summary>
    public bool IsLoading
    {
        get => SbLoader.IsBusy;
        set => SbLoader.IsBusy = value;
    }

    public List<SdkFeature> SdkFeatures { get; set; }

    public HomePage()
    {
        InitializeComponent();
        SdkFeatures =
        [
            new SdkFeature("DOCUMENT SCANNER"),
            new SdkFeature("Single Document Scanning", DocumentScannerFeature.SingleDocumentScanningClicked),
            new SdkFeature("Single Finder Document Scanning", DocumentScannerFeature.SingleFinderDocumentScanningClicked),
            new SdkFeature("Multiple Document Scanning", DocumentScannerFeature.MultipleDocumentScanningClicked),
            new SdkFeature("Scan Document From Image", ScanDocumentFromImageClicked),
            new SdkFeature("Scan Document From PDF", ScanDocumentFromPdfClicked),
            new SdkFeature("Delete all documents", DeleteAllDocsFromStorageClicked),

            new SdkFeature("CLASSIC COMPONENT"),
            new SdkFeature("Classic Document Scanner", DocumentScannerFeature.ClassicDocumentScannerViewClicked),
            new SdkFeature("Classic Document Scanner (MVVM)", DocumentScannerFeature.ClassicDocumentScannerMVVMViewClicked),

            new SdkFeature("DATA DETECTORS"),
            new SdkFeature("Check Scanner", DataDetectorsFeature.CheckScannerClicked),
            new SdkFeature("Credit Card Scanner", DataDetectorsFeature.CreditCardScannerClicked),
            new SdkFeature("Document Data Scanner", DataDetectorsFeature.DocumentDataScannerClicked),
            new SdkFeature("Mrz Scanner", DataDetectorsFeature.MrzScannerClicked),
            new SdkFeature("Text Pattern Scanner", DataDetectorsFeature.TextPatternScannerClicked),
            new SdkFeature("Vin Scanner", DataDetectorsFeature.VinScannerClicked),

            new SdkFeature("SCAN FROM IMAGE"),
            new SdkFeature("Check Recognizer", DetectOnImageFeature.CheckDetectorClicked),
            new SdkFeature("Credit Card Recognizer", DetectOnImageFeature.CreditCardDetectorClicked),
            new SdkFeature("MRZ Recognizer", DetectOnImageFeature.MrzDetectorClicked),
            new SdkFeature("Document Data Extractor", DetectOnImageFeature.DocumentDataExtractorClicked),

            new SdkFeature("SDK OPERATIONS"),
            new SdkFeature("PDF from Image", CreatePdfFromImageClicked),
            new SdkFeature("Extract Images from PDF", ExtractImagesFromPdfClicked),
            new SdkFeature("OCR from Image", ExtractOcrFromImageClicked),
            
            new SdkFeature("MISCELLANEOUS"),
            new SdkFeature(ViewLicenseInfo, ViewLicenseInfoClicked), 
            new SdkFeature("Learn more about Scanbot SDK", LearnMoreClicked)
        ];
        
        BindingContext = this;
    }

    /// Item Selected method invoked on the ListView item selection.
    private async void SdkFeatureSelected(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not SdkFeature feature || feature.Action == null)
            return;

        if (!App.IsLicenseValid && feature.Title != ViewLicenseInfo)
        {
            await Alert.ShowAsync("Oops!", LicenseInvalidMessage);
            return;
        }

        await feature.Action();
    }

    private async Task ScanDocumentFromImageClicked()
    {
        try
        {
            IsLoading = true;

            // @Tag("Detect Document from Image")
            var image = await ImagePicker.PickImageAsSourceAsync();
            if (image is null) return;

            // Creates a document from the given image as original image and create a Page object
            var result = await ScanbotSDKMain.Document.CreateDocumentFromImagesAsync([image], new CreateDocumentOptions
            {
                // runs document detection on the given image.
                DocumentDetection = true
            });

            if (!result.IsSuccess)
            {
                await Alert.ShowAsync(result.Error);
                return;
            }

            await Navigation.PushAsync(new ScannedDocumentsPage(result.Value));
            // @EndTag("Detect Document from Image")
        }
        catch (Exception ex)
        {
            await Alert.ShowAsync("Error", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ScanDocumentFromPdfClicked()
    {
        try
        {
            var filePath = await PdfPicker.PickAsync();
            if (filePath is null) return;
         
            IsLoading = true;
            
            var result = await ScanbotSDKMain.Document.CreateDocumentFromPdfAsync(filePath, new CreateDocumentOptions
            {
                DocumentDetection = true,
                Filters = [new ColorDocumentFilter()]
            });

            if (!result.IsSuccess)
            {
                await Alert.ShowAsync(result.Error);
                return;
            }

            await Navigation.PushAsync(new ScannedDocumentsPage(result.Value));
        }
        catch (Exception ex)
        {
            await Alert.ShowAsync("Error", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExtractOcrFromImageClicked()
    {
        try
        {
            var image = await ImagePicker.PickImageAsSourceAsync();
            if (image is null) return;

            IsLoading = true;

            var result =
                await ScanbotSDKMain.OcrEngine.RecognizeOnImagesAsync(images: [image],
                    configuration: OcrConfiguration.ScanbotOcr);
            if (!result.IsSuccess)
            {
                await Alert.ShowAsync(result.Error);
                return;
            }

            await Alert.ShowAsync(title: "Ocr Result", message: result.Value.RecognizedText);
        }
        catch (Exception ex)
        {
            await Alert.ShowAsync("Error", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreatePdfFromImageClicked()
    {
        var image = await ImagePicker.PickImageAsSourceAsync();
        if (image is null) return; 

        var result = await ScanbotSDKMain.PdfGenerator.GenerateFromImagesAsync(images: [image], pdfConfiguration: new PdfConfiguration());
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }
        
        await SharingUtils.ShareFileAsync(result.Value.LocalPath, "application/pdf");
    }
    
    private async Task DeleteAllDocsFromStorageClicked()
    {
        var documentIds = ScanbotSDKMain.Document.StoredDocumentUuids()?.ValueOrNull ?? [];
        if (documentIds.Length == 0)
        {
            await Alert.ShowAsync("Alert!", "There are no more documents available to delete.");
            return;
        }

        var message = "This will delete all the documents found on the local storage.";
        var alertAccepted = await Alert.ShowAsync("Attention!", message, "Confirm", "Cancel");
        if (!alertAccepted) return;

        var result = await ScanbotSDKMain.Document.DeleteAllDocumentsAsync();
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        await Alert.ShowAsync("Alert", $"Number of documents deleted: {documentIds.Length}");
    }
    
    private async Task ExtractImagesFromPdfClicked()
    {
        try
        {
            var filePath = await PdfPicker.PickAsync();
            if (filePath is null) return;

            IsLoading = true;

            var result = await ScanbotSDKMain.PdfImageExtractor.ExtractImageFilesAsync(
                pdfFileUri: new Uri(filePath));

            if (!result.IsSuccess)
            {
                await Alert.ShowAsync(result.Error);
                return;
            }

            await Navigation.PushAsync(new PdfExtractedImageResultPage(result.Value));
        }
        catch (Exception ex)
        {
            await Alert.ShowAsync("Error", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ViewLicenseInfoClicked()
    {
        var result = ScanbotSDKMain.LicenseInfo();
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        var info = result.Value;
        var message = info.IsValid
            ? $"License status: {info.Status}\nIt is valid until {info.ExpirationDateString}."
            : LicenseInvalidMessage;

        await Alert.ShowAsync("License info", message);
    }

    private async Task LearnMoreClicked()
    {
        await Browser.OpenAsync(new Uri("https://scanbot.io/developer/net-maui-barcode-scanner-sdk/"), BrowserLaunchMode.SystemPreferred);
    }
}