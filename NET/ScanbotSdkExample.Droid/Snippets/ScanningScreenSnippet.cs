using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets;

public class ScanningScreenSnippet : AppCompatActivity
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

		configuration.OutputSettings.PagesScanLimit = 30;

		// Pass the DOCUMENT_UUID here to resume an old session, or pass null to start a new session or to resume a draft session.
		configuration.DocumentUuid = null;

		// Controls whether to resume an existing draft session or start a new one when DOCUMENT_UUID is null.
		configuration.CleanScanningSession = true;

		// Configure the bottom bar and the bottom bar buttons.
		// Set the background color of the bottom bar.
		configuration.Appearance.BottomBarBackgroundColor = new ScanbotColor("#C8193C");

		// Retrieve the camera screen configuration.
		configuration.Screens.Camera = new CameraScreenConfiguration();

		// Configure the user guidance.
		// Configure the top user guidance.
		configuration.Screens.Camera.TopUserGuidance.Visible = true;
		configuration.Screens.Camera.TopUserGuidance.Background.FillColor = new ScanbotColor("#4A000000");
		configuration.Screens.Camera.TopUserGuidance.Title.Text = "Scan your document";

		// Configure the bottom user guidance.
		configuration.Screens.Camera.UserGuidance.Visibility = UserGuidanceVisibility.Enabled;
		configuration.Screens.Camera.UserGuidance.Background.FillColor = new ScanbotColor("#4A000000");
		configuration.Screens.Camera.UserGuidance.Title.Text = "Please hold your device over a document";

		// Configure the the scanning assistance overlay.
		configuration.Screens.Camera.ScanAssistanceOverlay.Visible = true;
		configuration.Screens.Camera.ScanAssistanceOverlay.BackgroundColor = new ScanbotColor("#4A000000");
		configuration.Screens.Camera.ScanAssistanceOverlay.ForegroundColor = new ScanbotColor("#FFFFFF");

		// Configure the title of the bottom user guidance for different states.
		configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "No Document";
		configuration.Screens.Camera.UserGuidance.StatesTitles.BadAspectRatio = "Bad Aspect Ratio";
		configuration.Screens.Camera.UserGuidance.StatesTitles.BadAngles = "Bad angle";
		configuration.Screens.Camera.UserGuidance.StatesTitles.TextHintOffCenter = "The document is off center";
		configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "The document is too small";
		configuration.Screens.Camera.UserGuidance.StatesTitles.TooNoisy = "The document is too noisy";

		configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more light";
		configuration.Screens.Camera.UserGuidance.StatesTitles.EnergySaveMode = "Energy save mode is active";
		configuration.Screens.Camera.UserGuidance.StatesTitles.ReadyToCapture = "Ready to capture";
		configuration.Screens.Camera.UserGuidance.StatesTitles.Capturing = "Capturing the document";

		// The title of the user guidance when the document ready to be captured in manual mode.
		configuration.Screens.Camera.UserGuidance.StatesTitles.CaptureManual = "The document is ready to be captured";

		// Import button is used to import image from the gallery.
		configuration.Screens.Camera.BottomBar.ImportButton.Visible = true;
		configuration.Screens.Camera.BottomBar.ImportButton.Title.Visible = true;
		configuration.Screens.Camera.BottomBar.ImportButton.Title.Text = "Import";

		// Configure the auto/manual snap button.
		configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Title.Visible = true;
		configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Title.Text = "Auto";
		configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Title.Visible = true;
		configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Title.Text = "Manual";

		// Configure the torch off/on button.
		configuration.Screens.Camera.BottomBar.TorchOnButton.Title.Visible = true;
		configuration.Screens.Camera.BottomBar.TorchOnButton.Title.Text = "On";
		configuration.Screens.Camera.BottomBar.TorchOffButton.Title.Visible = true;
		configuration.Screens.Camera.BottomBar.TorchOffButton.Title.Text = "Off";


		// Configure the document capture feedback.
		// Configure the camera blink behavior when an image is captured.
		configuration.Screens.Camera.CaptureFeedback.CameraBlinkEnabled = true;

		// Configure the animation mode. You can choose between a checkmark animation or a document funnel animation.
		// Configure the checkmark animation. You can use the default colors or set your own desired colors for the checkmark.
		configuration.Screens.Camera.CaptureFeedback.SnapFeedbackMode =
							PageSnapFeedbackMode.PageSnapCheckMarkAnimation();

		// Or you can choose the funnel animation.
		configuration.Screens.Camera.CaptureFeedback.SnapFeedbackMode = PageSnapFeedbackMode.PageSnapFunnelAnimation();

		// Start the Document Scanner activity.
		var intent = DocumentScannerActivity.NewIntent(this, configuration);
		StartActivityForResult(intent, ScanDocumentRequestCode);
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		base.OnActivityResult(requestCode, resultCode, data);

		// Check if the result was cancelled
		if (resultCode != Result.Ok)
		{
			return;
		}

		// Indicates that the cancel button was tapped.
		if (requestCode == ScanDocumentRequestCode)
		{
			// Handle the document result ("documentUuid").
			var documentUuid = data?.GetStringExtra(IO.Scanbot.Sdk.Ui_v2.Common.Activity.ActivityConstants.ExtraKeyRtuResult);
		}
	}
}