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
        private List<ListItem> barcodeV2Detectors;
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
                new ListItem("Import and Detect Barcodes", ImportAndDetectBarcode),
            };
            
            barcodeV2Detectors = new List<ListItem>
            {
                new ListItem("Single RTUUI V2", ShowSingleBarcodeScannerFromRTUUI),
                new ListItem("Single AR RTUUI V2", ShowSingleARBarcodeScannerFromRTUUI),
                new ListItem("Single AR AUTO_SELECT RTUUI V2", ShowSingleARAutoSelectBarcodeScannerFromRTUUI),
                new ListItem("Multiple RTUUI V2", ShowMultiBarcodeScannerFromRTUUI),
                new ListItem("Multiple SHEET RTUUI V2", ShowMultiSheetBarcodeScannerFromRTUUI),
                new ListItem("Multiple SHEET AR COUNT RTUUI V2", ShowMultiSheetARCountAutoSelectBarcodeScannerFromRTUUI),
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
            contentView.AddContent("BARCODE DETECTORS V2", barcodeV2Detectors);
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

        private void ShowSingleBarcodeScannerFromRTUUI()
        {
            // Create the default configuration object.
            var configuration = new SBSDKUI2BarcodeScannerConfiguration();
            
            configuration.RecognizerConfiguration.BarcodeFormats =
                new[] { SBSDKUI2BarcodeFormat.AustraliaPost, SBSDKUI2BarcodeFormat.Aztec };
            
            var usecases = new SBSDKUI2SingleScanningMode{};
            usecases.ConfirmationSheetEnabled = true;
            usecases.ArOverlay.Visible = false;
            usecases.ArOverlay.AutomaticSelectionEnabled = false;

            configuration.UseCase = usecases;
            
            var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
                (viewController, cancelled, error, result) =>
                {
                    if (!cancelled)
                    {
                        viewController.DismissViewController(true, delegate
                        {
                            ShowPopup(this, result?.ToJson());
                        });
                    }
                    else
                    {
                        viewController.DismissViewController(true, () => { });
                    }
                });

            PresentViewController(controller, false, null);
        }
        
        private void ShowSingleARBarcodeScannerFromRTUUI()
        {
            // Create the default configuration object.
            var configuration = new SBSDKUI2BarcodeScannerConfiguration();
            
            var usecases = new SBSDKUI2SingleScanningMode();
            usecases.ConfirmationSheetEnabled = true;
            usecases.ArOverlay.Visible = true;
            usecases.ArOverlay.AutomaticSelectionEnabled = false;

            configuration.UseCase = usecases;
            
            
            var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
                (viewController, cancelled, error, result) =>
                {
                    if (!cancelled)
                    {
                        viewController.DismissViewController(true, delegate
                        {
                            ShowPopup(this, result?.ToJson());
                        });
                    }
                    else
                    {
                        viewController.DismissViewController(true, () => { });
                    }
                });

            PresentViewController(controller, false, null);
        }

        private void ShowSingleARAutoSelectBarcodeScannerFromRTUUI()
        {
            // Create the default configuration object.
            var configuration = new SBSDKUI2BarcodeScannerConfiguration();
            
            var usecases = new SBSDKUI2SingleScanningMode();
            usecases.ConfirmationSheetEnabled = true;
            usecases.ArOverlay.Visible = true;
            usecases.ArOverlay.AutomaticSelectionEnabled = true;
            
            configuration.UseCase = usecases;
            
            var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
                (viewController, cancelled, error, result) =>
                {
                    if (!cancelled)
                    {
                        viewController.DismissViewController(true, delegate
                        {
                            ShowPopup(this, result?.ToJson());
                        });
                    }
                    else
                    {
                        viewController.DismissViewController(true, () => { });
                    }
                });

            PresentViewController(controller, false, null);
        }

        private void ShowMultiBarcodeScannerFromRTUUI()
        {
            // Create the default configuration object.
            var configuration = new SBSDKUI2BarcodeScannerConfiguration();
            
            var usecases = new SBSDKUI2MultipleScanningMode();
            usecases.Mode = SBSDKUI2MultipleBarcodesScanningMode.Unique;
            usecases.Sheet.Mode = SBSDKUI2SheetMode.Button;
            usecases.ArOverlay.Visible = true;
            
            configuration.UseCase = usecases;
            
            var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
                (viewController, cancelled, error, result) =>
                {
                    if (!cancelled)
                    {
                        viewController.DismissViewController(true, delegate
                        {
                            ShowPopup(this, result?.ToJson());
                        });
                    }
                    else
                    {
                        viewController.DismissViewController(true, () => { });
                    }
                });

            PresentViewController(controller, false, null);
        }

        private void ShowMultiSheetBarcodeScannerFromRTUUI()
        {
            var configuration = new SBSDKUI2BarcodeScannerConfiguration();

            configuration.RecognizerConfiguration.BarcodeFormats =
                new[] { SBSDKUI2BarcodeFormat.AustraliaPost,  SBSDKUI2BarcodeFormat.Aztec };
            
            var usecases = new SBSDKUI2MultipleScanningMode();
            usecases.Mode = SBSDKUI2MultipleBarcodesScanningMode.Unique;
            usecases.Sheet.Mode = SBSDKUI2SheetMode.CollapsedSheet;
            usecases.ArOverlay.Visible = false;
            
            configuration.UseCase = usecases;
            
            var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
                (viewController, cancelled, error, result) =>
                {
                    if (!cancelled)
                    {
                        viewController.DismissViewController(true, delegate
                        {
                            ShowPopup(this, result?.ToJson());
                        });
                    }
                    else
                    {
                        viewController.DismissViewController(true, () => { });
                    }
                });

            PresentViewController(controller, false, null);
        }

        private void ShowMultiSheetARCountAutoSelectBarcodeScannerFromRTUUI()
        {
            var configuration = new SBSDKUI2BarcodeScannerConfiguration();
            
            var usecases = new SBSDKUI2MultipleScanningMode();
            usecases.Mode = SBSDKUI2MultipleBarcodesScanningMode.Counting;
            usecases.Sheet.Mode = SBSDKUI2SheetMode.CollapsedSheet;
            usecases.Sheet.CollapsedVisibleHeight = SBSDKUI2CollapsedVisibleHeight.Large;
            usecases.SheetContent.ManualCountChangeEnabled = true;
            usecases.ArOverlay.Visible = true;
            usecases.ArOverlay.AutomaticSelectionEnabled = true;
            
            configuration.UseCase = usecases; 
            
            var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
                (viewController, cancelled, error, result) =>
                {
                    if (!cancelled)
                    {
                        viewController.DismissViewController(true, delegate
                        {
                            ShowPopup(this, result?.ToJson());
                        });
                    }
                    else
                    {
                        viewController.DismissViewController(true, () => { });
                    }
                });

            PresentViewController(controller, false, null);
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

            var controller = SBSDKUIBarcodeScannerViewController.CreateWithConfiguration(configuration,  @delegate: null);
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

            controller.DidDetectResults += (_, args) =>
            {
                string text = "No barcode detected";
                if (args.BarcodeResults.Length > 0)
                {
                    controller.IsRecognitionEnabled = false; // stop recognition
                    var result = args.BarcodeResults[0];
                    text = $"Found Barcode(s):\n\n";

                    foreach (var code in args.BarcodeResults)
                    {
                        text += code.Type.Name + ": " + code.RawTextString + "\n";
                    }
                }

                ShowPopup(controller, text, delegate
                {
                    controller.IsRecognitionEnabled = true; // continue recognition
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

            var controller = SBSDKUIBarcodesBatchScannerViewController.CreateNew(configuration: configuration, @delegate: null);
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;

            controller.DidFinishWithResults += (_, args) =>
            {
                string text = "No barcode detected";
                if (args.BarcodeResults.Length > 0)
                {
                    controller.IsRecognitionEnabled = false; // stop recognition
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

                    var quality = new SBSDKDocumentQualityAnalyzer().AnalyzeOnImage(image);
                    Console.WriteLine("The quality of the imported image: " + quality.ToString());
                    text += "(Additionally, blur: " + quality.ToString() + ")";
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
            config.BehaviorConfiguration.IsMultiPageEnabled = true;
            config.TextConfiguration.PageCounterButtonTitle = "%d Page(s)";
            config.TextConfiguration.TextHintOK = "Don't move.\nScanning document...";

            // further configuration properties
            //config.UiConfiguration.BottomBarBackgroundColor = UIColor.Blue;
            //config.UiConfiguration.BottomBarButtonsColor = UIColor.White;
            //config.UiConfiguration.FlashButtonHidden = true;
            // and so on...

            var controller = SBSDKUIDocumentScannerViewController
                .CreateNew(configuration: config, @delegate: null);

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
                .CreateNew(configuration: config, @delegate: null);

            controller.DidFinishWithDocument += OnScanComplete;
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(controller, false, null);
        }

        private void OnScanComplete(object _, FinishWithDocumentEventArgs args)
        {
            if (args?.Document?.Pages == null && args.Document.Pages.Length == 0)
            {
                return;
            }

            for (int i = 0; i < args.Document.Pages.Length; ++i)
            {
                var page = args.Document.PageAtIndex(i);
                var result = page.DetectDocumentAndApplyPolygonIfOkay(true);
                PageRepository.Add(page);
            }
            OpenImageListController();
        }

        private async void ImportImage()
        {
            var image = await ImagePicker.Instance.PickImageAsync();
            var page = PageRepository.Add(image, new SBSDKPolygon());
            var result = page.DetectDocumentAndApplyPolygonIfOkay(true);
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
                .CreateWithConfiguration(config, null);

            controller.DidDetect += (viewController, args) =>
            {
                controller.IsRecognitionEnabled = false;
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
                .CreateWithConfiguration(configuration, null);

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
            var controller = SBSDKUIGenericDocumentRecognizerViewController.CreateWithConfigurationAndDelegate(configuration, null);

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
                    SBSDKCheckDocumentRootType.AusCheck,
                    SBSDKCheckDocumentRootType.FraCheck,
                    SBSDKCheckDocumentRootType.IndCheck,
                    SBSDKCheckDocumentRootType.KwtCheck,
                    SBSDKCheckDocumentRootType.UsaCheck,
                };
            var controller = SBSDKUICheckRecognizerViewController.CreateWithConfiguration(configuration, null);
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
            var scanner = SBSDKUITextDataScannerViewController.CreateWithConfiguration(configuration, null);
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
            var scanner = SBSDKUIVINScannerViewController.CreateNew(configuration: configuration, @delegate: null);
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
            var scanner = SBSDKUILicensePlateScannerViewController.CreateNew(configuration: configuration, @delegate: null);
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
            var scanner = SBSDKUIMedicalCertificateScannerViewController.CreateWithConfiguration(configuration, null);
            scanner.DidFinishWithResult  += (_, args) =>
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