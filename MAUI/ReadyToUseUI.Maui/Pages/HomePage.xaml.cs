using ScanbotSDK.MAUI;
using ReadyToUseUI.Maui.Utils;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage : ContentPage
{
    private bool _isLoading;
    /// <summary>
    /// Binding property configured with the Scanbot activity loader.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            this.OnPropertyChanged(nameof(IsLoading));
        }
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

        public bool ShowHeading { get => DoTask == null; }
        public bool ShowFeature { get => DoTask != null; }
    }

    private List<SdkFeature> sdkFeatures;

    public List<SdkFeature> SDKFeatures { get => sdkFeatures; }

    public HomePage()
    {
        sdkFeatures = new List<SdkFeature>
        {
#if LEGACY_EXAMPLES
            new SdkFeature("BARCODE DETECTOR V1"),
            new SdkFeature("Scan QR & Barcodes", () => BarcodeScannerClicked(withImage: false)),
            new SdkFeature("Scan QR & Barcodes With Image", () => BarcodeScannerClicked(withImage: true)),
            new SdkFeature("Scan Multiple QR & Barcodes", BatchBarcodeScannerClicked),
            new SdkFeature("BARCODE DETECTOR V2"),
#else
            new SdkFeature("BARCODE DETECTOR"),
#endif
            new SdkFeature("Single Scanning", SingleScanning),
            new SdkFeature("Single Scanning Selection Overlay", SingleScanningWithArOverlay),
            new SdkFeature("Batch Barcode Scanning", BatchBarcodeScanning),
            new SdkFeature("Multiple Unique Barcode Scanning", MultipleUniqueBarcodeScanning),
            new SdkFeature("Find and Pick Barcode Scanning", FindAndPickScanning),
            new SdkFeature("Import Image & Detect Barcodes", ImportAndDetectBarcodesClicked),

            new SdkFeature("DOCUMENT SCANNER"),
            new SdkFeature("Scan Document", () => DocumentScannerClicked(withFinder: false)),
            new SdkFeature("Scan Document with Finder", () => DocumentScannerClicked(withFinder: true)),
            new SdkFeature("Import image & Detect Document", ImportButtonClicked),
            new SdkFeature("View Image Results", ViewImageResultsClicked),

            new SdkFeature("DATA DETECTORS"),
            new SdkFeature("MRZ Scanner", MRZScannerClicked),
            new SdkFeature("EHIC Scanner", EHICScannerClicked),
            new SdkFeature("Generic Document Recognizer", GenericDocumentRecognizerClicked),
            new SdkFeature("Check Recognizer", CheckRecognizerClicked),
            new SdkFeature("Text Data Recognizer", TextDataRecognizerClicked),
            new SdkFeature("VIN Recognizer", VinRecognizerClicked),
            new SdkFeature("License Plate Recognizer", LicensePlateRecognizerClicked),
            new SdkFeature("Medical Certificate Recognizer", MedicalCertificateRecognizerClicked),
            
            new SdkFeature("DETECTION FROM IMAGE"),
            new SdkFeature("MRZ Detector", MRZDetectorClicked),
            new SdkFeature("EHIC Detector", EHICDetectorClicked),
            new SdkFeature("Generic Document Detector", GenericDocumentDetectorClicked),
            new SdkFeature("Check Detector", CheckDetectorrClicked),
            new SdkFeature("Medical Certificate Detector", MedicalCertificateDetectorClicked),

            new SdkFeature("MISCELLANEOUS"),
            new SdkFeature("View License Info", ViewLicenseInfoClicked),
            new SdkFeature("Learn more about Scanbot SDK", LearnMoreClicked)
        };

        this.BindingContext = this;
        InitializeComponent();
    }

    /// Item Selected method invoked on the ListView item selection.
    async void SdkFeatureSelected(System.Object sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        if (e?.CurrentSelection?.FirstOrDefault() is SdkFeature feature && feature.DoTask != null)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            await feature.DoTask();
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