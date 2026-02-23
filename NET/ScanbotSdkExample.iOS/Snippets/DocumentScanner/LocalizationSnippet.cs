using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

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

		try
		{
			// Launch the scanner view controller
			SBSDKUI2DocumentScannerController.PresentOn(viewController: this, configuration: configuration, error: out var presentationError, completion: DocumentScannerCompletion).GetOrThrow(presentationError);
		}
		catch (Exception e)
		{
			// handle the error thrown from the GetOrThrow(...) function, referenced by the PresentOn(...) error object. 
			Console.WriteLine(e);
		}
	}

	private void DocumentScannerCompletion(SBSDKUI2DocumentScannerController controller, SBSDKScannedDocument document, NSError error)
	{
		// check for error
		if (error != null)
		{
			// display error
			Alert.ValidateAndShowError(error);
			return;
		}

		// Handle the document result.
		var documentId = document.Uuid;
	}
}