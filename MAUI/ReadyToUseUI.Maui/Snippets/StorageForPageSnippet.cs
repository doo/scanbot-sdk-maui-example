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
        var scannedDocument = new ScannedDocument();

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

    }

    static async Task RemoveAllPagesFromDocument(Guid documentUuid)
    {
        var document = new ScannedDocument(documentUuid);
        await document.RemoveAllPagesAsync();
    }

    static async Task DeleteDocument(Guid documentUuid)
    {
        await ScannedDocument.DeleteAsync(documentUuid);
    }

    static async Task DeleteAllDocuments()
    {
        await ScannedDocument.DeleteAllDocumentsAsync();
    }
}