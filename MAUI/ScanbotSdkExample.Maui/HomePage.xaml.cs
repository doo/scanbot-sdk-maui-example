using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
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
        SdkFeatures =
        [
            new SdkFeature("DOCUMENT SCANNER"),
            new SdkFeature("Single Document Scanning", DocumentScannerFeature.SingleDocumentScanningClicked),
            new SdkFeature("Single Finder Document Scanning",
                DocumentScannerFeature.SingleFinderDocumentScanningClicked),
            new SdkFeature("Multiple Document Scanning", DocumentScannerFeature.MultipleDocumentScanningClicked),
            new SdkFeature("Import Image", ImportButtonClicked),

            new SdkFeature("CLASSIC COMPONENT"),
            new SdkFeature("Classic Document Scanner", DocumentScannerFeature.ClassicDocumentScannerViewClicked),

            new SdkFeature("DATA DETECTORS"),

            new SdkFeature("Check Scanner", DataDetectorsFeature.CheckScannerClicked),
            new SdkFeature("Credit Card Scanner", DataDetectorsFeature.CreditCardScannerClicked),
            new SdkFeature("European Health Insurance Scanner", DataDetectorsFeature.EhicScannerClicked),
            new SdkFeature("Document Data Scanner", DataDetectorsFeature.DocumentDataScannerClicked),
            new SdkFeature("Medical Certificate Scanner", DataDetectorsFeature.MedicalCertificateScannerClicked),
            new SdkFeature("Mrz Scanner", DataDetectorsFeature.MrzScannerClicked),
            new SdkFeature("Text Pattern Scanner", DataDetectorsFeature.TextPatternScannerClicked),
            new SdkFeature("Vin Scanner", DataDetectorsFeature.VinScannerClicked),

            new SdkFeature("DETECTION FROM IMAGE"),
            new SdkFeature("Check Recognizer", DetectOnImageFeature.CheckDetectorClicked),
            new SdkFeature("Credit Card Recognizer", DetectOnImageFeature.CreditCardDetectorClicked),
            new SdkFeature("MRZ Recognizer", DetectOnImageFeature.MrzDetectorClicked),
            new SdkFeature("EHIC Recognizer", DetectOnImageFeature.EhicDetectorClicked),
            new SdkFeature("Document Data Extractor", DetectOnImageFeature.DocumentDataExtractorClicked),
            new SdkFeature("Medical Certificate Recognizer", DetectOnImageFeature.MedicalCertificateDetectorClicked),

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

        if (!ScanbotSDKMain.IsLicenseValid && feature.Title != ViewLicenseInfo)
        {
            ViewUtils.Alert("Oops!", LicenseInvalidMessage);
            return;
        }

        await feature.DoTask();
    }

    // ------------------------------------
    // View License Info
    // ------------------------------------
    private Task ViewLicenseInfoClicked()
    {
        var info = ScanbotSDKMain.LicenseInfo;
        var message = $"License status: {info.Status}\n";
        if (info.IsValid)
        {
            message += $"It is valid until {info.ExpirationDate?.ToLocalTime()}.";
        }
        else
        {
            message = LicenseInvalidMessage;
        }

        ViewUtils.Alert("License info", message);
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

            var image = await ScanbotSDKMain.ImagePicker.PickImageAsync();
            if (image is null) return;

            var document = new ScannedDocument();

            // Import the selected image as original image and create a Page object
            var page = document.AddPage(image);

            // Run document detection on it
            var result = await ScanbotSDKMain.Rtu.CroppingScreen.LaunchAsync(
                new CroppingConfiguration()
                {
                    DocumentUuid = document.Uuid.ToString(),
                    PageUuid = page.Uuid.ToString()
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
}