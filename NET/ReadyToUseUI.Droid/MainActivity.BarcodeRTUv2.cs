using IO.Scanbot.Sdk.Ui_v2.Barcode;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Common.Mappers;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using ReadyToUseUI.Droid.Utils;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private void SingleScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var intent = BarcodeScannerActivity.NewIntent(this, new BarcodeScannerConfiguration
        {
            RecognizerConfiguration = new BarcodeRecognizerConfiguration
            {
                BarcodeFormats = BarcodeFormat.AllCodes,
                Gs1Handling = Gs1Handling.Decode
            },
            UseCase = new SingleScanningMode()
            {
                ConfirmationSheetEnabled = true
            }
        });
        
        StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
    }

    private void SingleScanningWithArOverlay()
    {
        if (!CheckLicense())
        {
            return;
        }

        var useCase = new SingleScanningMode();
        useCase.ArOverlay.Visible = true;

        var intent = BarcodeScannerActivity.NewIntent(this, new BarcodeScannerConfiguration
        {
            UseCase = useCase
        });
        StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
    }

    private void BatchBarcodeScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var intent = BarcodeScannerActivity.NewIntent(this, new BarcodeScannerConfiguration 
        {
            RecognizerConfiguration = new BarcodeRecognizerConfiguration
            {
                BarcodeFormats = BarcodeFormat.AllCodes  
            },
            UseCase = new MultipleScanningMode
            {
                Mode = MultipleBarcodesScanningMode.Counting
            }
        });
        StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
    }

    private void MultipleUniqueBarcodeScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var useCase = new MultipleScanningMode();
        useCase.Mode = MultipleBarcodesScanningMode.Unique;
        useCase.Sheet.Mode = SheetMode.CollapsedSheet;
        useCase.SheetContent.ManualCountChangeEnabled = false;
        useCase.ArOverlay.Visible = true;
        useCase.ArOverlay.AutomaticSelectionEnabled = false;

        var intent = BarcodeScannerActivity.NewIntent(this, new BarcodeScannerConfiguration
        {
            UseCase = useCase,
            UserGuidance = new UserGuidanceConfiguration
            {
                Title = new StyledText { Text = "Please align the QR-/Barcode in the frame above to scan it." }
            }
        });
        StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
    }

    private void FindAndPickScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var configuration = new BarcodeScannerConfiguration();

        // Initialize the use case for multiple scanning.
        var findAndPickConfig = new FindAndPickScanningMode();

        // Set the sheet mode for the barcodes preview.
        findAndPickConfig.Sheet.Mode = SheetMode.CollapsedSheet;

        // Enable/Disable the automatic selection.
        findAndPickConfig.ArOverlay.AutomaticSelectionEnabled = false;

        // Set the height for the collapsed sheet.
        findAndPickConfig.Sheet.CollapsedVisibleHeight = CollapsedVisibleHeight.Large;

        // Enable manual count change.
        findAndPickConfig.SheetContent.ManualCountChangeEnabled = true;

        // Set the delay before same barcode counting repeat.
        findAndPickConfig.CountingRepeatDelay = 1000;

        // Configure the submit button.
        findAndPickConfig.SheetContent.SubmitButton.Text = "Submit";
            
        findAndPickConfig.SheetContent.SubmitButton.Foreground.Color = new ScanbotColor("#000000"); //arg string

        // Set the expected barcodes.
        findAndPickConfig.ExpectedBarcodes = new List<ExpectedBarcode>() 
        {
            new ExpectedBarcode(barcodeValue: "123456", title: "numeric barcode", image: "https://avatars.githubusercontent.com/u/1454920", count: 4),
            new ExpectedBarcode(barcodeValue: "SCANBOT", title: "value barcode", image: "https://avatars.githubusercontent.com/u/1454920", count: 4)
        };

        // Configure other parameters, pertaining to findAndPick-scanning mode as needed.
        configuration.UseCase = findAndPickConfig;
        configuration.RecognizerConfiguration.BarcodeFormats = BarcodeFormat.Values().ToList();

        var intent = BarcodeScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, BARCODE_DEFAULT_UI_REQUEST_CODE_V2);
    }
}