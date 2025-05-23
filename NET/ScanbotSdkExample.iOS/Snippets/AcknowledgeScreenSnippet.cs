using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets;

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

		// Set the minimum acceptable document quality.
		// Options: excellent, good, reasonable, poor, veryPoor, or noDocument.
		configuration.Screens.Camera.Acknowledgement.MinimumQuality = SBSDKDocumentQuality.Reasonable;

		// Set the background color for the acknowledgment screen.
		configuration.Screens.Camera.Acknowledgement.BackgroundColor = new SBSDKUI2Color(colorString: "#EFEFEF");

		// You can also configure the buttons in the bottom bar of the acknowledgment screen.
		// e.g To force the user to retake, if the captured document is not OK.
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenNotOkButton.Visible = false;

		// Hide the titles of the buttons.
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenNotOkButton.Title.Visible = false;
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenOkButton.Title.Visible = false;
		configuration.Screens.Camera.Acknowledgement.BottomBar.RetakeButton.Title.Visible = false;

		// Configure the acknowledgment screen's hint message which is shown if the least acceptable quality is not met.
		configuration.Screens.Camera.Acknowledgement.BadImageHint.Visible = true;

		// Present the recognizer view controller modal on this view controller.
		SBSDKUI2DocumentScannerController.PresentOn(this, configuration,
			(document) =>
		    {
			    // Completion handler to process the result.
			    if (document != null)
			    {
				    // Handle the document.
			    }
			    else
			    {
				    // Indicates that the cancel button was tapped.
			    }
		    });
	}
}