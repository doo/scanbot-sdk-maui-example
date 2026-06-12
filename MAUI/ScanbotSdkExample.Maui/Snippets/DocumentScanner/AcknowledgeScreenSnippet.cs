using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.DocumentQualityAnalyzer;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public class AcknowledgeScreenSnippet
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Set the acknowledgment mode
        // Modes:
        // - UnacceptableQuality: The acknowledgment screen will only be shown when the quality of a scanned page is unacceptable.
        //                        The quality threshold is determined by the document quality analyzer parameters.
        // - Always: The acknowledgment screen will always be shown after each snap, regardless of the scanned page's quality.
        // - None: The acknowledgment screen will be disabled, in effect never shown.
        configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.Always;

        // Set the minimum threshold of unacceptable and uncertain qualities.
        configuration.Screens.Camera.DocumentQualityAnalyzerConfiguration.QualityUnacceptableUncertainThreshold = 0.75;

        // Set the background color for the acknowledgment screen.
        configuration.Screens.Camera.Acknowledgement.BackgroundColor = new ColorValue("#EFEFEF");

        // You can also configure the buttons in the bottom bar of the acknowledgment screen.
        // E.g. to force the user to retake, if the captured document is not acceptable.
        configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenAcceptableButton.Visible = false;

        // Hide the titles of the buttons.
        configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenAcceptableButton.Title.Visible = false;
        configuration.Screens.Camera.Acknowledgement.BottomBar.ProceedAnywayButton.UnacceptableQuality.Title.Visible = false;
        configuration.Screens.Camera.Acknowledgement.BottomBar.RetakeButton.Title.Visible = false;

        // Configure the acknowledgment screen's hint message which is shown if the least acceptable quality is not met.
        configuration.Screens.Camera.Acknowledgement.UnacceptableQualityWarning.Visible = true;
        
        // Launch the scanner
        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }
        
        // Handle the document.
        var scannedDocument = result.Value;
    }
}