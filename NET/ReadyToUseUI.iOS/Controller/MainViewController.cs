using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.Utils;
using ReadyToUseUI.iOS.View;
using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;
using Scanbot.ImagePicker.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public partial class MainViewController : UIViewController
    {
        private MainView contentView;

        private List<ListItem> barcodeDetectorsLegacy;
        private List<ListItem> barcodeDetectors;
        private List<ListItem> documentScanners;
        private List<ListItem> dataDetectors;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            contentView = new MainView();
            View = contentView;

            Title = "Scanbot SDK RTU UI Example";
            
#if LEGACY_EXAMPLES
            barcodeDetectorsLegacy = new List<ListItem>
            {
                new ListItem("Barcode Scanner", ScanBarcode),
                new ListItem("Scan Batch Barcodes", ScanBarcodesInBatch),
                new ListItem("Import and Detect Barcodes", ImportAndDetectBarcode),
            };
#endif
            
            barcodeDetectors = new List<ListItem>
            {
                new ListItem("Single Barcode Scanning", SingleScanning),
                new ListItem("Single Barcode Scanning - AR Overlay", SingleScanningWithArOverlay),
                new ListItem("Batch Barcode Scanning", BatchBarcodeScanning),
                new ListItem("Multiple Unique Barcode Scanning", MultipleUniqueBarcodeScanning),
                new ListItem("Find and Pick Barcode Scanning", FindAndPickScanning),
                new ListItem("Import and Detect Barcodes", ImportAndDetectBarcode),
            };

            documentScanners = new List<ListItem>
            {
                new ListItem("Scan Document", ScanDocument),
                new ListItem("Scan Document with Finder", ScanDocumentWithFinder),
                new ListItem("Import Image", ImportImage),
                new ListItem("View Images", OpenImageListController)
            };

            dataDetectors = new List<ListItem>
            {
                new ListItem("Scan MRZ",                      ScanMrz),
                new ListItem("Scan Health Insurance card",    ScanEhic),
                new ListItem("Generic Document Recognizer",   RecongnizeGenericDocument),
                new ListItem("Check Recognizer",              RecognizeCheck),
                new ListItem("Text Data Recognizer",          TextDataRecognizerTapped),
                new ListItem("VIN Recognizer",                VinRecognizerTapped),
                new ListItem("License Plate Recognizer",      LicensePlateRecognizerTapped),
                new ListItem("Medical Certificate Recognizer", MedicalCertificateRecognizerTapped),
            };

            if (barcodeDetectorsLegacy != null)
            {
                contentView.AddContent("Barcode Scanner V1", barcodeDetectorsLegacy);
                contentView.AddContent("Barcode Scanner V2", barcodeDetectors);
            }
            else
            {
                contentView.AddContent("Barcode Scanner", barcodeDetectors);    
            }
            
            contentView.AddContent("Document Scanner", documentScanners);
            contentView.AddContent("Data Detectors", dataDetectors);

            contentView.LicenseIndicator.Text = Texts.no_license_found_the_app_will_terminate_after_one_minute;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                contentView.LayoutSubviews();
            }

            foreach (var button in contentView.AllButtons)
            {
                button.Click += OnScannerButtonClick;
            }
            
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            foreach (var button in contentView.AllButtons)
            {
                button.Click -= OnScannerButtonClick;
            }
        }

        private void OnScannerButtonClick(object sender, EventArgs e)
        {
            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                contentView.LayoutSubviews();
                return;
            }

            if (sender is ScannerButton button)
            {
                button.Data.DoAction();
            }
        }

        private void ConfigurePaletteV2Barcode(SBSDKUI2BarcodeScannerConfiguration configuration)
        {
            // Retrieve the instance of the palette from the configuration object.
            var palette = configuration.Palette;

            // Configure the colors.
            // The palette already has the default colors set, so you don't have to always set all the colors.
            palette.SbColorPrimary = new SBSDKUI2Color(colorString: "#C8193C");
            palette.SbColorPrimaryDisabled = new SBSDKUI2Color(colorString: "#F5F5F5");
            palette.SbColorNegative = new SBSDKUI2Color(colorString: "#FF3737");
            palette.SbColorPositive = new SBSDKUI2Color(colorString: "#4EFFB4");
            palette.SbColorWarning = new SBSDKUI2Color(colorString: "#FFCE5C");
            palette.SbColorSecondary = new SBSDKUI2Color(colorString: "#FFEDEE");
            palette.SbColorSecondaryDisabled = new  SBSDKUI2Color(colorString: "#F5F5F5");
            palette.SbColorOnPrimary = new SBSDKUI2Color(colorString: "#FFFFFF");
            palette.SbColorOnSecondary = new SBSDKUI2Color(colorString: "#C8193C");;
            palette.SbColorSurface = new SBSDKUI2Color(colorString: "#FFFFFF");
            palette.SbColorOutline = new SBSDKUI2Color(colorString: "#EFEFEF");
            palette.SbColorOnSurfaceVariant =new SBSDKUI2Color(colorString: "#707070");
            palette.SbColorOnSurface = new SBSDKUI2Color(colorString: "#000000");
            palette.SbColorSurfaceLow = new SBSDKUI2Color(colorString: "#26000000");
            palette.SbColorSurfaceHigh = new SBSDKUI2Color(colorString: "#7A000000");
            palette.SbColorModalOverlay = new SBSDKUI2Color(colorString: "#A3000000");
            
            // Set the palette in the barcode scanner configuration object.
            configuration.Palette = palette;
        }
        
        private static bool IsPresented { get; set; }

        public static void ShowPopup(UIViewController controller, string text, Action onClose = null)
        {
            if (IsPresented)
            {
                return;
            }

            IsPresented = true;

            var images = new List<UIImage>();
            var popover = new PopupController(text, images);

            controller.PresentViewController(popover, true, delegate
            {
                popover.Content.CloseButton.Click += delegate
                {
                    IsPresented = false;
                    popover.Dismiss();
                    onClose?.Invoke();
                };
            });
        }
    }
}