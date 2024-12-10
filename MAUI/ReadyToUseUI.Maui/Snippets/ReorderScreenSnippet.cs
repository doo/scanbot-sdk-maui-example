using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task ReorderScreenSnippet()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		// Retrieve the instance of the reorder pages configuration from the main configuration object.
		var reorderScreenConfiguration = configuration.Screens.ReorderPages;

		// Hide the guidance view.
		reorderScreenConfiguration.Guidance.Visible = false;

		// Set the title for the reorder screen.
		reorderScreenConfiguration.TopBarTitle.Text = "Reorder Pages Screen";

		// Set the title for the guidance.
		reorderScreenConfiguration.Guidance.Title.Text = "Reorder";

		// Set the color for the page number text.
		reorderScreenConfiguration.PageTextStyle.Color = Microsoft.Maui.Graphics.Colors.Black;

		// Apply the configurations.
		configuration.Screens.ReorderPages = reorderScreenConfiguration;

		// Present the recognizer view controller modal on this view controller.
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