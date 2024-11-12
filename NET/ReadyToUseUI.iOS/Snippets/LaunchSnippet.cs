using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class LaunchSnippet : UIViewController
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

		// Present the recognizer view controller modal on this view controller.
		SBSDKUI2DocumentScannerController.PresentOn(this, configuration, (document) =>
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