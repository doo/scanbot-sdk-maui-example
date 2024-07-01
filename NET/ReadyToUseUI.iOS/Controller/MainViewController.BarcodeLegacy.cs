    using ScanbotSDK.iOS;
    
    namespace ReadyToUseUI.iOS.Controller;
    
    public partial class MainViewController
    {
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
    }