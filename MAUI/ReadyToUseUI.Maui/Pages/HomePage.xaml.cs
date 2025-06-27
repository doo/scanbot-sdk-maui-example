using ScanbotSDK.MAUI;
using ReadyToUseUI.Maui.Utils;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage : ContentPage
{
    /// <summary>
    /// Binding property configured with the Scanbot activity loader.
    /// </summary>
    public bool IsLoading
    {
        get => sbLoader.IsBusy;
        set => sbLoader.IsBusy = value;
    }

    public struct SdkFeature
    {
        public SdkFeature(string title, Func<Task> doTask = null)
        {
            Title = title;
            DoTask = doTask;
        }

        public string Title { get; private set; }
        public Func<Task> DoTask { get; private set; }

        public bool ShowHeading => DoTask == null;
        public bool ShowFeature => DoTask != null;
    }

    private List<SdkFeature> _sdkFeatures = [];
    public List<SdkFeature> SdkFeatures
    {
        get => _sdkFeatures;
        set => _sdkFeatures = value;
    }

    public HomePage()
    {
        SdkFeatures =
        [
            new SdkFeature("DOCUMENT SCANNER"),
            new SdkFeature("Single Document Scanning", SingleDocumentScanningClicked),
            new SdkFeature("Single Finder Document Scanning", SingleFinderDocumentScanningClicked),
            new SdkFeature("Multiple Document Scanning", MultipleDocumentScanningClicked),
            new SdkFeature("Import Import Image", ImportButtonClicked),
            
            new SdkFeature("DOCUMENT SCANNER V1"),
            new SdkFeature("Scan V1 Document", ScanV1DocumentClicked),

            new SdkFeature("DATA DETECTORS"),
            
            new SdkFeature("Check Scanner", CheckScannerClicked),
            new SdkFeature("Credit Card Scanner", CreditCardScannerClicked),
            new SdkFeature("European Health Insurance Scanner", EhicScannerClicked),
            new SdkFeature("Document Data Scanner", DocumentDataScannerClicked),
            new SdkFeature("Medical Certificate Scanner", MedicalCertificateRecognizerClicked),
            new SdkFeature("Mrz Scanner", MrzScannerClicked),
            new SdkFeature("Text Pattern Scanner", TextPatternScannerClicked),
            new SdkFeature("Vin Scanner", VinScannerClicked),

            new SdkFeature("DETECTION FROM IMAGE"),
            new SdkFeature("Check Recognizer", CheckDetectorClicked),
            new SdkFeature("Credit Card Recognizer", CreditCardDetectorClicked),
            new SdkFeature("MRZ Recognizer", MrzDetectorClicked),
            new SdkFeature("EHIC Recognizer", EhicDetectorClicked),
            new SdkFeature("Document Data Extractor", DocumentDataExtractorClicked),
            new SdkFeature("Medical Certificate Recognizer", MedicalCertificateDetectorClicked),

            new SdkFeature("MISCELLANEOUS"),
            new SdkFeature("View License Info", ViewLicenseInfoClicked),
            new SdkFeature("Learn more about Scanbot SDK", LearnMoreClicked)
        ];

        this.BindingContext = this;
        InitializeComponent();
    }

    private async Task ScanV1DocumentClicked()
    {
        var scanner = await ScanbotSDKMain.Rtu.Legacy.DocumentScanner.LaunchDocumentScannerAsync(new ScanbotSDK.MAUI.Document.RTU.v1.DocumentScannerConfiguration());
        await Navigation.PushModalAsync(new DocumentPreviewPage
        {
            DocPreviewSource = scanner.Pages.First().DocumentPreview
        });
    }

    /// Item Selected method invoked on the ListView item selection.
    async void SdkFeatureSelected(Object sender, SelectionChangedEventArgs e)
    {
        if (e?.CurrentSelection?.FirstOrDefault() is SdkFeature feature && feature.DoTask != null)
        {
            if (!SdkUtils.CheckLicense(this)) { return; }

            try
            {
                await feature.DoTask();
            }
            catch
            {
                // ignored
            }
        }
        FeaturesCollectionView.SelectedItem = null;
    }

    // ------------------------------------
    // View License Info
    // ------------------------------------
    private Task ViewLicenseInfoClicked()
    {
        var message = SBSDK.IsLicenseValid ? "Scanbot SDK License is valid" : "Scanbot SDK License is expired";
        ViewUtils.Alert(this, "License info", message);
        return Task.CompletedTask;
    }

    // ------------------------------------
    // Learn More
    // ------------------------------------
    private async Task LearnMoreClicked()
    {
        await Browser.OpenAsync(new Uri("https://scanbot.io/developer/net-maui-barcode-scanner-sdk/"), BrowserLaunchMode.SystemPreferred);
    }
}