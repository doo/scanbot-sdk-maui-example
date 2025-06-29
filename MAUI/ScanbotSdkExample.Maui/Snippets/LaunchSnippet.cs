using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui;

public static partial class Snippets
{
	private static async Task LaunchSnippet()
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