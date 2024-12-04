using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Imagefilters;
using IO.Scanbot.Sdk.Tiff.Model;
using Uri = Android.Net.Uri;

namespace ReadyToUseUI.Droid.Snippets;

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
		
		var options = new IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterParameters(
							binarizationFilter: new ScanbotBinarizationFilter(),
							dpi:200,
							compression: IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterCompressionOptions.CompressionNone,
							userDefinedFields: Array.Empty<TIFFImageWriterUserDefinedField>());

		var isTiffGenerated = _scanbotSdk.CreateTiffWriter().WriteTIFF(document, tiffFile, options);
		if (isTiffGenerated && document?.TiffUri != null)
		{
			// Do something with the TIFF file
		}
	}
	
	private void CreateTiffFromImages(List<Uri> inputUris)
	{
		var tiffFile = new Java.IO.File("Your path to tif file.");
		
		var options = new IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterParameters(
							binarizationFilter: new ScanbotBinarizationFilter(),
							dpi:200,
							compression: IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterCompressionOptions.CompressionNone,
							userDefinedFields: Array.Empty<TIFFImageWriterUserDefinedField>());
		// Notify the renderer that the images are encrypted with global sdk-encryption settings
		var encryptionEnabled = false;
		
		var isFileCreated = _scanbotSdk.CreateTiffWriter().WriteTIFF(inputUris.ToArray(), encryptionEnabled, tiffFile, options);
		if (isFileCreated && tiffFile.Exists())
		{
			// Do something with the TIFF file
		}
	}
}