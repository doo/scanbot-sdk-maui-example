using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSDK.MAUI.Document;
using ScanbotSdkExample.Maui.ClassicUI.MVVM.Views;
using ScanbotSdkExample.Maui.ClassicUI.Pages;
using ScanbotSdkExample.Maui.Results;
using ScanbotSdkExample.Maui.Utils;

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
        configuration.Screens.Camera.Toolbar.AutoSnappingModeButton.Visible = false;
        configuration.Screens.Camera.Toolbar.ManualSnappingModeButton.Visible = false;
        configuration.Screens.Camera.Toolbar.ImportButton.Title.Visible = true;
        configuration.Screens.Camera.Toolbar.TorchOnButton.Title.Visible = true;
        configuration.Screens.Camera.Toolbar.TorchOffButton.Title.Visible = true;

        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.WhiteSmoke;

        // Configure the hint texts for different scenarios
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";
        
        // launch the scanner
        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (result.IsCanceled) return;

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        await App.Navigation.PushAsync(new ScannedDocumentsPage(result.Value));
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
        configuration.Screens.Camera.Toolbar.AutoSnappingModeButton.Visible = false;
        configuration.Screens.Camera.Toolbar.ManualSnappingModeButton.Visible = false;

        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.WhiteSmoke;

        // Configure the hint texts for different scenarios
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";

        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (result.IsSuccess)
        {
            await App.Navigation.PushAsync(new ScannedDocumentsPage(result.Value));
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
        configuration.Screens.Camera.Toolbar.AutoSnappingModeButton.Visible = true;
        configuration.Screens.Camera.Toolbar.ManualSnappingModeButton.Visible = true;

        // Set colors
        // configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        // configuration.Palette.SbColorOnPrimary = Colors.White;

        // Configure the hint texts for different scenarios
        // e.G
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";

        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = true;

        // Configure bottom bar (further properties like title, icon and  background can also be set for these buttons)
        configuration.Screens.Review.Toolbar.AddButton.BarButton.Visible = true;
        configuration.Screens.Review.Toolbar.RetakeButton.BarButton.Visible = true;
        configuration.Screens.Review.Toolbar.CropButton.BarButton.Visible = true;
        configuration.Screens.Review.Toolbar.RotateButton.BarButton.Visible = true;
        configuration.Screens.Review.Toolbar.DeleteButton.BarButton.Visible = true;

        // Configure `more` popup on review screen
        // e.G
        configuration.Screens.Review.MorePopup.DeleteAll.Icon.Visible = true;
        configuration.Screens.Review.MorePopup.DeleteAll.Title.Text = "Delete all pages";

        // Configure reorder pages screen
        // e.G
        configuration.Screens.ReorderPages.TopBarTitle.Text = "Reorder Pages";
        configuration.Screens.ReorderPages.Guidance.Title.Text = "Reorder Pages";

        // Configure cropping screen
        // e.G
        configuration.Screens.Cropping.TopBarTitle.Text = "Cropping Screen";
        configuration.Screens.Cropping.Toolbar.ResetButton.Visible = true;
        configuration.Screens.Cropping.Toolbar.RotateButton.Visible = true;
        configuration.Screens.Cropping.Toolbar.DetectButton.Visible = true;

        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (result.IsSuccess)
        {
            await App.Navigation.PushAsync(new ScannedDocumentsPage(result.Value));
        }
    }

    public static async Task ClassicDocumentScannerViewClicked()
    {
        await App.Navigation.PushAsync(new ClassicDocumentScannerPage(), true);
    }

    public static async Task ClassicDocumentScannerMvvmViewClicked()
    {
        await App.Navigation.PushAsync(new ClassicDocumentScannerView(), true);
    }

    public static async Task ClassicBarcodeScannerViewClicked()
    {
        MauiProgram.ShouldScanBarcodes = true;
        await App.Navigation.PushAsync(new ClassicBarcodeScannerPage(), true);
    }
}