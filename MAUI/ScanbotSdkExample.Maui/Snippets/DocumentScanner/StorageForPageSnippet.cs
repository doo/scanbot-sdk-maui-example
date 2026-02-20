using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Image;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class DocumentOperationSnippets
{
    static void CreateScannedDocument(ImageRef[] images)
    {
        var configuration = new CreateDocumentOptions
        {
            // detects the document when adding the page.
            // saves detected document is considered as an individual page(IPage) of the IScannedDocument object.
            DocumentDetection = true
        };
        
        // Create a new document with the specified maximum image size.
        // Setting the limit to 0, effectively disables the size limit.
        var scannedDocument = ScanbotSDKMain.Document.CreateDocumentFromImagesAsync(images, configuration);
    }

    static void LoadDocument(string documentUuid)
    {
        var loadedDocument = ScanbotSDKMain.Document.LoadDocument(documentUuid);
    }

    static void StoredDocumentUuiDs()
    {
        var documentUuids = ScanbotSDKMain.Document.StoredDocumentUuids;
    }

    static void ReorderDocumentPages(string documentUuid)
    {
        var result = ScanbotSDKMain.Document.LoadDocument(documentUuid);
        if (!result.IsSuccess)
        {
            // access the returned exception with `result.Error`
            return;
        }
        
        var sourceIndex = result.Value.PageCount - 1;

        // create destination index.
        var destinationIndex = 0;

        // Reorder images in the scanned document.
        result.Value.MovePage(sourceIndex, destinationIndex);
    }

    static async Task RemoveAllPagesFromDocument(string documentUuid)
    {
        var result = ScanbotSDKMain.Document.LoadDocument(documentUuid);
        if (!result.IsSuccess)
        {
            // access the returned exception with `result.Error`
            return;
        }
        await result.Value.RemoveAllPagesAsync();
    }

    static async Task DeleteDocument(string documentUuid)
    {
        await ScanbotSDKMain.Document.DeleteDocumentAsync(documentUuid);
        
        // or -- When working with the existing document object.
        // IScannedDocument document = null; 
        // await document?.DeleteDocumentAsync();
    }

    static async Task DeleteAllDocuments()
    {
        await ScanbotSDKMain.Document.DeleteAllDocumentsAsync();
    }
}