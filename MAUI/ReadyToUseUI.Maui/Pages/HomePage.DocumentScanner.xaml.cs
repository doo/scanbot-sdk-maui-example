﻿using ReadyToUseUI.Maui.Models;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Constants;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Pages
{
    public partial class HomePage
    {
        private async Task DocumentScannerClicked(bool withFinder = false)
        {
            ScanbotSDK.MAUI.Models.DocumentScannerResult result;
            if (withFinder)
            {
                result = await SBSDK.ReadyToUseUIService.LaunchFinderDocumentScannerAsync(new FinderDocumentScannerConfiguration
                {
                    CameraPreviewMode = CameraPreviewMode.FitIn,
                    IgnoreBadAspectRatio = true,
                    TextHintOK = "Don't move.\nScanning document...",
                    OrientationLockMode = InterfaceOrientation.Portrait,
                    // implicitly the aspect ratio is set to a4 portrait

                    // further configuration properties
                    //FinderLineColor = Colors.Red,
                    //TopBarBackgroundColor = Colors.Blue,
                    //FlashButtonHidden = true,
                    // and so on...
                });
            }
            else
            {
                result = await SBSDK.ReadyToUseUIService.LaunchDocumentScannerAsync(new DocumentScannerConfiguration
                {
                    CameraPreviewMode = CameraPreviewMode.FitIn,
                    IgnoreBadAspectRatio = true,
                    MultiPageEnabled = true,
                    PageCounterButtonTitle = "%d Page(s)",
                    TextHintOK = "Don't move.\nScanning document...",

                    // further configuration properties
                    //BottomBarBackgroundColor = Colors.Blue,
                    //BottomBarButtonsColor = Colors.White,
                    //FlashButtonHidden = true,
                    // and so on...
                });
            }

            if (result.Status == OperationResult.Ok)
            {
                foreach (var page in result.Pages)
                {
                    await PageStorage.Instance.CreateAsync(page);
                }

                await Navigation.PushAsync(new ImageResultsPage());
            }
        }

        private async Task ImportButtonClicked()
        {
            ImageSource source = await SBSDK.PickerService.PickImageAsync();
            if (source != null)
            {
                // Import the selected image as original image and create a Page object
                var importedPage = await SBSDK.SDKService.CreateScannedPageAsync(source);

                // Run document detection on it
                await importedPage.DetectDocumentAsync();
                await PageStorage.Instance.CreateAsync(importedPage);
                await Navigation.PushAsync(new ImageResultsPage());
            }
        }

        private async Task ViewImageResultsClicked()
        {
            await Navigation.PushAsync(new ImageResultsPage());
        }
    }
}
