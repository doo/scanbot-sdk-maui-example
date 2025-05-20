using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ReadyToUseUI.Droid.Snippets;

public class AcknowledgementScreenSnippet : AppCompatActivity
{
	private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
	private const int ScanDocumentRequestCode = 001;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		
		// Returns the singleton instance of the Sdk.
		_scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
		if (_scanbotSdk.LicenseInfo.IsValid)
		{
			LaunchDocumentScanner();
		}
	}
	private void LaunchDocumentScanner()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		// Set the acknowledgment mode
		// Modes:
		// - `ALWAYS`: Runs the quality analyzer on the captured document and always displays the acknowledgment screen.
		// - `BAD_QUALITY`: Runs the quality analyzer and displays the acknowledgment screen only if the quality is poor.
		// - `NONE`: Skips the quality check entirely.
		configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.Always;

		// Set the minimum acceptable document quality.
		// Options: excellent, good, reasonable, poor, veryPoor, or noDocument.
		configuration.Screens.Camera.Acknowledgement.MinimumQuality = DocumentQuality.Reasonable;

		// Set the background color for the acknowledgment screen.
		configuration.Screens.Camera.Acknowledgement.BackgroundColor = new ScanbotColor("#EFEFEF");

		// You can also configure the buttons in the bottom bar of the acknowledgment screen.
		// e.g To force the user to retake, if the captured document is not OK.
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenNotOkButton.Visible = false;

		// Hide the titles of the buttons.
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenNotOkButton.Title.Visible = false;
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenOkButton.Title.Visible = false;
		configuration.Screens.Camera.Acknowledgement.BottomBar.RetakeButton.Title.Visible = false;

		// Configure the acknowledgment screen's hint message which is shown if the least acceptable quality is not met.
		configuration.Screens.Camera.Acknowledgement.BadImageHint.Visible = true;

		// Launch the scanner here .. 
		// Start the Document Scanner activity.
		var intent = DocumentScannerActivity.NewIntent(this, configuration);
		StartActivityForResult(intent, ScanDocumentRequestCode);
	}
}