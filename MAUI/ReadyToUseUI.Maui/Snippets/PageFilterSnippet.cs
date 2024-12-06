using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task PageFilterSnippet()
	{
		// Retrieve the scanned document (replace Guid.Empty with a real value)
		var document = new ScannedDocument(documentUuid: Guid.Empty);
        
		// Retrieve the selected document page.
		var page = document[0]; //or document.Pages.First();
		
		// Apply rotation and filters on the page
		// Create the instances of the filters you want to apply.
		var filter1 = new ScanbotBinarizationFilter { OutputMode = OutputMode.Antialiased };
		var filter2 = new BrightnessFilter { Brightness = 0.4 };

		await page.ModifyPageAsync([filter1, filter2]);
	}
}