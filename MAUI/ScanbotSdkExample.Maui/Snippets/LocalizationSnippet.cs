using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui;

public static partial class Snippets
{
	private static async Task LocalizationSnippet()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		// Retrieve the instance of the localization from the configuration object.
		var localization = configuration.Localization;

		// Configure the strings.
		localization.CameraTopBarTitle = "document.camera.title";
		localization.ReviewScreenSubmitButtonTitle = "review.submit.title";
		localization.CameraUserGuidanceNoDocumentFound = "camera.userGuidance.noDocumentFound";
		localization.CameraUserGuidanceTooDark = "camera.userGuidance.tooDark";

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