using System.Collections.ObjectModel;
using System.ComponentModel;
using BarcodeSDK.MAUI.Configurations;
using BarcodeSDK.MAUI.Constants;
using DocumentSDK.MAUI.Constants;
using DocumentSDK.MAUI.Example.Models;
using DocumentSDK.MAUI.Example.Pages;
using DocumentSDK.MAUI.Example.Utils;

namespace DocumentSDK.MAUI.Example.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        /// Current page instance
        private ContentPage CurrentPage => Interaction.CurrentPage;

        /// Used to interact with View
        public IPageInteraction Interaction { get; set; }

        /// List of the Services provided by the SDK.
        private ObservableCollection<SDKService> _sdkServices;
        public ObservableCollection<SDKService> SDKServices
        {
            get
            {
                return _sdkServices;
            }
            set
            {
                _sdkServices = value;
                PropertyChanged?.Invoke(CurrentPage, new PropertyChangedEventArgs(nameof(SDKServices)));
            }
        }

        public HomePageViewModel(IPageInteraction interaction)
        {
            Interaction = interaction;
            InitListViewServices();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InitListViewServices()
        {
            SDKServices = new ObservableCollection<SDKService>
                {
                    new SDKService { Title = "DOCUMENT SCANNER", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.ScanDocument, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ImportImageAndDetectDoc, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ViewImageResults, ShowService = true },

                    new SDKService { Title = "BARCODE DETECTOR", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.ScanQRAndBarcodes, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ScanMultipleQRAndBarcodes, ShowService = true },
                    new SDKService { Title = SDKServiceTitle.ImportImageAndDetectBarcode, ShowService = true },

                    new SDKService { Title = "DATA DETECTORS", ShowSection = true },
                    new SDKService { Title = SDKServiceTitle.MRZScanner, ShowService = true },

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
                    await ScanningUIClicked();
                    break;

                case SDKServiceTitle.ImportImageAndDetectDoc:
                    await ImportButtonClicked();
                    break;

                case SDKServiceTitle.ViewImageResults:
                    ViewImageResultsClicked();
                    break;

                case SDKServiceTitle.ScanQRAndBarcodes:
                    await BarcodeScannerClicked();
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
        async Task ScanningUIClicked()
        {
            var configuration = new DocumentScannerConfiguration
            {
                CameraPreviewMode = CameraPreviewMode.FitIn,
                IgnoreBadAspectRatio = true,
                MultiPageEnabled = true,
                PolygonColor = Colors.Red,
                PolygonColorOK = Colors.Green,
                BottomBarBackgroundColor = Colors.Blue,
                PageCounterButtonTitle = "%d Page(s)",
                //DocumentImageSizeLimit = new Size(2000, 3000)
            };
            var result = await ScanbotSDK.ReadyToUseUIService.LaunchDocumentScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                foreach (var page in result.Pages)
                {
                    await ScannedPage.Instance.Add(page);

                    // If encryption is enabled, load the decrypted document.
                    // Else accessible via page.Document
                    var blur = await ScanbotSDK.SDKService.EstimateBlurriness(await page.DecryptedDocument());
                    //var blur = await SBSDK.Operations.EstimateBlurriness(page.Document);
                    Console.WriteLine("Estimated blurriness for detected document: " + blur);
                }
                await Navigate(new ImageResultsPage());
            }
        }

        /// Navigate to the given content page
        private async Task Navigate(ContentPage contentPage)
        {
            await AddDelay();
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await CurrentPage.Navigation.PushAsync(contentPage);
            });
        }

        /// Explicit Delay - TODO: fix the problem and remove this workaround
        private async Task AddDelay()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await Task.Delay(1000);
            }
        }

        // ------------------------------------
        // Barcode Scanner
        // ------------------------------------
        async Task BarcodeScannerClicked()
        {
            var config = new BarcodeScannerConfiguration
            {
                BarcodeFormats = BarcodeTypes.Instance.AcceptedTypes,
                MsiPlesseyChecksumAlgorithm = DetectionPreferences.Instance.BarcodeAdditionalParameters.MsiPlesseyChecksumAlgorithm,
                AcceptedDocumentFormats = DocumentTypes.Instance.AcceptedTypes,
            };

            if (DetectionPreferences.Instance.BarcodeAdditionalParameters.Gs1DecodingEnabled is bool gs1DecodingEnabled)
            {
                config.Gs1DecodingEnabled = gs1DecodingEnabled;
            }

            if (DetectionPreferences.Instance.BarcodeAdditionalParameters.StripCheckDigits is bool stripCheckDigits)
            {
                config.StripCheckDigits = stripCheckDigits;
            }

            config.CodeDensity = BarcodeDensity.High;
            config.EngineMode = EngineMode.NextGen;
            config.OverlayConfiguration = new SelectionOverlayConfiguration(Colors.Yellow, Colors.Yellow, Colors.Black, Colors.Red, Colors.Red, Colors.White);
            var result = await ScanbotSDK.ReadyToUseUIService.OpenBarcodeScannerView(config);
            if (result.Status == OperationResult.Ok)
            {
                if (result.Barcodes.Count == 0)
                {
                    ViewUtils.Alert(CurrentPage, "Oops!", "No barcodes found, please try again");
                    return;
                }

                var source = result.Image;
                var barcodes = result.Barcodes[0];

                await Navigate(new BarcodeResultPage(new List<BarcodeSDK.MAUI.Models.Barcode> {
                    new BarcodeSDK.MAUI.Models.Barcode {
                            Format = barcodes.Format,
                            Text = barcodes.Text,
                            Image = source
                }}));
            }
        }

        // ------------------------------------
        // Batch Barcode Scanner
        // ------------------------------------
        async Task BatchBarcodeScannerClicked()
        {
            var config = new BatchBarcodeScannerConfiguration();
            config.BarcodeFormats = BarcodeTypes.Instance.AcceptedTypes;
            config.OverlayConfiguration = new SelectionOverlayConfiguration(Colors.Yellow, Colors.Yellow, Colors.Black, Colors.Red, Colors.Red, Colors.White);
            var result = await ScanbotSDK.ReadyToUseUIService.OpenBatchBarcodeScannerView(config);
            if (result.Status == OperationResult.Ok)
            {
                if (result.Barcodes.Count == 0) 
                {
                    ViewUtils.Alert(CurrentPage, "Oops!", "No barcodes found, please try again");
                    return;
                }
                await Navigate(new BarcodeResultPage(null, result.Barcodes));
            }
        }

        // ------------------------------------
        // Import Image to detect barcode.
        // ------------------------------------
        async Task ImportAndDetectBarcodesClicked()
        {
            ImageSource source = await ScanbotSDK.PickerService.PickImageAsync();

            if (source != null)
            {
                var barcodes = await ScanbotSDK.DetectionService.DetectBarcodesFrom(source);
                await Navigate(new BarcodeResultPage(source, barcodes));
            }
        }

        // ------------------------------------
        // Import Image to detect document.
        // ------------------------------------
        async Task ImportButtonClicked()
        {
            ImageSource source = await ScanbotSDK.PickerService.PickImageAsync();
            if (source != null)
            {
                // Import the selected image as original image and create a Page object
                var importedPage = await ScanbotSDK.SDKService.CreateScannedPageAsync(source);

                // Run document detection on it
                await importedPage.DetectDocumentAsync();
                await ScannedPage.Instance.Add(importedPage);
                await Navigate(new ImageResultsPage());
            }
        }

        // ------------------------------------
        // View scanned images result page.
        // ------------------------------------
        void ViewImageResultsClicked()
        {
            CurrentPage.Navigation.PushAsync(new ImageResultsPage());
        }

        // ------------------------------------
        // MRZ Scanner
        // ------------------------------------
        async Task MRZScannerClicked()
        {
            MrzScannerConfiguration configuration = null;

            var result = await ScanbotSDK.ReadyToUseUIService.LaunchMrzScannerAsync(configuration);
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
            var configuration = new HealthInsuranceCardConfiguration { };

            var result = await ScanbotSDK.ReadyToUseUIService.LaunchHealthInsuranceCardScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ParseEHICResult(result);
                ViewUtils.Alert(CurrentPage, "MRZ Scanner result", message);
            }
        }

        // ------------------------------------
        // GenericDocumentRecognizer
        // ------------------------------------
        async Task GenericDocumentRecognizerClicked()
        {
            var configuration = new GenericDocumentRecognizerConfiguration
            {
                DocumentType = GenericDocumentType.DeIdCard
                // Other options...
                // DetailsActionColor = Color.Blue,
                // DetailsBackgroundColor = Color.Yellow,
                // CancelButtonTitle = "CANCEL TEST",
                // ScanFrontSideTitle = "FRONT SIDE TITLE",
                // TopBarButtonsInactiveColor = Color.Tomato,
                // TopBarButtonsColor = Color.Green,
            };

            var result = await ScanbotSDK.ReadyToUseUIService.LaunchGenericDocumentRecognizerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ParseGDRResult(result);
                ViewUtils.Alert(CurrentPage, "GDR Result", message);
            }
        }

        // ------------------------------------
        // View License Info
        // ------------------------------------
        void ViewLicenseInfoClicked()
        {
            var message = "Scanbot SDK License is valid";
            if (!ScanbotSDK.IsLicenseValid)
            {
                message = "Scanbot SDK License is expired";
            }
            ViewUtils.Alert(CurrentPage, "License info", message);
        }

        // ------------------------------------
        // Learn More
        // ------------------------------------
        async Task LearnMoreClicked()
        {
            var uri = new Uri("https://scanbot.io/sdk");
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
    }
}

