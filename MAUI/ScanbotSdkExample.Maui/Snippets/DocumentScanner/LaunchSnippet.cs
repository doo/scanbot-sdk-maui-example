using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class LaunchSnippet
{
	private static async Task LaunchAsync()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

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