using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

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
		var document = new SBSDKScannedDocument(documentUuid: "SOME_SAVED_UUID");
        
		// Retrieve the selected document page.
		SBSDKScannedPage page = document.PageAt(0);
		
		// Apply rotation and filters on the page
		// Create the instances of the filters you want to apply.
		var filter1 = new SBSDKScanbotBinarizationFilter(outputMode: SBSDKOutputMode.Antialiased);
		var filter2 = new SBSDKBrightnessFilter(brightness: 0.4);

		page?.ApplyWithRotation(page.Rotation, new SBSDKPolygon(), [filter1, filter2]);
	}
}