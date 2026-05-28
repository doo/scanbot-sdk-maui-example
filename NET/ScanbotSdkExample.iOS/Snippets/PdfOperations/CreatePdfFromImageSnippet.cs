using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.PdfOperations;

public class CreatePdfFromImageSnippet
{
    public static void CreatePdfFromImage(UIImage image, NSUrl outputPdfUrl)
    {
        try
        {
            // Create the OCR configuration for a searchable PDF.
            var ocrConfiguration = SBSDKOCREngineConfiguration.ScanbotOCR;

            // Convert UIImage to SBSDKImageRef.
            var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

            // Create an image storage to save the captured or imported document image to
            var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
            var tmp = NSUrl.FromFilename($"{(url.Scheme == "file" ? url.Path : url.AbsoluteString)}/{Guid.NewGuid()}");
            var location = new SBSDKStorageLocation(tmp);
            var imageStorage = new SBSDKIndexedImageStorage(storageLocation: location,
                cryptingProvider: ScanbotSDKGlobal.DefaultCryptingProvider);

            // Add the image to the image storage
            imageStorage.AddImage(imageRef);

            // Create the default PDF rendering options.
            var configuration = new SBSDKPDFConfiguration();

            // Create the PDF renderer and pass the PDF options to it.
            var renderer = new SBSDKPDFGenerator(configuration: configuration, ocrConfiguration: ocrConfiguration,
                useEncryptionIfAvailable: true, error: out var error).GetOrThrow(error);

            // Synchronously renders the images from the image storage into a PDF file with the given page size, and saves the PDF to the specified URL.
            renderer.GenerateFromImageStorage(imageStorage, null, outputPdfUrl,
                completion: (isCompleted, generationError) =>
                {
                    if (generationError != null)
                    {
                        // Handle the error
                        Console.WriteLine(generationError);
                        return;
                    }

                    // Handle the result
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}