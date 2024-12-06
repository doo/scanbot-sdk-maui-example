using Android.Graphics;
using IO.Scanbot.Sdk.Docprocessing;
using ScanbotSdk = IO.Scanbot.Sdk.ScanbotSDK;

namespace ReadyToUseUI.Droid.Snippets;

public class StorageForPageSnippet
{
	void CreateScannedDocument(ScanbotSdk sdk, List<Bitmap> images)
	{
		// Create a new document with the specified maximum image size.
		// Setting the limit to 0, effectively disables the size limit.
		var scannedDocument = sdk.DocumentApi.CreateDocument(documentImageSizeLimit: 2048);

		// add images to the document.
		foreach (var image in images)
		{
			scannedDocument.AddPage(image);
		}
	}

	Document CreateFromUri(ScanbotSdk sdk, Android.Net.Uri uri)
	{
		// Create the scanned document using convenience initializer `init?(document:documentImageSizeLimit:)`
		// `Document` doesn't support `documentImageSizeLimit`, but you can add it to unify size of the documents.
		var scannedDocument = sdk.DocumentApi.CreateDocument(documentImageSizeLimit: 2048);
		scannedDocument.AddPage(uri);
		// Return newly created scanned document
		return scannedDocument;
	}

	void AccessImageURLs(Document scannedDocument)
	{
		// get an array of original image URLs from scanned document.
		var originalImageUris = scannedDocument.Pages.Select(page => page.OriginalFileUri);

		// get an array of document image (processed, rotated, cropped and filtered) URLs from scanned document.
		var documentImageUris = scannedDocument.Pages.Select(page => page.DocumentFileUri);

		// get an array of screen-sized preview image URLs from scanned document.
		var previewImageUris = scannedDocument.Pages.Select(page => page.DocumentPreviewFileUri);
	}

	void ReorderPagesInScannedDocument(Document scannedDocument)
	{
		// Move last and first images in the scanned document.
		// Create source index.
		var sourceIndex = scannedDocument.PageCount - 1;

		// create destination index.
		var destinationIndex = 0;

		// Reorder images in the scanned document.
		scannedDocument.MovePage(sourceIndex, destinationIndex);
	}

	void RemoveAllPagesFromScannedDocument(Document scannedDocument)
	{
		// Call the `removeAllPages()` to remove all pages from the document, but keep the document itself.
		scannedDocument.RemoveAllPages();
	}

	void RemovePdfFromScannedDocument(Document scannedDocument)
	{
		// Take a file from document and delete it
		new Java.IO.File(scannedDocument?.PdfUri?.Path ?? "")?.Delete();
	}

	void RemoveTiffFromScannedDocument(Document scannedDocument)
	{
		// Take a file from document and delete it
		new Java.IO.File(scannedDocument?.TiffUri?.Path ?? "")?.Delete();
	}

	void DeleteScannedDocument(Document scannedDocument)
	{
		// just call delete and document would be deleted
		scannedDocument.Delete();
	}
}