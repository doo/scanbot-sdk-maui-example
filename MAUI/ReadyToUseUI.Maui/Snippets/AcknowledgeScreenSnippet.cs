using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task AcknowledgeScreenSnippet()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		// Set the acknowledgment mode
		// Modes:
		// - `always`: Runs the quality analyzer on the captured document and always displays the acknowledgment screen.
		// - `badQuality`: Runs the quality analyzer and displays the acknowledgment screen only if the quality is poor.
		// - `none`: Skips the quality check entirely.
		configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.Always;

		// Set the minimum acceptable document quality.
		// Options: excellent, good, reasonable, poor, veryPoor, or noDocument.
		configuration.Screens.Camera.Acknowledgement.MinimumQuality = DocumentQuality.Reasonable;

		// Set the background color for the acknowledgment screen.
		configuration.Screens.Camera.Acknowledgement.BackgroundColor = new ColorValue("#EFEFEF");

		// You can also configure the buttons in the bottom bar of the acknowledgment screen.
		// e.g To force the user to retake, if the captured document is not OK.
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenNotOkButton.Visible = false;

		// Hide the titles of the buttons.
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenNotOkButton.Title.Visible = false;
		configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenOkButton.Title.Visible = false;
		configuration.Screens.Camera.Acknowledgement.BottomBar.RetakeButton.Title.Visible = false;

		// Configure the acknowledgment screen's hint message which is shown if the least acceptable quality is not met.
		configuration.Screens.Camera.Acknowledgement.BadImageHint.Visible = true;

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