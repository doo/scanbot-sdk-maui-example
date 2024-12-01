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

        public bool ShowHeading { get => DoTask == null; }
        public bool ShowFeature { get => DoTask != null; }
    }

    private List<SdkFeature> sdkFeatures;

    public List<SdkFeature> SDKFeatures { get => sdkFeatures; }

    public HomePage()
    {
        sdkFeatures = new List<SdkFeature>
        {
            new SdkFeature("DOCUMENT SCANNER"),
            //new ListItem("Single Document Scanning", SingleDocumentScanning),
            //new ListItem("Single Finder Document Scanning", SingleFinderDocumentScanning),
            //new ListItem("Multiple Document Scanning", MultipleDocumentScanning),
            //new ListItem("Import Image", ImportImage)
            new SdkFeature("Single Document Scanning", SingleDocumentScanningClicked),
            new SdkFeature("Single Finder Document Scanning", SingleFinderDocumentScanningClicked),
            new SdkFeature("Multiple Document Scanning", MultipleDocumentScanningClicked),
            new SdkFeature("Import Import Image", ImportButtonClicked),
            
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

            try
            {
                await feature.DoTask();
            }
            catch
            {

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