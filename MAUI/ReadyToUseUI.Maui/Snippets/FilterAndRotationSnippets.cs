using Microsoft.Maui.Graphics.Platform;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task FilterAndRotateOnDocumentPage(Guid documentUuid)
	{
		// Retrieve the scanned document (replace Guid.Empty with a real value)
		var document = new ScannedDocument(documentUuid: documentUuid);

		// Retrieve the selected document page.
		var page = document[0]; //or document.Pages.First();

		// Apply rotation and filters on the page
		var rotation = DisplayRotation.Rotation90;
		// Create the instances of the filters you want to apply.
		var filter1 = new ScanbotBinarizationFilter { OutputMode = OutputMode.Antialiased };
		var filter2 = new BrightnessFilter { Brightness = 0.4 };

		await page.ModifyPageAsync(filters: [filter1, filter2], rotation: rotation);
	}

	private static PlatformImage FilterAndRotateOnImage(PlatformImage image)
	{
		return new ImageProcessor(image)
				.Rotate(DisplayRotation.Rotation90)
				.Resize(size: 700)
				.ApplyFilters([
					new ScanbotBinarizationFilter { OutputMode = OutputMode.Antialiased },
					new BrightnessFilter { Brightness = 0.4 }
				])
				.GetProcessedImage();
	}
}