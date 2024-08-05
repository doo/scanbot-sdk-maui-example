using ReadyToUseUI.iOS.Utils;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
    
    private void SingleScanning()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2BarcodeScannerConfiguration
        {
            RecognizerConfiguration = new SBSDKUI2BarcodeRecognizerConfiguration
            {
                BarcodeFormats = SBSDKUI2BarcodeFormat.AllFormats,
                Gs1Handling = SBSDKUI2Gs1Handling.Decode
            },
            UseCase = new SBSDKUI2SingleScanningMode
            {
                ConfirmationSheetEnabled = true
            }
        };

        // To try some of the snippets, comment out the above and use an existing configuration object from the Snippets class:
        // var configuration =  Snippets.SingleScanningUseCase;
        // Or any other snippet (like MultipleScanningUseCase, FindAndPickUseCase, ArOverlay, etc.)
            
        var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
            (viewController, cancelled, error, result) =>
            {
                if (!cancelled)
                {
                    viewController.DismissViewController(true, delegate
                    {
                        ShowBarcodeReults(result.Items);
                    });
                }
                else
                {
                    viewController.DismissViewController(true, () => { });
                }
            });

        PresentViewController(controller, false, null);
    }

    private void SingleScanningWithArOverlay()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2BarcodeScannerConfiguration();
        configuration.RecognizerConfiguration.BarcodeFormats = SBSDKUI2BarcodeFormat.AllFormats;

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
                        ShowBarcodeReults(result.Items);
                    });
                }
                else
                {
                    viewController.DismissViewController(true, () => { });
                }
            });


        PresentViewController(controller, false, null);
    }

    private void BatchBarcodeScanning()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2BarcodeScannerConfiguration();
        configuration.RecognizerConfiguration.BarcodeFormats = SBSDKUI2BarcodeFormat.AllFormats;

        var usecases = new SBSDKUI2MultipleScanningMode();
        usecases.Mode = SBSDKUI2MultipleBarcodesScanningMode.Counting;
            
        configuration.UseCase = usecases;
            
        var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
            (viewController, cancelled, error, result) =>
            {
                if (!cancelled)
                {
                    viewController.DismissViewController(true, delegate
                    {
                        ShowBarcodeReults(result.Items);
                    });
                }
                else
                {
                    viewController.DismissViewController(true, () => { });
                }
            });

        PresentViewController(controller, false, null);
    }

    private void MultipleUniqueBarcodeScanning()
    {
        var configuration = new SBSDKUI2BarcodeScannerConfiguration();
        configuration.RecognizerConfiguration.BarcodeFormats = SBSDKUI2BarcodeFormat.AllFormats;
        configuration.UserGuidance.Title.Text = "Please align the QR-/Barcode in the frame above to scan it.";

        var usecases = new SBSDKUI2MultipleScanningMode();
        usecases.Mode = SBSDKUI2MultipleBarcodesScanningMode.Unique;
        usecases.Sheet.Mode = SBSDKUI2SheetMode.CollapsedSheet;
        usecases.SheetContent.ManualCountChangeEnabled = false;
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
                        ShowBarcodeReults(result.Items);
                    });
                }
                else
                {
                    viewController.DismissViewController(true, () => { });
                }
            });

        PresentViewController(controller, false, null);
    }
        
    private void FindAndPickScanning()
    {
        var configuration = new SBSDKUI2BarcodeScannerConfiguration();
            
        var usecases = new SBSDKUI2FindAndPickScanningMode();
        usecases.Sheet.Mode = SBSDKUI2SheetMode.CollapsedSheet;
        usecases.Sheet.CollapsedVisibleHeight = SBSDKUI2CollapsedVisibleHeight.Large;
        usecases.SheetContent.ManualCountChangeEnabled = true;
        usecases.ArOverlay.Visible = true;
        usecases.ArOverlay.AutomaticSelectionEnabled = false;
        usecases.ExpectedBarcodes = new SBSDKUI2ExpectedBarcode[] {
            new SBSDKUI2ExpectedBarcode(barcodeValue: "123456", title: "numeric barcode", image: "https://avatars.githubusercontent.com/u/1454920", count: 4),
            new SBSDKUI2ExpectedBarcode(barcodeValue: "SCANBOT", title: "value barcode", image: "https://avatars.githubusercontent.com/u/1454920", count: 4),
        };

        configuration.UseCase = usecases; 
            
        var controller = SBSDKUI2BarcodeScannerViewController.CreateNew(configuration,
            (viewController, cancelled, error, result) =>
            {
                if (!cancelled)
                {
                    viewController.DismissViewController(true, delegate
                    {
                        ShowBarcodeReults(result.Items);
                    });
                }
                else
                {
                    viewController.DismissViewController(true, () => { });
                }
            });

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
    
    private void ShowBarcodeReults(SBSDKUI2BarcodeItem[] items)
    {
        var viewController = new BarcodeResultListController();
        viewController.NavigateData(items.ToList());
        NavigationController.PushViewController(viewController, true);
    }
}