using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class StorageForPageSnippet
{
    void CreateScannedDocument(List<UIImage> images)
    {
        // Create a new document with the specified maximum image size.
        // Setting the limit to 0, effectively disables the size limit.
        var scannedDocument = new SBSDKScannedDocument();

        // add images to the document.
        foreach (var image in images)
        {
            scannedDocument.AddPageWith(image, new SBSDKPolygon(), new SBSDKParametricFilter[] { });
        }
    }

    SBSDKScannedDocument CreateFromDocument(SBSDKDocument document)
    {
        // Create the scanned document using convenience initializer `init?(document:documentImageSizeLimit:)`
        // `SBSDKDocument` doesn't support `documentImageSizeLimit`, but you can add it to unify size of the documents.
        var scannedDocument = new SBSDKScannedDocument(document: document, documentImageSizeLimit: 2048);

        // Return newly created scanned document
        return scannedDocument;
    }

    void AccessImageURLs(SBSDKScannedDocument scannedDocument)
    {
        // get an array of original image URLs from scanned document.
        var originalImageUris = scannedDocument.Pages.Select(page => page.OriginalImageURI);

        // get an array of document image (processed, rotated, cropped and filtered) URLs from scanned document.
        var documentImageUris = scannedDocument.Pages.Select(page => page.DocumentImageURI);

        // get an array of screen-sized preview image URLs from scanned document.
        var previewImageUris = scannedDocument.Pages.Select(page => page.DocumentImagePreviewURI);
    }

    void ReorderPagesInScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        // Move last and first images in the scanned document.
        // Create source index.
        var sourceIndex = scannedDocument.PageCount - 1;

        // create destination index.
        var destinationIndex = 0;

        // Reorder images in the scanned document.
        scannedDocument.MovePageAtTo(sourceIndex, destinationIndex);
    }

    void RemoveAllPagesFromScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        // Call the `removeAllPages(onError:)` to remove all pages from the document, but keep the document itself.
        scannedDocument.RemoveAllPagesOnError(
                            (page, error) =>
                            {
                                // Handle error.
                                SBSDKLog.LogError("Failed to remove page" + page.Uuid +
                                                  " with error: " + error.LocalizedDescription);
                            });
    }

    void RemovePdfFromScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        // Create a file manager instance.
        var fileManager = Foundation.NSFileManager.DefaultManager;

        try
        {
            NSError error;

            // Try to remove a PDF file at URL provided by `SBSDKScannedDocument`.
            fileManager.Remove(scannedDocument.PdfURI, out error);
            if (error != null)
            {
                SBSDKLog.LogError("Error message: " + error.LocalizedDescription);
            }
        }
        catch
        {
            // Handle error.
            SBSDKLog.LogError("Failed to remove a PDF at " + scannedDocument.PdfURI);
        }
    }

    void RemoveTIFFFromScannedDocument(SBSDKScannedDocument scannedDocument)
    {

        // Create a file manager instance.
        var fileManager = Foundation.NSFileManager.DefaultManager;

        try
        {
            NSError error;

            // Try to remove a PDF file at URL provided by `SBSDKScannedDocument`.
            fileManager.Remove(scannedDocument.TiffURI, out error);
            if (error != null)
            {
                SBSDKLog.LogError("Error message: " + error.LocalizedDescription);
            }
        }
        catch
        {
            // Handle error.
            SBSDKLog.LogError("Failed to remove a Tiff at " + scannedDocument.TiffURI);
        }
    }

    void DeleteScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        try
        {
            NSError error;
            // Try to delate scanned document comparely, including all images and generated files from disk.
            scannedDocument.DeleteAndReturnError(out error);
            if (error != null)
            {
                SBSDKLog.LogError("Error message: " + error.LocalizedDescription);
            }
        }
        catch (Exception e)
        {
            // Handle error.
            SBSDKLog.LogError("Failed to devare scanned document:" + e.Message);
        }
    }
}