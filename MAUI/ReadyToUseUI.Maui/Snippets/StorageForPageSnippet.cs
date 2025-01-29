using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using Microsoft.Maui.Graphics.Platform;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
    static void CreateScannedDocument(PlatformImage[] images)
    {
        // Create a new document with the specified maximum image size.
        // Setting the limit to 0, effectively disables the size limit.
        var scannedDocument = new ScannedDocument(documentImageSizeLimit: 0);

        // add images to the document.
        foreach (var image in images)
        {
            scannedDocument.AddPage(image);
        }
    }

    static void LoadDocument(Guid documentUuid)
    {
        var loadedDocument = new ScannedDocument(documentUuid);
    }

    static void StoredDocumentUUIDs()
    {
        var documentUuids = ScannedDocument.StoredDocumentUuids;
    }

    static void ReorderDocumentPages(Guid documentUuid)
    {
        var document = new ScannedDocument(documentUuid);

        var sourceIndex = document.PageCount - 1;

        // create destination index.
        var destinationIndex = 0;

        // Reorder images in the scanned document.
        document.MovePage(sourceIndex, destinationIndex);
    }

    static async Task RemoveAllPagesFromDocument(Guid documentUuid)
    {
        var document = new ScannedDocument(documentUuid);
        await document.RemoveAllPagesAsync();
    }

    static async Task DeleteDocument(Guid documentUuid)
    {
        await ScannedDocument.DeleteAsync(documentUuid);
        // or await new ScannedDocument(documentUuid).DeleteAsync();
    }

    static async Task DeleteAllDocuments()
    {
        await ScannedDocument.DeleteAllDocumentsAsync();
    }
}