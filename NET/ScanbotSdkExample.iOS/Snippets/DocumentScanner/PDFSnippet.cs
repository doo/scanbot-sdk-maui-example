using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class PdfSnippet
{
    void CreatePdfFromDocument(SBSDKScannedDocument scannedDocument)
    {
        NSError error;

        // Specify the file URL where the TIFF will be saved to. Nil makes no sense here.
        var outputPdfUrl = new NSUrl("outputPdfUrl");

        // Create the OCR configuration for a searchable PDF (HOCR).
        var ocrConfiguration = SBSDKOCREngineConfiguration.ScanbotOCR;

        // Create the default PDF rendering options.
        var options = new SBSDKPDFConfiguration();

        try
        {
            // Create the PDF renderer and pass the PDF options to it.
            var renderer = new SBSDKPDFGenerator(options, ocrConfiguration: ocrConfiguration, useEncryptionIfAvailable: AppDelegate.IsEncryptionEnabled, error: out error).GetOrThrow(error);

            //If output URL is `null`the default PDF location of the scanned document will be used.
            renderer.GenerateFromScannedDocument(scannedDocument: scannedDocument, output: outputPdfUrl, completion: (isCompleted, generationError) =>
            {
                // Handle the error
                if (generationError != null)
                {
                    // display error
                    Alert.ValidateAndShowError(generationError);
                    return;
                }

                // completion status - isCompleted
            });
        }
        catch (Exception ex)
        {
            // handle the error thrown from the GetOrThrow(...) function.
            Alert.Show(ex);
        }
    }

    void CreatePdfFromImage(UIImage image)
    {
        NSError error;

        // Convert UIImage to SBSDKImageRef.
        var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

        // Specify the file URL where the PDF will be saved to. Nil makes no sense here.
        var outputPdfUrl = new NSUrl("outputPdfUrl");

        // Create an image storage to save the captured or imported document image to
        var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
        var tmp = NSUrl.FromFilename($"{(url.Scheme == "file" ? url.Path : url.AbsoluteString)}/{Guid.NewGuid()}");
        var location = new SBSDKStorageLocation(tmp);
        var imageStorage = new SBSDKIndexedImageStorage(storageLocation: location, cryptingProvider: ScanbotSDKGlobal.DefaultCryptingProvider);

        // Add the image to the image storage
        imageStorage.AddImage(imageRef);

        // Create the default PDF rendering options.
        var configuration = new SBSDKPDFConfiguration();

        // Set the maximum JPEG Quality.
        configuration.JpegQuality = 100;
        
        try
        {
            // Create the PDF renderer and pass the PDF options to it.
            var renderer = new SBSDKPDFGenerator(configuration: configuration, ocrConfiguration: null, useEncryptionIfAvailable: AppDelegate.IsEncryptionEnabled, error: out error).GetOrThrow(error);

            // Synchronously renders the images from the image storage into a PDF file with the given page size, and saves the PDF to the specified URL.
            renderer.GenerateFromImageStorage(imageStorage, null, outputPdfUrl, completion: (isComplete, generationError) =>
            {
                // Handle the error
                if (generationError != null)
                {
                    // display error
                    Alert.ValidateAndShowError(generationError);
                    return;
                }

                // completion status - isCompleted
            });
        }
        catch (Exception ex)
        {
            // handle the error thrown from the GetOrThrow(...) function.
            Alert.Show(ex);
        }
    }
}