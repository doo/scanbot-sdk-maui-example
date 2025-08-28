using Android.Graphics;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Core;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Document;
using IO.Scanbot.Sdk.Imagefilters;
using ScanbotSdkExample.Droid.Utils;

namespace ScanbotSdkExample.Droid.Snippets;

public class ImageFilterAndRotateSnippet : AppCompatActivity
{
	private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;

	protected override async void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Returns the singleton instance of the Sdk.
		_scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);

		if (_scanbotSdk.LicenseInfo.IsValid)
		{
			var resultImage = await ApplyImageFilterAndRotate();
		}
	}

	private async Task<Bitmap> ApplyImageFilterAndRotate()
	{
		// Pick image from gallery
		var bitmap = await ImagePickerServiceActivity.PickImageAsync(this);

		// Create a document detector
		var documentDetector = _scanbotSdk.CreateDocumentScanner();

		// Run detection on the picked image
		var detectionResult = documentDetector.ScanFromBitmap(bitmap);

		// Validate the result status and retrieve the detected polygon.
		if (detectionResult == null || detectionResult.Status != DocumentDetectionStatus.Ok)
		{
			return bitmap;
		}

		// If the result is an acceptable polygon, we warp the image into the polygon.
		var imageProcessor = new ImageProcessor();

		// Perform operations like rotating, resizing and applying filters to the image.
		// Rotate the image.
		imageProcessor.Rotate(ImageRotation.Clockwise90);

		// You can crop the image using the polygon if you want.
		imageProcessor.Crop(detectionResult.PointsNormalized);

		// Resize the image.
		imageProcessor.Resize(700);

		var binarizationFilter = new ScanbotBinarizationFilter(OutputMode.Antialiased);
		var brightnessFilter = new BrightnessFilter(0.4);
		imageProcessor.ApplyFilter(binarizationFilter);
		imageProcessor.ApplyFilter(brightnessFilter);

		// Result Image
		return imageProcessor.ProcessedBitmap();
	}
}
