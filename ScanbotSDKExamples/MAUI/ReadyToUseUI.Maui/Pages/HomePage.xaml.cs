using ScanbotSDK.MAUI.Configurations;
using ScanbotSDK.MAUI.Constants;
using ScanbotSDK.MAUI;
using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.Utils;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage : ContentPage
{
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
            new SdkFeature("Scan Document", () => DocumentScannerClicked(withFinder: false)),
            new SdkFeature("Scan Document with Finderr", () => DocumentScannerClicked(withFinder: false)),
            new SdkFeature("Import image & Detect Document", ImportButtonClicked),
            new SdkFeature("View Image Results", ViewImageResultsClicked),

            new SdkFeature("BARCODE DETECTOR"),
            new SdkFeature("Scan QR & Barcodes", () => BarcodeScannerClicked(withImage: false)),
            new SdkFeature("Scan QR & Barcodes With Image", () => BarcodeScannerClicked(withImage: true)),
            new SdkFeature("Scan Multiple QR & Barcodes", BatchBarcodeScannerClicked),
            new SdkFeature("Import Image & Detect Barcodes", ImportAndDetectBarcodesClicked),

            new SdkFeature("DATA DETECTORS"),
            new SdkFeature("MRZ Scanner", MRZScannerClicked),
            new SdkFeature("EHIC Scanner", EHICScannerClicked),
            new SdkFeature("Generic Document Recognizer", GenericDocumentRecognizerClicked),
            new SdkFeature("Check Recognizer", CheckRecognizerClicked),

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
        if (e?.CurrentSelection?.FirstOrDefault() is SdkFeature feature)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }
            await feature.DoTask();
        }
        FeaturesCollectionView.SelectedItem = null;
    }


    //--------------------------------------------
    // Document Scanner
    //--------------------------------------------
    private async Task DocumentScannerClicked(bool withFinder = false)
    {
        ScanbotSDK.MAUI.Models.DocumentScannerResult result;
        if (withFinder)
        {
            result = await SBSDK.ReadyToUseUIService.LaunchFinderDocumentScannerAsync(new FinderDocumentScannerConfiguration
            {
                CameraPreviewMode = CameraPreviewMode.FitIn,
                IgnoreBadAspectRatio = true,
                TextHintOK = "Don't move.\nScanning document...",
                OrientationLockMode = InterfaceOrientation.Portrait,
                // implicitly the aspect ratio is set to a4 portrait

                // further configuration properties
                //FinderLineColor = Colors.Red,
                //TopBarBackgroundColor = Colors.Blue,
                //FlashButtonHidden = true,
                // and so on...
            });
        }
        else
        {
            result = await SBSDK.ReadyToUseUIService.LaunchDocumentScannerAsync(new DocumentScannerConfiguration
            {
                CameraPreviewMode = CameraPreviewMode.FitIn,
                IgnoreBadAspectRatio = true,
                MultiPageEnabled = true,
                PageCounterButtonTitle = "%d Page(s)",
                TextHintOK = "Don't move.\nScanning document...",

                // further configuration properties
                //BottomBarBackgroundColor = Colors.Blue,
                //BottomBarButtonsColor = Colors.White,
                //FlashButtonHidden = true,
                // and so on...
            });
        }

        if (result.Status == OperationResult.Ok)
        {
            foreach (var page in result.Pages)
            {
                await PageStorage.Instance.CreateAsync(page);
            }

            await Navigation.PushAsync(new ImageResultsPage());
        }
    }

    // ------------------------------------
    // Barcode Scanner
    // ------------------------------------
    private async Task BarcodeScannerClicked(bool withImage)
    {
        var config = new BarcodeScannerConfiguration
        {
            BarcodeFormats = Enum.GetValues<BarcodeFormat>().ToList(),
            CodeDensity = BarcodeDensity.High,
            EngineMode = EngineMode.NextGen,
            AcceptedDocumentFormats = Enum.GetValues<BarcodeDocumentFormat>().ToList(),            
        };

        if (withImage)
        {
            config.BarcodeImageGenerationType = BarcodeImageGenerationType.CapturedImage;
        }

        config.OverlayConfiguration = new SelectionOverlayConfiguration(
            automaticSelectionEnabled: true,
            overlayFormat: BarcodeTextFormat.Code,
            polygon: Colors.Yellow,
            text: Colors.Yellow,
            textContainer: Colors.Black);

        // To see the confirmation dialog in action, uncomment the below and comment out the config.OverlayConfiguration line above.
        //config.ConfirmationDialogConfiguration = new BarcodeConfirmationDialogConfiguration
        //{
        //    Title = "Barcode Detected!",
        //    Message = "A barcode was found.",
        //    ConfirmButtonTitle = "Continue",
        //    RetryButtonTitle = "Try again",
        //    TextFormat = BarcodeTextFormat.CodeAndType
        //};

        var result = await SBSDK.ReadyToUseUIService.OpenBarcodeScannerView(config);

        if (result.Status == OperationResult.Ok)
        {
            await Navigation.PushAsync(new BarcodeResultPage(result.Barcodes, withImage ? result.Image : result.ImagePath));
        }
    }

    // ------------------------------------
    // Batch Barcode Scanner
    // ------------------------------------
    private async Task BatchBarcodeScannerClicked()
    {
        var config = new BatchBarcodeScannerConfiguration
        {
            BarcodeFormats = Enum.GetValues<BarcodeFormat>().ToList(),
            OverlayConfiguration = new SelectionOverlayConfiguration(
                automaticSelectionEnabled: true,
                overlayFormat: BarcodeTextFormat.Code,
                polygon: Colors.Yellow,
                text: Colors.Yellow,
                textContainer: Colors.Black)
        };

        var result = await SBSDK.ReadyToUseUIService.OpenBatchBarcodeScannerView(config);

        if (result.Status == OperationResult.Ok)
        {
            await Navigation.PushAsync(new BarcodeResultPage(result.Barcodes, ""));
        }
    }

    // ------------------------------------
    // Import Image to detect barcode.
    // ------------------------------------
    private async Task ImportAndDetectBarcodesClicked()
    {
        ImageSource source = await SBSDK.PickerService.PickImageAsync();

        if (source != null)
        {
            var barcodes = await SBSDK.DetectionService.DetectBarcodesFrom(source);
            await Navigation.PushAsync(new BarcodeResultPage(barcodes, source));
        }
    }

    // ------------------------------------
    // Import Image to detect document.
    // ------------------------------------
    private async Task ImportButtonClicked()
    {
        ImageSource source = await SBSDK.PickerService.PickImageAsync();
        if (source != null)
        {
            // Import the selected image as original image and create a Page object
            var importedPage = await SBSDK.SDKService.CreateScannedPageAsync(source);

            // Run document detection on it
            await importedPage.DetectDocumentAsync();
            await PageStorage.Instance.CreateAsync(importedPage);
            await Navigation.PushAsync(new ImageResultsPage());
        }
    }

    // ------------------------------------
    // View scanned images result page.
    // ------------------------------------
    private async Task ViewImageResultsClicked()
    {
        await Navigation.PushAsync(new ImageResultsPage());
    }

    // ------------------------------------
    // MRZ Scanner
    // ------------------------------------
    private async Task MRZScannerClicked()
    {
        MrzScannerConfiguration configuration = new MrzScannerConfiguration();
        configuration.CancelButtonTitle = "Done";
        configuration.TopBarButtonsColor = Colors.Green;

        var result = await SBSDK.ReadyToUseUIService.LaunchMrzScannerAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ParseMRZResult(result);
            ViewUtils.Alert(this, "MRZ Scanner result", message);
        }
    }

    // ------------------------------------
    // EHIC Scanner
    // ------------------------------------
    private async Task EHICScannerClicked()
    {
        var configuration = new HealthInsuranceCardConfiguration();
        configuration.CancelButtonTitle = "Done";
        configuration.TopBarButtonsColor = Colors.Green;
        var result = await SBSDK.ReadyToUseUIService.LaunchHealthInsuranceCardScannerAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ParseEHICResult(result);
            ViewUtils.Alert(this, "EHIC Scanner result", message);
        }
    }

    // ------------------------------------
    // GenericDocumentRecognizer
    // ------------------------------------
    private async Task GenericDocumentRecognizerClicked()
    {
        if (!SDKUtils.CheckLicense(this)) { return; }

        var configuration = new GenericDocumentRecognizerConfiguration
        {
            DocumentType = GenericDocumentType.DeIdCard
        };
        var result = await SBSDK.ReadyToUseUIService.LaunchGenericDocumentRecognizerAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ParseGDRResult(result);
            ViewUtils.Alert(this, "GDR Result", message);
        }
    }

    // ------------------------------------
    // Check Recognizer
    // ------------------------------------
    private async Task CheckRecognizerClicked()
    {
        if (!SDKUtils.CheckLicense(this)) { return; }

        var configuration = new CheckRecognizerConfiguration
        {
            AcceptedCheckStandards = new List<CheckStandard>() {
                    CheckStandard.USA,
                    CheckStandard.AUS,
                    CheckStandard.IND,
                    CheckStandard.FRA,
                    CheckStandard.KWT,
                }
        };

        var result = await SBSDK.ReadyToUseUIService.LaunchCheckRecognizerAsync(configuration);

        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ParseCheckResult(result);
            ViewUtils.Alert(this, "Check Result", message);
        }
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
