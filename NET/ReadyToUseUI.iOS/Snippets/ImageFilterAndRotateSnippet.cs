using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class ImageFilterAndRotateSnippet: UIViewController, IUIImagePickerControllerDelegate
{
	public override void ViewDidAppear(bool animated)
	{
		base.ViewDidAppear(animated);
		
		// Launch image picker.
		var picker = new UIImagePickerController();
		picker.ImagePickerControllerDelegate = this;
	}

	public void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
	{
		var result = info[UIImagePickerController.OriginalImage];
		if (result != null && result is UIImage selectedImage)
		{
			picker.DismissViewController(true, null);
			var resultImage = ApplyFiltersAndRotate(image: selectedImage);
		}
	}

	private UIImage ApplyFiltersAndRotate(UIImage image)
	{
		// Create an instance of `SBSDKImageProcessor` passing the input image to the initializer.
		var processor = new SBSDKImageProcessor(image);
        
		// Perform operations like rotating, resizing and applying filters to the image.
		// Rotate the image.
		processor.Rotate(SBSDKImageRotation.Clockwise90);
        
		// Resize the image.
		processor.Resize(size: 700);
		
		// Create the instances of the filters you want to apply.
		var filter1 = new SBSDKScanbotBinarizationFilter(outputMode: SBSDKOutputMode.Antialiased);
		var filter2 = new SBSDKBrightnessFilter(brightness: 0.4);
        
		// Set the filters
		processor.ApplyFilter([filter1, filter2]);

		return processor.ProcessedImage;
	}
}