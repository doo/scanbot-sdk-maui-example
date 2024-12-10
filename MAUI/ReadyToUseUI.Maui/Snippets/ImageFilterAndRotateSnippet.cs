using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics.Platform;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task<PlatformImage> ApplyFiltersAndRotate()
	{
        var image = await ImagePicker.PickImageAsync();

		// Create an instance of `ImageProcessor` passing the input image to the initializer.
		var processor = new ImageProcessor(image);
        
		// Perform operations like rotating, resizing and applying filters to the image.
		// Rotate the image.
		processor.Rotate(DisplayRotation.Rotation90);
        
		// Resize the image.
		processor.Resize(size: 700);
		
		// Create the instances of the filters you want to apply.
		var filter1 = new ScanbotBinarizationFilter { OutputMode = OutputMode.Antialiased };
		var filter2 = new BrightnessFilter { Brightness = 0.4 };
        
		// Set the filters
		processor.ApplyFilters([filter1, filter2]);

		return processor.GetProcessedImage();
	}
}