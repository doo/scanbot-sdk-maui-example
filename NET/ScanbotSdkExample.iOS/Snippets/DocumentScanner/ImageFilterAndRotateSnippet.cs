using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

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
		try
		{
			NSError error;
			
			// Convert UIImage to SBSDKImageRef.
			var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

			// Create an instance of `SBSDKImageProcessor` passing the input image to the initializer.
			var processor = new SBSDKImageProcessor();

			// Perform operations like rotating, resizing and applying filters to the image.
			// Rotate the image.
			imageRef = processor.RotateWithImage(image: imageRef, rotation: SBSDKImageRotation.Clockwise90, error: out error).GetOrThrow(error);

			// Resize the image.	
			imageRef = processor.ResizeWithImage(image: imageRef, size: 700, error: out error).GetOrThrow(error);

			// Create the instances of the filters you want to apply.
			var filter1 = new SBSDKScanbotBinarizationFilter(outputMode: SBSDKOutputMode.Antialiased);
			var filter2 = new SBSDKBrightnessFilter(brightness: 0.4);

			// Set the filters
			imageRef = processor.ApplyFiltersWithImage(image: imageRef, filters: [filter1, filter2], error: out error).GetOrThrow(error);

			return imageRef?.ToUIImageAndReturnError(error: out error).GetOrThrow(error);
		}
		catch (Exception ex)
		{
			// handle the error thrown from the GetOrThrow(...) function.
			Alert.Show(ex);
			return null;
		}
	}
}