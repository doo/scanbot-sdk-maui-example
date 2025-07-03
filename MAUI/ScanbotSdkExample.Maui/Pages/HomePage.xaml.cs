using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Barcode;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Pages;

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
            new SdkFeature("Single Document Scanning", SingleDocumentScanningClicked),
            new SdkFeature("Single Finder Document Scanning", SingleFinderDocumentScanningClicked),
            new SdkFeature("Multiple Document Scanning", MultipleDocumentScanningClicked),
            new SdkFeature("Import Import Image", ImportButtonClicked),
            
            new SdkFeature("CLASSIC COMPONENT"),
            new SdkFeature("Classic Document Scanner", ClassicDocumentScannerViewClicked),

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
            ViewUtils.Alert(this, "Oops!", LicenseInvalidMessage);
            return;
        }

        try
        {
            await feature.DoTask();
        }
        catch
        {
            // ignored
        }
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