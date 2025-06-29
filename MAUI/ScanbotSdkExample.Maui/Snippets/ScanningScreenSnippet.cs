using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui;

public static partial class Snippets
{

    private static async Task ScanningScreenSnippet()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow(); ;

        // MARK: Set the limit for the number of pages you want to scan.
        configuration.OutputSettings.PagesScanLimit = 30;

        // Pass the DOCUMENT_UUID here to resume an old session, or pass null; to start a new session or to resume a draft session.
        configuration.DocumentUuid = null;

        // Controls whether to resume an existing draft session or start a new one when DOCUMENT_UUID is null;.
        configuration.CleanScanningSession = true;

        // Retrieve the camera screen configuration.
        var cameraScreenConfig = configuration.Screens.Camera;

        // MARK: Configure the user guidance.
        // Configure the top user guidance.
        cameraScreenConfig.TopUserGuidance.Visible = true;
        cameraScreenConfig.TopUserGuidance.Background.FillColor = new ColorValue("#4A000000");
        cameraScreenConfig.TopUserGuidance.Title.Text = "Scan your document";

        // Configure the bottom user guidance.
        cameraScreenConfig.UserGuidance.Visibility = UserGuidanceVisibility.Enabled;
        cameraScreenConfig.UserGuidance.Background.FillColor = new ColorValue("#4A000000");
        cameraScreenConfig.UserGuidance.Title.Text = "Please hold your device over a document";

        // Configure the the scanning assistance overlay.
        cameraScreenConfig.ScanAssistanceOverlay.Visible = true;
        cameraScreenConfig.ScanAssistanceOverlay.BackgroundColor = new ColorValue("#4A000000");
        cameraScreenConfig.ScanAssistanceOverlay.ForegroundColor = new ColorValue("#FFFFFF");

        // Configure the title of the bottom user guidance for different states.
        cameraScreenConfig.UserGuidance.StatesTitles.NoDocumentFound = "No Document";
        cameraScreenConfig.UserGuidance.StatesTitles.BadAspectRatio = "Bad Aspect Ratio";
        cameraScreenConfig.UserGuidance.StatesTitles.BadAngles = "Bad angle";
        cameraScreenConfig.UserGuidance.StatesTitles.TextHintOffCenter = "The document is off center";
        cameraScreenConfig.UserGuidance.StatesTitles.TooSmall = "The document is too small";
        cameraScreenConfig.UserGuidance.StatesTitles.TooNoisy = "The document is too noisy";
        cameraScreenConfig.UserGuidance.StatesTitles.TooDark = "Need more light";
        cameraScreenConfig.UserGuidance.StatesTitles.EnergySaveMode = "Energy save mode is active";
        cameraScreenConfig.UserGuidance.StatesTitles.ReadyToCapture = "Ready to capture";
        cameraScreenConfig.UserGuidance.StatesTitles.Capturing = "Capturing the document";

        // The title of the user guidance when the document ready to be captured in manual mode.
        cameraScreenConfig.UserGuidance.StatesTitles.CaptureManual = "The document is ready to be captured";


        // MARK: Configure the bottom bar and the bottom bar buttons.
        // Set the background color of the bottom bar.
        configuration.Appearance.BottomBarBackgroundColor = new ColorValue("#C8193C");

        // Import button is used to import image from the gallery.
        cameraScreenConfig.BottomBar.ImportButton.Visible = true;
        cameraScreenConfig.BottomBar.ImportButton.Title.Visible = true;
        cameraScreenConfig.BottomBar.ImportButton.Title.Text = "Import";

        // Configure the auto/manual snap button.
        cameraScreenConfig.BottomBar.AutoSnappingModeButton.Title.Visible = true;
        cameraScreenConfig.BottomBar.AutoSnappingModeButton.Title.Text = "Auto";
        cameraScreenConfig.BottomBar.ManualSnappingModeButton.Title.Visible = true;
        cameraScreenConfig.BottomBar.ManualSnappingModeButton.Title.Text = "Manual";

        // Configure the torch off/on button.
        cameraScreenConfig.BottomBar.TorchOnButton.Title.Visible = true;
        cameraScreenConfig.BottomBar.TorchOnButton.Title.Text = "On";
        cameraScreenConfig.BottomBar.TorchOffButton.Title.Visible = true;
        cameraScreenConfig.BottomBar.TorchOffButton.Title.Text = "Off";


        // MARK: Configure the document capture feedback.
        // Configure the camera blink behavior when an image is captured.
        cameraScreenConfig.CaptureFeedback.CameraBlinkEnabled = true;

        // Configure the animation mode. You can choose between a checkmark animation or a document funnel animation.
        // Configure the checkmark animation. You can use the default colors or set your own desired colors for the checkmark.
        cameraScreenConfig.CaptureFeedback.SnapFeedbackMode = new PageSnapCheckMarkAnimation();

        // Or you can choose the funnel animation.
        cameraScreenConfig.CaptureFeedback.SnapFeedbackMode = new PageSnapFunnelAnimation();

        // // Present the recognizer view controller modal on this view controller.
        try
        {
            var document = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
            // Handle the document.
        }
        catch (TaskCanceledException)
        {
            // Indicates that the cancel button was tapped.
        }
    }
}