using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.RTU.v1;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Pages
{
    public partial class HomePage
    {
        private async Task BarcodeScannerClicked(bool withImage)
        {
            var config = new BarcodeScannerConfiguration
            {
                BarcodeFormats = BarcodeFormat.Values.ToList(),
                CodeDensity = BarcodeDensity.High,
                EngineMode = EngineMode.NextGen
            };

            if (withImage)
            {
                config.BarcodeImageGenerationType = BarcodeImageGenerationType.CapturedImage;
            }

            config.OverlayConfiguration = new SelectionOverlayConfiguration(
                automaticSelectionEnabled: true,
                overlayFormat: BarcodeTextFormat.Code,
                polygonBackgroundColor: Colors.Yellow,
                textColor: Colors.Yellow,
                textContainerColor: Colors.Black);

            // To see the confirmation dialog in action, uncomment the below and comment out the config.OverlayConfiguration line above.
            //config.ConfirmationDialogConfiguration = new BarcodeConfirmationDialogConfiguration
            //{
            //    Title = "Barcode Detected!",
            //    Message = "A barcode was found.",
            //    ConfirmButtonTitle = "Continue",
            //    RetryButtonTitle = "Try again",
            //    TextFormat = BarcodeTextFormat.CodeAndType
            //};

            var result = await SBSDK.LegacyBarcodeScanner.OpenBarcodeScannerView(config);

            if (result.Status == OperationResult.Ok)
            {
                await Navigation.PushAsync(new BarcodeResultPage(result.Barcodes, withImage ? result.Image : result.ImagePath));
            }
        }

        private async Task BatchBarcodeScannerClicked()
        {
            var config = new BatchBarcodeScannerConfiguration
            {
                BarcodeFormats = BarcodeFormat.Values.ToList(),
                OverlayConfiguration = new SelectionOverlayConfiguration(
                    automaticSelectionEnabled: true,
                    overlayFormat: BarcodeTextFormat.Code,
                    polygonBackgroundColor: Colors.Yellow,
                    textColor: Colors.Yellow,
                    textContainerColor: Colors.Black,
                    polygonBackgroundHighlightedColor: Colors.Red,
                    highlightedTextColor: Colors.Red,
                    highlightedTextContainerColor: Colors.Black),
                SuccessBeepEnabled = true,
                CodeDensity = BarcodeDensity.High,
                EngineMode = EngineMode.NextGen
            };

            var result = await SBSDK.LegacyBarcodeScanner.OpenBatchBarcodeScannerView(config);

            if (result.Status == OperationResult.Ok)
            {
                await Navigation.PushAsync(new BarcodeResultPage(result.Barcodes, ""));
            }
        }

        private async Task ImportAndDetectBarcodesClicked()
        {
            ImageSource source = await SBSDK.PickerService.PickImageAsync();

            if (source != null)
            {
                var barcodes = await SBSDK.DetectionService.DetectBarcodesFrom(source);
                await Navigation.PushAsync(new BarcodeResultPage(barcodes, source));
            }
        }
    }
}

