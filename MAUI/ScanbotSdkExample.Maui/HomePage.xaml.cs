using Microsoft.Maui.Graphics.Platform;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.PdfGeneration;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Ocr;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Results;
using ScanbotSdkExample.Maui.ReadyToUseUI;
using ScanbotSdkExample.Maui.Utils;
using Image = Microsoft.Maui.Controls.PlatformConfiguration.TizenSpecific.Image;
using ImageSource = Microsoft.Maui.Controls.ImageSource;

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
        SdkFeatures =
        [
            new SdkFeature("DOCUMENT SCANNER"),
            new SdkFeature("Single Document Scanning", DocumentScannerFeature.SingleDocumentScanningClicked),
            new SdkFeature("Single Finder Document Scanning",
                DocumentScannerFeature.SingleFinderDocumentScanningClicked),
            new SdkFeature("Multiple Document Scanning", DocumentScannerFeature.MultipleDocumentScanningClicked),
            new SdkFeature("Import Image", ImportButtonClicked),
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

            new SdkFeature("DETECTION FROM IMAGE"),
            new SdkFeature("Check Recognizer", DetectOnImageFeature.CheckDetectorClicked),
            new SdkFeature("Credit Card Recognizer", DetectOnImageFeature.CreditCardDetectorClicked),
            new SdkFeature("MRZ Recognizer", DetectOnImageFeature.MrzDetectorClicked),
            new SdkFeature("Document Data Extractor", DetectOnImageFeature.DocumentDataExtractorClicked),
            new SdkFeature("Medical Certificate Recognizer", DetectOnImageFeature.MedicalCertificateDetectorClicked),
            
            new SdkFeature("SDK OPERATIONS"),
            new SdkFeature("Mock Camera", ConfigureMockCameraClicked),
            new SdkFeature("PDF from Image", CreatePdfFromImageClicked),
            new SdkFeature("OCR from Image", ExtractOcrFromImageClicked),
            
            new SdkFeature("MISCELLANEOUS"),
            new SdkFeature(ViewLicenseInfo, ViewLicenseInfoClicked),
            new SdkFeature("Learn more about Scanbot SDK", LearnMoreClicked)
        ];

        BindingContext = this;
        InitializeComponent();
    }

    /// Item Selected method invoked on the ListView item selection.
    async void SdkFeatureSelected(Object sender, SelectionChangedEventArgs e)
    {
        FeaturesCollectionView.SelectedItem = null;
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        if (e.CurrentSelection.FirstOrDefault() is not SdkFeature feature || feature.DoTask == null)
        {
            return;
        }

        if (!ScanbotSdkMain.IsLicenseValid && feature.Title != ViewLicenseInfo)
        {
            Alert.Show("Oops!", LicenseInvalidMessage);
            return;
        }

        await feature.DoTask();
    } 

    // ------------------------------------
    // View License Info
    // ------------------------------------
    private Task ViewLicenseInfoClicked()
    {
        var info = ScanbotSdkMain.LicenseInfo;
        var message = $"License status: {info.Status}\n";
        if (info.IsValid)
        {
            message += $"It is valid until {info.ExpirationDateString}.";
        }
        else
        {
            message = LicenseInvalidMessage;
        }

        Alert.Show("License info", message);
        return Task.CompletedTask;
    }

    // ------------------------------------
    // Learn More
    // ------------------------------------
    private async Task LearnMoreClicked()
    {
        await Browser.OpenAsync(new Uri("https://scanbot.io/developer/net-maui-barcode-scanner-sdk/"),
            BrowserLaunchMode.SystemPreferred);
    }

    private async Task ImportButtonClicked()
    {
        try
        {
            IsLoading = true;

            var imageSource = await PickImageAsync();
            if (imageSource is null) return;

            // Creates a document from the given image as original image and create a Page object
            var document = await ScanbotSdkMain.DocumentScanner.CreateDocumentFromImagesAsync([imageSource], new CreateDocumentConfiguration
            {
                // runs document detection on the given image.
                DocumentDetection = true
            });
            
            await Navigation.PushAsync(new ScannedDocumentsPage(document));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <summary>
    /// Picks image from the photos application.
    /// </summary>
    /// <returns></returns>
    public static async Task<FileImageSource> PickImageAsync()
    {
        try
        {
            // Pick the photo
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
              return ImageSource.FromFile(photo.FullPath) as FileImageSource;
            }
        }
        catch (Exception ex)
        {
            Alert.Show("Error", $"Unable to pick image: {ex.Message}");
        }

        return null;
    }
    
    private async Task ExtractOcrFromImageClicked()
    {
        var image = await PickImageAsync();
        if (image == null) return;


        var result = await ScanbotSdkMain.ImageProcessor.PerformOcrAsync(sourceImages:[image], sourceImagesEncrypted: false, ocrEngine: OcrEngine.ScanbotOcr);
        Alert.Show(title: "Ocr Result", message: result.RecognizedText);
    }
    
    private async Task CreatePdfFromImageClicked()
    {
        var image = await PickImageAsync();
        if (image == null) return;


        var result = await ScanbotSdkMain.ImageProcessor.CreatePdfAsync(sourceImages:[image], sourceImagesEncrypted: false, configuration: new PdfConfiguration());
        if (result == null || !result.IsFile)
            return;
        
        // Sharing the Pdf.
        await SharingUtils.ShareFileAsync(result.LocalPath, "application/pdf");
    }

    private async Task ConfigureMockCameraClicked()
    {
        FileImageSource image = await PickImageAsync();
        if (image?.File == null)
        {
            Alert.Show("Error","Something went wrong while loading the image from photos app.");
            return;
        }
        ScanbotSdkMain.MockCamera.ConfigureMockCamera(new MockCameraConfiguration(image.File, image.File, "Scanbot SDK Mock Cam"));
    }

    private async Task DeleteAllDocsFromStorageClicked()
    {
        var documentCount = ScanbotSdkMain.DocumentScanner.StoredDocumentUuids.Length;
        if (documentCount == 0)
            return;
        
        var message = "This will delete all the documents found on the local storage.";
        var result = await DisplayAlert("Attention!", message, "Confirm", "Cancel");
        if (!result) return;
        
        await ScanbotSdkMain.DocumentScanner.DeleteAllDocumentsAsync();
        await DisplayAlert("Alert", $"Number of documents deleted: {documentCount}", "Ok");
    }
}