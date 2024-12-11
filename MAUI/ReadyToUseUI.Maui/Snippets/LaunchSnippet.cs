using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task LaunchSnippet()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		try
		{
			var document = await ScanbotSDKMain.RTU.DocumentScanner.LaunchAsync(configuration);
			// Handle the document.
		}
		catch (TaskCanceledException)
		{
			// Indicates that the cancel button was tapped.
		}
	}
}