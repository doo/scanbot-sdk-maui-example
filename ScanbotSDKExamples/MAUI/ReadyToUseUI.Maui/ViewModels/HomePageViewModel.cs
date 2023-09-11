using System.Collections.ObjectModel;
using BarcodeSDK.MAUI.Configurations;
using BarcodeSDK.MAUI.Constants;
using DocumentSDK.MAUI;
using DocumentSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.Pages;
using ReadyToUseUI.Maui.Utils;
using SBSDK = DocumentSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.ViewModels
{
    public class HomePageViewModel
    {
        /// Current page instance
        private Page CurrentPage => navigation.NavigationStack.FirstOrDefault();

        private INavigation navigation;

        /// List of the Services provided by the SDK.
        private ObservableCollection<SDKService> _sdkServices;
        public ObservableCollection<SDKService> SDKServices
        {
            get => _sdkServices;
        }

        public HomePageViewModel(INavigation navigation)
        {
            this.navigation = navigation;
            _sdkServices = new ObservableCollection<SDKService>
                {
                    new SDKService { Title = "DOCUMENT SCANNER", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.ScanDocument, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ScanDocumentWithFinder, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ImportImageAndDetectDoc, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ViewImageResults, ShowService = true },

                    new SDKService { Title = "BARCODE DETECTOR", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.ScanQRAndBarcodes, ShowService = true },
                     new SDKService { Title = SDKServiceTitle.ScanQRAndBarcodesWithImage, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ScanMultipleQRAndBarcodes, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ImportImageAndDetectBarcode, ShowService = true },

                      new SDKService { Title = "DATA DETECTORS", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.MRZScanner, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.EHICScanner, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.GenericDocRecognizer, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.CheckRecognizer, ShowService = true },

                    new SDKService { Title = "MISCELLANEOUS", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.ViewLicenseInfo, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.LearnMore, ShowService = true }
            };
        }

        /// <summary>
        /// Invokes the SDK Service
        /// </summary>
        /// <param name="serviceTitle"></param>
        public async Task InvokeSDKService(string serviceTitle)
        {
            switch (serviceTitle)
            {
                case SDKServiceTitle.ScanDocument:
                    await ScanningUIClicked(withFinder: false);
                    break;
                case SDKServiceTitle.ScanDocumentWithFinder:
                    await ScanningUIClicked(withFinder: true);
                    break;
                case SDKServiceTitle.ImportImageAndDetectDoc:
                    await ImportButtonClicked();
                    break;

                case SDKServiceTitle.ViewImageResults:
                        ViewImageResultsClicked();
                    break;

                case SDKServiceTitle.ScanQRAndBarcodes:
                    await BarcodeScannerClicked(false);
                    break;

                case SDKServiceTitle.ScanQRAndBarcodesWithImage:
                    await BarcodeScannerClicked(true);
                    break;

                case SDKServiceTitle.ScanMultipleQRAndBarcodes:
                    await BatchBarcodeScannerClicked();
                    break;

                case SDKServiceTitle.ImportImageAndDetectBarcode:
                    await ImportAndDetectBarcodesClicked();
                    break;

                case SDKServiceTitle.MRZScanner:
                    await MRZScannerClicked();
                    break;

                case SDKServiceTitle.EHICScanner:
                    await EHICScannerClicked();
                    break;

                case SDKServiceTitle.GenericDocRecognizer:
                    await GenericDocumentRecognizerClicked();
                    break;

                case SDKServiceTitle.CheckRecognizer:
                    await CheckRecognizerClicked();
                    break;

                case SDKServiceTitle.ViewLicenseInfo:
                    ViewLicenseInfoClicked();
                    break;

                case SDKServiceTitle.LearnMore:
                    await LearnMoreClicked();
                    break;

                default:
                    break;
            }
        }

        //--------------------------------------------
        // Document Scanner
        //--------------------------------------------
        async Task ScanningUIClicked(bool withFinder = false)
        {
            DocumentSDK.MAUI.Models.DocumentScannerResult result;
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

                    // If encryption is enabled, load the decrypted document.
                    // Else accessible via page.Document
                    var blur = await SBSDK.SDKService.EstimateBlurriness(await page.DecryptedDocument());
                    //var blur = await SBSDK.Operations.EstimateBlurriness(page.Document);
                    Console.WriteLine("Estimated blurriness for detected document: " + blur);
                }

                await navigation.PushAsync(new ImageResultsPage());
            }
        }

        // ------------------------------------
        // Barcode Scanner
        // ------------------------------------
        async Task BarcodeScannerClicked(bool withImage)
        {
            var config = new BarcodeScannerConfiguration
            {
                BarcodeFormats = BarcodeTypes.Instance.AcceptedTypes,
                MsiPlesseyChecksumAlgorithm = DetectionPreferences.Instance.BarcodeAdditionalParameters.MsiPlesseyChecksumAlgorithm,
                AcceptedDocumentFormats = DocumentTypes.Instance.AcceptedTypes,
                CodeDensity = BarcodeDensity.High,
                EngineMode = EngineMode.NextGen
            };


            if (DetectionPreferences.Instance.BarcodeAdditionalParameters.Gs1DecodingEnabled is bool gs1DecodingEnabled)
            {
                config.Gs1DecodingEnabled = gs1DecodingEnabled;
            }

            if (DetectionPreferences.Instance.BarcodeAdditionalParameters.StripCheckDigits is bool stripCheckDigits)
            {
                config.StripCheckDigits = stripCheckDigits;
            }

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
                if (result.Barcodes.Count == 0)
                {

                    ViewUtils.Alert(CurrentPage, "Oops!", "No barcodes found, please try again");
                    return;
                }

                if (withImage)
                {
                    await navigation.PushAsync(new BarcodeResultPage(result.Image, result.Barcodes));
                }
                else
                {
                    await navigation.PushAsync(new BarcodeResultPage(result.Barcodes));
                }   
            }
        }

        // ------------------------------------
        // Batch Barcode Scanner
        // ------------------------------------
        async Task BatchBarcodeScannerClicked()
        {
            var config = new BatchBarcodeScannerConfiguration
            {
                BarcodeFormats = BarcodeTypes.Instance.AcceptedTypes,
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
                if (result.Barcodes.Count == 0) 
                {
                    ViewUtils.Alert(CurrentPage, "Oops!", "No barcodes found, please try again");
                    return;
                }
                await navigation.PushAsync(new BarcodeResultPage(null, result.Barcodes));
            }
        }

        // ------------------------------------
        // Import Image to detect barcode.
        // ------------------------------------
        async Task ImportAndDetectBarcodesClicked()
        {
            ImageSource source = await SBSDK.PickerService.PickImageAsync();

            if (source != null)
            {
                var barcodes = await SBSDK.DetectionService.DetectBarcodesFrom(source);
                await navigation.PushAsync(new BarcodeResultPage(source, barcodes));
            }
        }

        // ------------------------------------
        // Import Image to detect document.
        // ------------------------------------
        async Task ImportButtonClicked()
        {
            ImageSource source = await SBSDK.PickerService.PickImageAsync();
            if (source != null)
            {
                // Import the selected image as original image and create a Page object
                var importedPage = await SBSDK.SDKService.CreateScannedPageAsync(source);

                // Run document detection on it
                await importedPage.DetectDocumentAsync();
                await PageStorage.Instance.CreateAsync(importedPage);
                await navigation.PushAsync(new ImageResultsPage());
            }
        }

        // ------------------------------------
        // View scanned images result page.
        // ------------------------------------
        void ViewImageResultsClicked()
        {
            navigation.PushAsync(new ImageResultsPage());
        }

        // ------------------------------------
        // MRZ Scanner
        // ------------------------------------
        async Task MRZScannerClicked()
        {
            MrzScannerConfiguration configuration = new MrzScannerConfiguration();
            configuration.CancelButtonTitle = "Done";
            configuration.FlashEnabled = true;
            configuration.TopBarButtonsColor = Colors.Green;

            var result = await SBSDK.ReadyToUseUIService.LaunchMrzScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ParseMRZResult(result);
                ViewUtils.Alert(CurrentPage, "MRZ Scanner result", message);
            }
        }

        // ------------------------------------
        // EHIC Scanner
        // ------------------------------------
        async Task EHICScannerClicked()
        {
            var configuration = new HealthInsuranceCardConfiguration();
            configuration.CancelButtonTitle = "Done";
            configuration.FlashEnabled = true;
            configuration.TopBarButtonsColor = Colors.Green;
            //TestCloseHealthInsuranceCardScannerAsync();
            var result = await SBSDK.ReadyToUseUIService.LaunchHealthInsuranceCardScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ParseEHICResult(result);
                ViewUtils.Alert(CurrentPage, "EHIC Scanner result", message);
            }
        }

        // ------------------------------------
        // GenericDocumentRecognizer
        // ------------------------------------
        async Task GenericDocumentRecognizerClicked()
        {
            if (!SDKUtils.CheckLicense(CurrentPage)) { return; }

            var configuration = new GenericDocumentRecognizerConfiguration
            {
                DocumentType = GenericDocumentType.DeIdCard
            };
            var result = await SBSDK.ReadyToUseUIService.LaunchGenericDocumentRecognizerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ParseGDRResult(result);
                ViewUtils.Alert(CurrentPage, "GDR Result", message);
            }
        }

        // ------------------------------------
        // Check Recognizer
        // ------------------------------------
        async Task CheckRecognizerClicked()
        {
            if (!SDKUtils.CheckLicense(CurrentPage)) { return; }

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
                ViewUtils.Alert(CurrentPage, "Check Result", message);
            }
        }

        // ------------------------------------
        // View License Info
        // ------------------------------------
        void ViewLicenseInfoClicked()
        {
            var message = SBSDK.IsLicenseValid ? "Scanbot SDK License is valid" : "Scanbot SDK License is expired";
            ViewUtils.Alert(CurrentPage, "License info", message);
        }

        // ------------------------------------
        // Learn More
        // ------------------------------------
        async Task LearnMoreClicked()
        {
            await Browser.OpenAsync(new Uri("https://scanbot.io/developer/net-maui-barcode-scanner-sdk/"), BrowserLaunchMode.SystemPreferred);
        }
    }
}

