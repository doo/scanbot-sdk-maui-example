using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.Utils;
using ReadyToUseUI.iOS.View;
using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;
using Scanbot.ImagePicker.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public class MainViewController : UIViewController
    {
        private MainView contentView;

        private List<ListItem> barcodeDetectors;
        private List<ListItem> documentScanners;
        private List<ListItem> dataDetectors;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            contentView = new MainView();
            View = contentView;

            Title = "Scanbot SDK RTU UI Example";

            barcodeDetectors = new List<ListItem>
            {
                new ListItem("Scan Barcodes", ScanBarcode),
                new ListItem("Scan Batch Barcodes", ScanBarcodesInBatch),
                new ListItem("Import and Detect Barcodes", ImportAndDetectBarcode)
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

            contentView.AddContent("BARCODE DETECTORS", barcodeDetectors);
            contentView.AddContent("DOCUMENT SCANNER", documentScanners);
            contentView.AddContent("DATA DETECTORS", dataDetectors);

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

        private void ScanBarcode()
        {
            var configuration = SBSDKUIBarcodeScannerConfiguration.DefaultConfiguration;

            // AR overlay configuration
            configuration.TrackingOverlayConfiguration.OverlayEnabled = true;
            configuration.TrackingOverlayConfiguration.AutomaticSelectionEnabled = true;
            configuration.TrackingOverlayConfiguration.OverlayTextFormat = SBSDKBarcodeOverlayFormat.CodeAndType;
            configuration.TrackingOverlayConfiguration.PolygonColor = UIColor.Yellow;
            configuration.TrackingOverlayConfiguration.TextColor = UIColor.Yellow;
            configuration.TrackingOverlayConfiguration.TextContainerColor = UIColor.Black;

            var controller = SBSDKUIBarcodeScannerViewController.CreateNewWithConfiguration(configuration, null);
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

            controller.DidDetectResults += (_, args) =>
            {
                string text = "No barcode detected";
                if (args.BarcodeResults.Length > 0)
                {
                    controller.RecognitionEnabled = false; // stop recognition
                    var result = args.BarcodeResults[0];
                    text = $"Found Barcode(s):\n\n";

                    foreach (var code in args.BarcodeResults)
                    {
                        text += code.Type.Name + ": " + code.RawTextString + "\n";
                    }
                }

                ShowPopup(controller, text, delegate
                {
                    controller.RecognitionEnabled = true; // continue recognition
                });
            };

            PresentViewController(controller, false, null);
        }

        private void ScanBarcodesInBatch()
        {
            var configuration = SBSDKUIBarcodesBatchScannerConfiguration.DefaultConfiguration;

            // AR overlay configuration
            configuration.TrackingOverlayConfiguration.OverlayEnabled = true;
            configuration.TrackingOverlayConfiguration.AutomaticSelectionEnabled = true;
            configuration.TrackingOverlayConfiguration.OverlayTextFormat = SBSDKBarcodeOverlayFormat.CodeAndType;
            configuration.TrackingOverlayConfiguration.PolygonColor = UIColor.Yellow;
            configuration.TrackingOverlayConfiguration.TextColor = UIColor.Yellow;
            configuration.TrackingOverlayConfiguration.TextContainerColor = UIColor.Black;

            var controller = SBSDKUIBarcodesBatchScannerViewController.CreateNewWithConfiguration(configuration, null);
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

            controller.DidFinish += (_, args) =>
            {
                string text = "No barcode detected";
                if (args.BarcodeResults.Length > 0)
                {
                    controller.RecognitionEnabled = false; // stop recognition
                    var result = args.BarcodeResults[0];
                    text = $"Found Barcode(s):\n\n";

                    foreach (var code in args.BarcodeResults)
                    {
                        text += code.Barcode.Type.Name + ": " + code.Barcode.RawTextString + "\n";
                    }
                }
                // the controller object is out of the current view hierarchy as it is dismisssed.
                ShowPopup(AppDelegate.NavigationController, text);
            };

            PresentViewController(controller, false, null);
        }

        private async void ImportAndDetectBarcode()
        {
            var text = "No Barcode detected.";
            var image = await ImagePicker.Instance.PickImageAsync();
            if (image != null)
            {
                SBSDKBarcodeScannerResult[] results = new SBSDKBarcodeScanner().DetectBarCodesOnImage(image);
                if (results != null && results.Length > 0)
                {
                    text = "";
                    foreach (var item in results)
                    {
                        text += item.Type.Name + ": " + item.RawTextString + "\n";
                    }

                    var blur = new SBSDKDocumentQualityAnalyzer().AnalyzeOnImage(image);
                    Console.WriteLine("Blur of imported image: " + blur);
                    text += "(Additionally, blur: " + blur + ")";
                }
            }
            else
            {
                text = "Image format not recognized";
            }

            Alert.Show(this, "Detected Barcodes", text);
        }

        private void ScanDocument()
        {
            var config = SBSDKUIDocumentScannerConfiguration.DefaultConfiguration;

            config.BehaviorConfiguration.CameraPreviewMode = SBSDKVideoContentMode.FitIn;
            config.BehaviorConfiguration.IgnoreBadAspectRatio = true;
            config.BehaviorConfiguration.MultiPageEnabled = true;
            config.TextConfiguration.PageCounterButtonTitle = "%d Page(s)";
            config.TextConfiguration.TextHintOK = "Don't move.\nScanning document...";

            // further configuration properties
            //config.UiConfiguration.BottomBarBackgroundColor = UIColor.Blue;
            //config.UiConfiguration.BottomBarButtonsColor = UIColor.White;
            //config.UiConfiguration.FlashButtonHidden = true;
            // and so on...

            var controller = SBSDKUIDocumentScannerViewController
                .CreateNewWithConfiguration(config, null);

            controller.DidFinishWithDocument += OnScanComplete;
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(controller, false, null);
        }

        private void ScanDocumentWithFinder()
        {
            var config = SBSDKUIFinderDocumentScannerConfiguration.DefaultConfiguration;

            config.BehaviorConfiguration.CameraPreviewMode = SBSDKVideoContentMode.FitIn;
            config.BehaviorConfiguration.IgnoreBadAspectRatio = true;
            config.TextConfiguration.TextHintOK = "Don't move.\nScanning document...";
            config.UiConfiguration.OrientationLockMode = SBSDKOrientationLock.Portrait;
            config.UiConfiguration.FinderAspectRatio = new SBSDKAspectRatio(21.0, 29.7); // a4 portrait

            // further configuration properties
            //config.UiConfiguration.FinderLineColor = UIColor.Red;
            //config.UiConfiguration.TopBarBackgroundColor = UIColor.Blue;
            //config.UiConfiguration.FlashButtonHidden = true;
            // and so on...

            var controller = SBSDKUIFinderDocumentScannerViewController
                .CreateNewWithConfiguration(config, null);

            controller.DidFinishWithDocument += OnScanComplete;
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(controller, false, null);
        }

        private void OnScanComplete(object _, DidFinishWithDocumentEventArgs args)
        {
            if (args.Document.NumberOfPages == 0)
            {
                return;
            }

            for (int i = 0; i < args.Document.NumberOfPages; ++i)
            {
                var page = args.Document.PageAtIndex(i);
                var result = page.DetectDocument(applyPolygonIfOkay: true);
                PageRepository.Add(page);
            }
            OpenImageListController();
        }

        private async void ImportImage()
        {
            var image = await ImagePicker.Instance.PickImageAsync();
            var page = PageRepository.Add(image, new SBSDKPolygon());
            var result = page.DetectDocument(true);
            Console.WriteLine("Attempted document detection on imported page: " + result.Status);

            OpenImageListController();
        }

        private void OpenImageListController()
        {
            var controller = new ImageListController();
            NavigationController.PushViewController(controller, true);
        }

        private void ScanMrz()
        {
            var config = SBSDKUIMRZScannerConfiguration.DefaultConfiguration;
            config.TextConfiguration.CancelButtonTitle = "Done";
            var controller = SBSDKUIMRZScannerViewController
                .CreateNewWithConfiguration(config, null);

            controller.DidDetect += (viewController, args) =>
            {
                controller.RecognitionEnabled = false;
                controller.DismissViewController(true, delegate
                {
                    ShowPopup(this, args.Zone.StringRepresentation);
                });
            };

            PresentViewController(controller, true, null);
        }

        private void ScanEhic()
        {
            var configuration = SBSDKUIHealthInsuranceCardScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var controller = SBSDKUIHealthInsuranceCardScannerViewController
                .CreateNewWithConfiguration(configuration, null);

            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            controller.DidDetectCard += (_, args) =>
            {
                ShowPopup(controller, args.Card.StringRepresentation);
            };
            PresentViewController(controller, false, null);
        }

        private void RecongnizeGenericDocument()
        {
            var configuration = SBSDKUIGenericDocumentRecognizerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            configuration.BehaviorConfiguration.DocumentType = SBSDKUIDocumentType.IdCardFrontBackDE;
            var controller = SBSDKUIGenericDocumentRecognizerViewController.CreateNewWithConfiguration(configuration, null);

            controller.DidFinishWithDocuments += (_, args) =>
            {
                if (args.Documents == null || args.Documents.Length == 0)
                {
                    return;
                }

                // We only take the first document for simplicity
                var firstDocument = args.Documents.First();
                var fields = firstDocument.Fields
                    .Where((f) => f != null && f.Type != null && f.Type.Name != null && f.Value != null && f.Value.Text != null)
                    .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text))
                    .ToList();
                var description = string.Join("\n", fields);
                ShowPopup(this, description);
            };
            PresentViewController(controller, false, null);
        }

        private void RecognizeCheck()
        {
            var configuration = SBSDKUICheckRecognizerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            configuration.BehaviorConfiguration.AcceptedCheckStandards = new SBSDKCheckDocumentRootType[] {
                    SBSDKCheckDocumentRootType.AusCheck(),
                    SBSDKCheckDocumentRootType.FraCheck(),
                    SBSDKCheckDocumentRootType.IndCheck(),
                    SBSDKCheckDocumentRootType.KwtCheck(),
                    SBSDKCheckDocumentRootType.UsaCheck(),
                };
            var controller = SBSDKUICheckRecognizerViewController.CreateNewWithConfiguration(configuration, null);
            controller.DidRecognizeCheck += (_, args) =>
            {
                if (args.Result == null || args.Result.Document == null)
                {
                    return;
                }

                var fields = args.Result.Document.Fields
                    .Where((f) => f != null && f.Type != null && f.Type.Name != null && f.Value != null && f.Value.Text != null)
                    .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text))
                    .ToList();
                var description = string.Join("\n", fields);
                Console.WriteLine(description);

                controller.DismissViewController(true, null);

                ShowPopup(this, description);
            };
            PresentViewController(controller, false, null);
        }

        private void TextDataRecognizerTapped()
        {
            var configuration = SBSDKUITextDataScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUITextDataScannerViewController.CreateNewWithConfiguration(configuration, null);
            scanner.DidFinishStepWithResult += (_, args) =>
            {
                scanner.DismissViewController(true, () => Alert.Show(this, "Result Text:", args?.Result?.Text));
            };

            PresentViewController(scanner, true, null);
        }

        private void VinRecognizerTapped()
        {
            var configuration = SBSDKUIVINScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUIVINScannerViewController.CreateNewWithConfiguration(configuration, null);
            scanner.DidFinishWithResult += (_, args) =>
            {
                scanner.DismissViewController(true, () => Alert.Show(this, "Result Text:", args?.Result?.Text));
            };

            PresentViewController(scanner, true, null);
        }

        private void LicensePlateRecognizerTapped()
        {
            var configuration = SBSDKUILicensePlateScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUILicensePlateScannerViewController.CreateNewWithConfiguration(configuration, null);
            scanner.DidRecognizeLicensePlate += (_, args) =>
            {
                Alert.Show(this, "Result Text:", args?.Result?.RawString);
            };
            PresentViewController(scanner, true, null);
        }

        private void MedicalCertificateRecognizerTapped()
        {
             var configuration = SBSDKUIMedicalCertificateScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUIMedicalCertificateScannerViewController.CreateNewWithConfiguration(configuration, null);
            scanner.DidFinishWithCertificateResult  += (_, args) =>
            {
                Alert.Show(this, "Result Text:", args.Result.StringRepresentation);
            };
            PresentViewController(scanner, true, null);
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
