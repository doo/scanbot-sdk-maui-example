using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class LocalizationSnippet : UIViewController
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

		// Retrieve the instance of the localization from the configuration object.
		var localization = configuration.Localization;

		// Configure the strings.
		localization.CameraTopBarTitle = "document.camera.title";
		localization.ReviewScreenSubmitButtonTitle = "review.submit.title";
		localization.CameraUserGuidanceNoDocumentFound = "camera.userGuidance.noDocumentFound";
		localization.CameraUserGuidanceTooDark = "camera.userGuidance.tooDark";

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