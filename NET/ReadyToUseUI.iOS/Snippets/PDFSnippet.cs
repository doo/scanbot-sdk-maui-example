using ScanbotSDK.iOS;
using UniformTypeIdentifiers;

namespace ReadyToUseUI.iOS.Snippets;

public class PdfSnippet
{
    void CreatePdfFromDocument(SBSDKScannedDocument scannedDocument)
    {

        // Specify the file URL where the TIFF will be saved to. Nil makes no sense here.
        var outputPdfUrl = new NSUrl("outputPdfUrl");

        // Create the OCR configuration for a searchable Pdf (HOCR).
        var ocrConfiguration = SBSDKOpticalCharacterRecognizerConfiguration.ScanbotOCR;

        // Create the default Pdf rendering options.
        var options = new SBSDKPDFRendererOptions();

        // Set the OCR Configuration.
        options.OcrConfiguration = ocrConfiguration; // Comment this line to not generate HOCR

        // Create the Pdf renderer and pass the Pdf options to it.
        var renderer = new SBSDKPDFRenderer(options, ScanbotUI.DefaultImageStoreEncrypter);
        try
        {
            //If output URL is `null`the default Pdf location of the scanned document will be used.
            renderer.RenderScannedDocument(scannedDocument, outputPdfUrl,
                                (isCompleted, error) =>
                                {
                                    // completion status  
                                });
        }
        catch (Exception error)
        {
            SBSDKLog.LogError("Failed to render Pdf: " + error.Message);
        }
    }

    void CreatePdfFromImage(UIImage image)
    {
        // Specify the file URL where the Pdf will be saved to. Nil makes no sense here.
        var outputPdfUrl = new NSUrl("outputPdfUrl");

        // Create an image storage to save the captured or imported document image to
        var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
        var tmp = NSUrl.FromFilename(string.Format("{0}/{1}", url.Scheme == "file" ? url.Path : url.AbsoluteString,
                            Guid.NewGuid()));
        var location = new SBSDKStorageLocation(tmp);
        var imageStorage = new SBSDKIndexedImageStorage(storageLocation: location);

        // Add the image to the image storage
        imageStorage.AddImage(image);

        // In case you want to encrypt your Pdf file, create encrypter using a password and an encryption mode.
        var encrypter = new SBSDKAESEncrypter(password: "password_example#42",
                            mode: SBSDKAESEncrypterMode.SBSDKAESEncrypterModeAES256);

        // Create the default Pdf rendering options.
        var options = new SBSDKPDFRendererOptions();

        // Set the maximum JPEG Quality.
        options.JpegQuality = 100;

        // Create the Pdf renderer and pass the Pdf options to it.
        var renderer = new SBSDKPDFRenderer(options: options, encrypter: encrypter);
        try
        {
            // Synchronously renders the images from the image storage into a Pdf file with the given page size, and saves the Pdf to the specified URL.
            renderer.RenderImageStorage(imageStorage, null, outputPdfUrl, completion: (isComplete, error) =>
            {
                // completion status  
            });
        }
        catch (Exception error)
        {
            SBSDKLog.LogError("Failed to generate Pdf:" + error.Message);
        }
    }
}