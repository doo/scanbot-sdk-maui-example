using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class AcknowledgementScreenSnippet : UIViewController
{
    public override void ViewDidLoad()
    {
        if (ScanbotSDKGlobal.IsLicenseValid)
        {
            LaunchDocumentScanner();
        }
    }

    private void LaunchDocumentScanner()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2DocumentScanningFlow();

        // Set the acknowledgment mode
        // Modes:
        // - `always`: Runs the quality analyzer on the captured document and always displays the acknowledgment screen.
        // - `badQuality`: Runs the quality analyzer and displays the acknowledgment screen only if the quality is poor.
        // - `none`: Skips the quality check entirely.
        configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = SBSDKUI2AcknowledgementMode.Always;

        // Set the minimum threshold of unacceptable and uncertain qualities.
        configuration.Screens.Camera.DocumentQualityAnalyzerConfiguration.QualityUnacceptableUncertainThreshold = 0.75;

        // Set the background color for the acknowledgment screen.
        configuration.Screens.Camera.Acknowledgement.BackgroundColor = new SBSDKUI2Color(colorString: "#EFEFEF");

        // You can also configure the buttons in the bottom bar of the acknowledgment screen.
        // E.g. to force the user to retake, if the captured document is not acceptable.
        configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenAcceptableButton.Visible = false;

        // Hide the titles of the buttons.
        configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenAcceptableButton.Title.Visible = false;
        configuration.Screens.Camera.Acknowledgement.BottomBar.ProceedAnywayButton.UnacceptableQuality.Title.Visible = false;
        configuration.Screens.Camera.Acknowledgement.BottomBar.RetakeButton.Title.Visible = false;

        // Configure the acknowledgment screen's hint message which is shown if the least acceptable quality is not met.
        configuration.Screens.Camera.Acknowledgement.UnacceptableQualityWarning.Visible = true;

        try
        {
            // Launch the scanner view controller
            SBSDKUI2DocumentScannerController.PresentOn(viewController: this, configuration: configuration, error: out var presentationError, completion: DocumentScannerCompletion).GetOrThrow(presentationError);
        }
        catch (Exception e)
        {
            // handle the error thrown from the GetOrThrow(...) function, referenced by the PresentOn(...) error object. 
            Console.WriteLine(e);
        }
    }

    private void DocumentScannerCompletion(SBSDKUI2DocumentScannerController controller, SBSDKScannedDocument document, NSError error)
    {
        // check for error
        if (error != null)
        {
            // display error
            Alert.ValidateAndShowError(error);
            return;
        }

        // Handle the document result.
        var documentId = document.Uuid;
    }
}