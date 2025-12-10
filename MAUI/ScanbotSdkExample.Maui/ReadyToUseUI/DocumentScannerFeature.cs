using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Document;
using ScanbotSdkExample.Maui.ClassicUI.MVVM.Views;
using ScanbotSdkExample.Maui.ClassicUI.Pages;
using ScanbotSdkExample.Maui.Results;

namespace ScanbotSdkExample.Maui.ReadyToUseUI;

public static class DocumentScannerFeature
{
    public static async Task SingleDocumentScanningClicked()
    {
        var configuration = new DocumentScanningFlow();

        // Disable the multiple page behavior
        configuration.OutputSettings.PagesScanLimit = 1;

        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = false;

        // Enable/Disable Auto Snapping behavior
        configuration.Screens.Camera.CameraConfiguration.AutoSnappingEnabled = true;

        // Configure the animation
        // You can choose between genie animation or checkmark animation
        // Note: Both modes can be further configured to your liking

        // e.G for genie animation
        configuration.Screens.Camera.CaptureFeedback.SnapFeedbackMode = new PageSnapFunnelAnimation();
        // or for checkmark animation
        configuration.Screens.Camera.CaptureFeedback.SnapFeedbackMode = new PageSnapCheckMarkAnimation();

        // Hide the auto snapping enable/disable button
        configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Visible = false;
        configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Visible = false;
        configuration.Screens.Camera.BottomBar.ImportButton.Title.Visible = true;
        configuration.Screens.Camera.BottomBar.TorchOnButton.Title.Visible = true;
        configuration.Screens.Camera.BottomBar.TorchOffButton.Title.Visible = true;

        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.WhiteSmoke;

        // Configure the hint texts for different scenarios
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";

        TestForceCloseDocumentScanner();

        var result = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            await App.Navigation.PushAsync(new ScannedDocumentsPage(result.Result));
        }
    }

    public static async Task SingleFinderDocumentScanningClicked()
    {
        var configuration = new DocumentScanningFlow();

        // Disable the multiple page behavior
        configuration.OutputSettings.PagesScanLimit = 1;

        // Enable view finder
        configuration.Screens.Camera.ViewFinder.Visible = true;
        configuration.Screens.Camera.ViewFinder.AspectRatio = new AspectRatio(width: 3, height: 4);

        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = false;

        // Enable/Disable Auto Snapping behavior
        configuration.Screens.Camera.CameraConfiguration.AutoSnappingEnabled = true;

        // Hide the auto snapping enable/disable button
        configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Visible = false;
        configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Visible = false;

        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.WhiteSmoke;

        // Configure the hint texts for different scenarios
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";

        var result = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            await App.Navigation.PushAsync(new ScannedDocumentsPage(result.Result));
        }
    }

    public static async Task MultipleDocumentScanningClicked()
    {
        var configuration = new DocumentScanningFlow();
        // Enable the multiple page behavior
        configuration.OutputSettings.PagesScanLimit = 0;

        // Enable/Disable Auto Snapping behavior
        configuration.Screens.Camera.CameraConfiguration.AutoSnappingEnabled = true;

        // Hide/Unhide the auto snapping enable/disable button
        configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Visible = true;
        configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Visible = true;

        // Set colors
        //configuration.Palette.SbColorPrimary = new SBSDKUI2Color(uiColor: Colors.ScanbotRed);
        //configuration.Palette.SbColorOnPrimary = new SBSDKUI2Color(uiColor: Colors.NearWhite);

        // Configure the hint texts for different scenarios
        // e.G
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";

        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = true;

        // Configure bottom bar (further properties like title, icon and  background can also be set for these buttons)
        configuration.Screens.Review.BottomBar.AddButton.Visible = true;
        configuration.Screens.Review.BottomBar.RetakeButton.Visible = true;
        configuration.Screens.Review.BottomBar.CropButton.Visible = true;
        configuration.Screens.Review.BottomBar.RotateButton.Visible = true;
        configuration.Screens.Review.BottomBar.DeleteButton.Visible = true;

        // Configure `more` popup on review screen
        // e.G
        configuration.Screens.Review.MorePopup.ReorderPages.Icon.Visible = true;
        configuration.Screens.Review.MorePopup.DeleteAll.Icon.Visible = true;
        configuration.Screens.Review.MorePopup.DeleteAll.Title.Text = "Delete all pages";

        // Configure reorder pages screen
        // e.G
        configuration.Screens.ReorderPages.TopBarTitle.Text = "Reorder Pages";
        configuration.Screens.ReorderPages.Guidance.Title.Text = "Reorder Pages";

        // Configure cropping screen
        // e.G
        configuration.Screens.Cropping.TopBarTitle.Text = "Cropping Screen";
        configuration.Screens.Cropping.BottomBar.ResetButton.Visible = true;
        configuration.Screens.Cropping.BottomBar.RotateButton.Visible = true;
        configuration.Screens.Cropping.BottomBar.DetectButton.Visible = true;

        var result = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            await App.Navigation.PushAsync(new ScannedDocumentsPage(result.Result));
        }
    }

    public static async Task ClassicDocumentScannerViewClicked()
    {
        await App.Navigation.PushAsync(new ClassicDocumentScannerPage(), true);
    }

    public static async Task ClassicDocumentScannerMVVMViewClicked()
    {
        await App.Navigation.PushAsync(new ClassicDocumentScannerView(), true);
    }


    private static void TestForceCloseDocumentScanner()
    {
        HomePage.TestForceCloseScanner(async void () =>
        {
            await ScanbotSDKMain.Rtu.DocumentScanner.ForceCloseScannerAsync();
        });
    }
}