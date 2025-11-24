using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class PageFilterSnippet : UIViewController
{
	public override void ViewDidLoad()
	{
		if (ScanbotSDKGlobal.IsLicenseValid)
		{
			ApplyFilterAndRotateScannedPage();
		}
	}

	private void ApplyFilterAndRotateScannedPage()
	{
		// Retrieve the scanned document
		var document = new SBSDKScannedDocument(documentImageSizeLimit: 0, out _);
		SBSDKScannedDocument.LoadDocumentWithDocumentUuid(documentUuid: "SOME_SAVED_UUID", out var error);
        
		// Retrieve the selected document page.
		SBSDKScannedPage page = document.PageAt(0, out _);
		
		// Apply rotation and filters on the page
		// Create the instances of the filters you want to apply.
		var filter1 = new SBSDKScanbotBinarizationFilter(outputMode: SBSDKOutputMode.Antialiased);
		var filter2 = new SBSDKBrightnessFilter(brightness: 0.4);

		page?.ApplyWithRotation(rotation: page.Rotation, polygon: new SBSDKPolygon(), filters: [filter1, filter2], out _);
	}
}