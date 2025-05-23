using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets;

public class MultiPageSnippet : UIViewController
{
	public override void ViewDidLoad()
	{
		if (ScanbotSDKGlobal.IsLicenseValid)
		{
			LaunchDocumentScanner();
		}
	}

	private void LaunchDocumentScanner()
	{
		// Create the default configuration object.
		var configuration = new SBSDKUI2DocumentScanningFlow();

		// Set the page limit to 0, to disable the limit, or set it to the number of pages you want to scan.
		configuration.OutputSettings.PagesScanLimit = 0;

		// Disable the acknowledgment screen.
		configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = SBSDKUI2AcknowledgementMode.None;

		// Present the recognizer view controller modal on this view controller.
		SBSDKUI2DocumentScannerController.PresentOn(this, configuration,
			(document) =>
		    {
			    // Completion handler to process the result.
			    if (document != null)
			    {
				    // Handle the document.
			    }
			    else
			    {
				    // Indicates that the cancel button was tapped.
			    }
		    });
	}
}