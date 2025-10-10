using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Tiffgeneration;
using ScanbotSDK.Droid.Helpers;
using Boolean = Java.Lang.Boolean;
using Uri = Android.Net.Uri;

namespace ScanbotSdkExample.Droid.Snippets;

public class TIFFSnippet : AppCompatActivity
{
	private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Returns the singleton instance of the Sdk.
		_scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		var document = _scanbotSdk.DocumentApi.LoadDocument("Your_Document_Id");
		
		if (_scanbotSdk.LicenseInfo.IsValid)
		{
			CreateTiffFromDocument(document);
		}
	}

	private void CreateTiffFromDocument(Document document)
	{
		var tiffFile = new Java.IO.File(document?.TiffUri?.Path ?? "");
		
		var defaultParams = TiffGeneratorParameters.Default();
		
		var options = new TiffGeneratorParameters(
			CompressionMode.None,
			jpegQuality: defaultParams.JpegQuality,
			zipCompressionLevel: defaultParams.ZipCompressionLevel,
			dpi: 200,
			userFields: Array.Empty<UserField>(),
			ParametricFilter.ScanbotBinarizationFilter());

		var isTiffGenerated = _scanbotSdk.CreateTiffGeneratorManager().GenerateFromDocument(document, tiffFile, options).GetValue<bool>();
		if (isTiffGenerated && document?.TiffUri != null)
		{
			// Do something with the TIFF file
		}
	}
	
	private void CreateTiffFromImages(List<Uri> inputUris)
	{
		var tiffFile = new Java.IO.File("Your path to tif file.");
		
		var defaultParams = TiffGeneratorParameters.Default();
		
		var options = new TiffGeneratorParameters(
			CompressionMode.None,
			jpegQuality: defaultParams.JpegQuality,
			zipCompressionLevel: defaultParams.ZipCompressionLevel,
			dpi: 200,
			userFields: Array.Empty<UserField>(),
			ParametricFilter.ScanbotBinarizationFilter());

		// Notify the renderer that the images are encrypted with global sdk-encryption settings
		var encryptionEnabled = false;
		var isFileCreated = _scanbotSdk.CreateTiffGeneratorManager().GenerateFromUris(inputUris.ToArray(), encryptionEnabled, tiffFile, options, PageBinarization.EnabledIfBinarizationFilterSet).GetValue<bool>();
		if (isFileCreated && tiffFile.Exists())
		{
			// Do something with the TIFF file
		}
	}
}