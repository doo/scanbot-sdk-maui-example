using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.PdfOperations;

public static class CreatePdfFromDocumentSnippet
{
    public static void CreatePdfFromDocument(SBSDKScannedDocument scannedDocument, NSUrl outputPdfUrl)
    {
        // Create the OCR configuration for a searchable PDF.
        var ocrConfiguration = SBSDKOCREngineConfiguration.ScanbotOCR;

        // Create the default PDF rendering options.
        var options = new SBSDKPDFConfiguration();

        // Create the PDF renderer and pass the PDF options to it.
        var renderer = new SBSDKPDFGenerator(options, ocrConfiguration: ocrConfiguration,
            useEncryptionIfAvailable: AppDelegate.IsEncryptionEnabled, error: out _);

        // If output URL is `null`, the default PDF location of the scanned document will be used.
        renderer.GenerateFromScannedDocument(scannedDocument: scannedDocument, output: outputPdfUrl,
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
}