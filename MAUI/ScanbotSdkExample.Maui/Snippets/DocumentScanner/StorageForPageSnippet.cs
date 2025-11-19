using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class DocumentOperationSnippets
{
    static void CreateScannedDocument(ImageRef[] images)
    {
        var configuration = new CreateDocumentConfiguration
        {
            // detects the document when adding the page.
            // saves detected document is considered as an individual page(IPage) of the IScannedDocument object.
            DocumentDetection = true
        };
        
        // Create a new document with the specified maximum image size.
        // Setting the limit to 0, effectively disables the size limit.
        var scannedDocument = ScanbotSdkMain.DocumentScanner.CreateDocumentFromImagesAsync(images, configuration);
    }

    static void LoadDocument(Guid documentUuid)
    {
        var loadedDocument = ScanbotSdkMain.DocumentScanner.LoadDocument(documentUuid);
    }

    static void StoredDocumentUuiDs()
    {
        var documentUuids = ScanbotSdkMain.DocumentScanner.StoredDocumentUuids;
    }

    static void ReorderDocumentPages(Guid documentUuid)
    {
        var document = ScanbotSdkMain.DocumentScanner.LoadDocument(documentUuid);

        var sourceIndex = document.PageCount - 1;

        // create destination index.
        var destinationIndex = 0;

        // Reorder images in the scanned document.
        document.MovePage(sourceIndex, destinationIndex);
    }

    static async Task RemoveAllPagesFromDocument(Guid documentUuid)
    {
        var document = ScanbotSdkMain.DocumentScanner.LoadDocument(documentUuid);
        await document.RemoveAllPagesAsync();
    }

    static async Task DeleteDocument(Guid documentUuid)
    {
        await ScanbotSdkMain.DocumentScanner.DeleteDocumentAsync(documentUuid);
        // or
        // await ScanbotSdkMain.DocumentScanner.LoadDocument(documentUuid).DeleteDocumentAsync();
    }

    static async Task DeleteAllDocuments()
    {
        await ScanbotSdkMain.DocumentScanner.DeleteAllDocumentsAsync();
    }
}