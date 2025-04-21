using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class TiffSnippet
{
	void CreateTiff(SBSDKScannedDocument scannedDocument)
	{
		// Specify the file URL where the TIFF will be saved to. Nil makes no sense here.
		var outputTiffUrl = new NSUrl("outputTiffUrl");

		// The `SBSDKTIFFImageWriter` has parameters where you can define various options,
		// e.g. compression algorithm or whether the document should be binarized.
		// For this example we're going to use the default parameters.
		var parameters = new SBSDKTIFFGeneratorParameters();

		// Create the tiff image writer using created parameters.
		var writer = new SBSDKTIFFGenerator(parameters: parameters);

		// Synchronously converts the scanned document to a multipage-TIFF file and writes it to the specified URL.
		//If output URL is `nil`the default TIFF location of the scanned document will be used.
		writer.GenerateFromScannedDocumentToFile(scannedDocument, outputTiffUrl);
	}

	void CreateTiff(UIImage[] images)
	{
		// Specify the file URL where the TIFF will be saved to. Nil makes no sense here.
		var outputTiffUrl = new NSUrl("outputTiffUrl");

		// In case you want to encrypt your TIFF file, create encrypter using a password and an encryption mode.
		var encrypter = new SBSDKAESEncrypter(password: "password_example#42",
							mode: SBSDKAESEncrypterMode.SBSDKAESEncrypterModeAES256);

		// The `SBSDKTIFFImageWriter` has parameters where you can define various options,
		// e.g. compression algorithm or whether the document should be binarized.
		// For this example we're going to use the default parameters.
		var parameters = new SBSDKTIFFGeneratorParameters();

		// Create the tiff image writer using created parameters and the encrypter.
		var tiffImageWriter = new SBSDKTIFFGenerator(parameters: parameters, encrypter: encrypter);

		// Asynchronously writes a TIFF file with scanned images into the defined URL.
		// The completion handler passes a file URL where the file was to be saved, or nil if the operation did not succeed.
		tiffImageWriter.GenerateFromImagesToFileURL(images, outputTiffUrl, 
								completion: (url) =>
					            {
						            // Handle the Url.
					            });
	}
}