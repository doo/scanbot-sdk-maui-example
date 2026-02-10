using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class StorageForPageSnippet
{
    void CreateScannedDocument(List<UIImage> images)
    {		
        // Convert array of UIImage to SBSDKImageRef.
        var imageRefs = images.Select(item => SBSDKImageRef.FromUIImageWithImage(item, new SBSDKRawImageLoadOptions()));

        try
        {
            NSError error;
        
            // Create a new document with the specified maximum image size.
            // Setting the limit to 0, effectively disables the size limit.
            var scannedDocument = new SBSDKScannedDocument(documentImageSizeLimit: 0, error: out error).GetOrThrow(error);

            // add images to the document.
            foreach (var imageRef in imageRefs)
            {
                scannedDocument.AddPageWith(image: imageRef, polygon:new SBSDKPolygon(), filters: [new SBSDKColorDocumentFilter()], error: out error).GetOrThrow(error);
            }
        }
        catch (Exception ex)
        {
            // handle the error thrown from the GetOrThrow(...) function.
            Alert.Show(ex);
        }
    }

    SBSDKScannedDocument CreateFromDocument(SBSDKDocument document)
    {
        try
        {
            // Create the scanned document using convenience initializer `init?(document:documentImageSizeLimit:)`
            // `SBSDKDocument` doesn't support `documentImageSizeLimit`, but you can add it to unify size of the documents.
            var scannedDocument = new SBSDKScannedDocument(document: document, documentImageSizeLimit: 2048,error: out var error).GetOrThrow(error);
            return scannedDocument;
        }
        catch (Exception ex)
        {
            // Handle error.
            Alert.Show(ex);
            return null;
        }
    }

    void AccessImageUrLs(SBSDKScannedDocument scannedDocument)
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
        try
        {
            // Reorder images in the scanned document.
            scannedDocument.MovePageAtTo(sourceIndex, destinationIndex, error: out var error).GetOrThrow(error);
        }
        catch (Exception ex)
        {
            // Handle error.
            Alert.Show(ex);
        }
    }

    void RemoveAllPagesFromScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        try
        {
            // Call the `RemoveAllPagesAndReturnError(onError:)` to remove all pages from the document, but keep the document itself.
            scannedDocument.RemoveAllPagesAndReturnError(error: out var error).GetOrThrow(error);
        }
        catch (Exception ex)
        {
            // Handle error.
            Alert.Show(ex);
        }
    }

    void RemovePdfFromScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        // Create a file manager instance.
        var fileManager = NSFileManager.DefaultManager;

        try
        {
            // Try to remove a PDF file at URL provided by `SBSDKScannedDocument`.
            fileManager.Remove(scannedDocument.PdfURI, error: out var error).GetOrThrow(error);
        }
        catch (Exception ex)
        {
            // Handle error.
            Alert.Show(ex);
        }
    }

    void RemoveTiffFromScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        // Create a file manager instance.
        var fileManager = NSFileManager.DefaultManager;

        try
        {
            // Try to remove a TIFF file at URL provided by `SBSDKScannedDocument`.
            fileManager.Remove(url: scannedDocument.TiffURI, error: out var error).GetOrThrow(error);
        }
        catch (Exception ex)
        {
            // Handle error.
            Alert.Show(ex);
        }
    }

    void DeleteScannedDocument(SBSDKScannedDocument scannedDocument)
    {
        try
        {
            // Try to delate scanned document comparely, including all images and generated files from disk.
            scannedDocument.DeleteAndReturnError(out var error).GetOrThrow(error);
        }
        catch (Exception ex)
        {
            // Handle error.
            Alert.Show(ex);
        }
    }
}