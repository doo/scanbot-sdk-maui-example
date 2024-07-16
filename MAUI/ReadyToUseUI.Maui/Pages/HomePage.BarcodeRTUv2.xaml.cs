using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Barcode;
using ScanbotSDK.MAUI.Common;

namespace ReadyToUseUI.Maui.Pages;

/// <summary>
/// Home Page of the Application
/// </summary>
public partial class HomePage
{
    /// <summary>
    /// Starts the Barcode scanning.
    /// </summary>
    private async Task SingleScanning()
    {
        try
        {
            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.OpenBarcodeScannerAsync(
                new BarcodeScannerConfiguration
                {
                    RecognizerConfiguration = new BarcodeRecognizerConfiguration
                    {
                        BarcodeFormats = BarcodeFormats.All,
                        Gs1Handling = Gs1Handling.Decode
                    },
                    UseCase = new SingleScanningMode()
                    {
                        ConfirmationSheetEnabled = true
                    }
                });

            // Comment out the above and use the below to try some of our snippets instead:
            // var result = await ScanbotBarcodeSDK.BarcodeScanner.OpenBarcodeScannerAsync(Snippets.SingleScanningUseCase);
            // Or Snippets.MultipleScanningUseCase, Snippets.FindAndPickUseCase, Snippets.ActionBar, etc.

            // var barcodeAsText = result.Items.Select(barcode => $"{barcode.Type}: {barcode.Text}")
            //     .FirstOrDefault() ?? string.Empty;
            //
            // await DisplayAlert("Found barcode", barcodeAsText, "Finish");
            DisplayResults(result);
        }
        catch (TaskCanceledException)
        {
            // for when the user cancels the action
        }
        catch (Exception ex)
        {
            // for any other errors that occur
            Console.WriteLine(ex.Message);
        }
    }

    private async Task SingleScanningWithArOverlay()
    {
        try
        {
            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.OpenBarcodeScannerAsync(
                new BarcodeScannerConfiguration
                {
                    UseCase = new SingleScanningMode()
                    {
                        ArOverlay = new ArOverlayGeneralConfiguration()
                        {
                            Visible = true
                        }
                    }
                });

            // var barcodeAsText = result.Items.Select(barcode => $"{barcode.Type}: {barcode.Text}")
            //     .FirstOrDefault() ?? string.Empty;
            //
            // await DisplayAlert("Found barcode", barcodeAsText, "Finish");
            DisplayResults(result);
        }
        catch (TaskCanceledException)
        {
            // for when the user cancels the action
        }
        catch (Exception ex)
        {
            // for any other errors that occur
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Starts the Batch Barcode Scanning.
    /// </summary>
    private async Task BatchBarcodeScanning()
    {
        try
        {
            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.OpenBarcodeScannerAsync(
                new BarcodeScannerConfiguration
                {
                    RecognizerConfiguration = new BarcodeRecognizerConfiguration
                    {
                        BarcodeFormats = BarcodeFormats.All,
                    },
                    UseCase = new MultipleScanningMode
                    {
                        Mode = MultipleBarcodesScanningMode.Counting
                    }
                });

            // var barcodesAsText = result.Items.Select(barcode => $"{barcode.Type}: {barcode.Text}").ToArray();
            // await DisplayActionSheet("Found barcodes", "Finish", null, barcodesAsText);
            DisplayResults(result);
        }
        catch (TaskCanceledException)
        {
            // for when the user cancels the action
        }
        catch (Exception ex)
        {
            // for any other errors that occur
            Console.WriteLine(ex.Message);
        }
    }

    private async Task MultipleUniqueBarcodeScanning()
    {
        try
        {
            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.OpenBarcodeScannerAsync(
                new BarcodeScannerConfiguration
                {
                    UseCase = new MultipleScanningMode
                    {
                        Mode = MultipleBarcodesScanningMode.Unique,
                        SheetContent = new SheetContent
                        {
                            ManualCountChangeEnabled = false
                        },
                        Sheet = new Sheet
                        {
                            Mode = SheetMode.CollapsedSheet
                        },
                        ArOverlay = new ArOverlayGeneralConfiguration
                        {
                            Visible = true,
                            AutomaticSelectionEnabled = false
                        }
                    },
                    UserGuidance = new UserGuidanceConfiguration
                    {
                        Title = new StyledText { Text = "Please align the QR-/Barcode in the frame above to scan it." }
                    }
                });

            DisplayResults(result);
            // var barcodesAsText = result.Items.Select(barcode => $"{barcode.Type}: {barcode.Text}").ToArray();
            // await DisplayActionSheet("Found barcodes", "Finish", null, barcodesAsText);
        }
        catch (TaskCanceledException)
        {
            // for when the user cancels the action
        }
        catch (Exception ex)
        {
            // for any other errors that occur
            Console.WriteLine(ex.Message);
        }
    }

    private async Task FindAndPickScanning()
    {
        try
        {
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

            findAndPickConfig.SheetContent.SubmitButton.Foreground.Color = new ColorValue("#000000"); //arg string

            // Set the expected barcodes.
            findAndPickConfig.ExpectedBarcodes = new ExpectedBarcode[]
            {
                new ExpectedBarcode(barcodeValue: "123456", title: "numeric barcode",
                    image: "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png",
                    count: 4),
                new ExpectedBarcode(barcodeValue: "SCANBOT", title: "value barcode",
                    image: "https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png",
                    count: 4),
            };

            // Configure other parameters, pertaining to findAndPick-scanning mode as needed.
            configuration.UseCase = findAndPickConfig;
            configuration.RecognizerConfiguration.BarcodeFormats = BarcodeFormats.All.ToArray();

            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.OpenBarcodeScannerAsync(configuration);
            if (result?.Items?.Length  > 0)
            {
                await Navigation.PushAsync(new BarcodeResultPage(result.Items.ToList()));
            }
            // var barcodesAsText = result.Items.Select(barcode => $"{barcode.Type}: {barcode.Text}").ToArray();
            // await DisplayActionSheet("Found barcodes", "Finish", null, barcodesAsText);
        }
        catch (TaskCanceledException)
        {
            // for when the user cancels the action
        }
        catch (Exception ex)
        {
            // for any other errors that occur
            Console.WriteLine(ex.Message);
        }
    }

    private void DisplayResults(BarcodeScannerResult result)
    {
        if (result?.Items?.Length > 0)
        {
            MainThread.InvokeOnMainThreadAsync(async () =>
                await Navigation.PushAsync(new BarcodeResultPage(result.Items.ToList())));
        }
    }
}