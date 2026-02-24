using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class TiffSnippet
{
    void CreateTiff(SBSDKScannedDocument scannedDocument)
    {
        NSError error;
        try
        {
            // Specify the file URL where the TIFF will be saved to. Nil makes no sense here.
            var outputTiffUrl = new NSUrl("outputTiffUrl");

            // The `SBSDKTIFFImageWriter` has parameters where you can define various options,
            // e.g. compression algorithm or whether the document should be binarized.
            // For this example we're going to use the default parameters.
            var parameters = new SBSDKTIFFGeneratorParameters();

            // Create the tiff image writer using created parameters.
            var writer = new SBSDKTIFFGenerator(parameters: parameters, useEncryptionIfAvailable: AppDelegate.IsEncryptionEnabled, error: out error).GetOrThrow(error);

            // Synchronously converts the scanned document to a multipage-TIFF file and writes it to the specified URL.
            //If output URL is `nil`the default TIFF location of the scanned document will be used.
            writer.GenerateFromScannedDocumentToFile(scannedDocument: scannedDocument, fileURL: outputTiffUrl, error: out error).GetOrThrow(error);
        }
        catch (Exception e)
        {
            // handle the error thrown from the GetOrThrow(...) function.
            Console.WriteLine(e);
        }
    }

    void CreateTiff(UIImage[] images)
    {
        try
        {
            NSError error;

            // Convert array of UIImage to SBSDKImageRef.
            var imageRefs = images.Select(item => SBSDKImageRef.FromUIImageWithImage(item, new SBSDKRawImageLoadOptions())).ToArray();

            // Specify the file URL where the TIFF will be saved to. Nil makes no sense here.
            var outputTiffUrl = new NSUrl("outputTiffUrl");

            // The `SBSDKTIFFImageWriter` has parameters where you can define various options,
            // e.g. compression algorithm or whether the document should be binarized.
            // For this example we're going to use the default parameters.
            var parameters = new SBSDKTIFFGeneratorParameters();

            // Create the tiff image writer using created parameters and the encrypter.
            var tiffImageWriter = new SBSDKTIFFGenerator(parameters: parameters, useEncryptionIfAvailable: AppDelegate.IsEncryptionEnabled, error: out error).GetOrThrow(error);

            // Asynchronously writes a TIFF file with scanned images into the defined URL.
            // The completion handler passes a file URL where the file was to be saved, or nil if the operation did not succeed.
            tiffImageWriter.GenerateFromImagesToFileURL(imageRefs, outputTiffUrl, completion: (url, generationError) =>
            {
                // Handle the error
                if (generationError != null)
                {
                    // display error
                    Alert.ValidateAndShowError(generationError);
                    return;
                }

                // Handle the url result.
                var outputFilePath = url.Path;
            });
        }
        catch (Exception e)
        {
            // handle the error thrown from the GetOrThrow(...) function.
            Console.WriteLine(e);
        }
    }
}