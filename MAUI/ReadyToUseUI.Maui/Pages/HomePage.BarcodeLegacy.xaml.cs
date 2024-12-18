﻿using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Barcode;
using ScanbotSDK.MAUI.RTU.v1;
using BarcodeFormat = ScanbotSDK.MAUI.Barcode.BarcodeFormat;
using BarcodeScannerConfiguration = ScanbotSDK.MAUI.RTU.v1.BarcodeScannerConfiguration;
using EngineMode = ScanbotSDK.MAUI.EngineMode;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Pages
{
    public partial class HomePage
    {
        private async Task BarcodeScannerClicked(bool withImage)
        {
            var config = new BarcodeScannerConfiguration
            {
                BarcodeFormats = Enum.GetValues<BarcodeFormat>().ToList(),
                CodeDensity = CodeDensity.High,
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
                await Navigation.PushAsync(new BarcodeResultPage(result.Barcodes,
                    withImage ? result.Image : result.ImagePath));
            }
        }

        private async Task BatchBarcodeScannerClicked()
        {
            var config = new BatchBarcodeScannerConfiguration
            {
                BarcodeFormats = Enum.GetValues<BarcodeFormat>().ToList(),
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
                CodeDensity = CodeDensity.High,
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
            IsLoading = true;
            ImageSource source = await SBSDK.PickerService.PickImageAsync();
            if (source == null)
            {
                IsLoading = false;
                return;
            }

            var barcodes = await SBSDK.DetectionService.DetectBarcodesFrom(source);
            await Navigation.PushAsync(new BarcodeResultPage(barcodes, source));
            IsLoading = false;
        }
    }
}