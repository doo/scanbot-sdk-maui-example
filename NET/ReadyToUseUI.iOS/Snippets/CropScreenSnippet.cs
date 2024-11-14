using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class CropScreenSnippet : UIViewController
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

		// Retrieve the instance of the crop configuration from the main configuration object.
		var cropScreenConfiguration = configuration.Screens.Cropping;

		// e.g disable the rotation feature.
		cropScreenConfiguration.BottomBar.RotateButton.Visible = false;

		// e.g. configure various colors.
		configuration.Appearance.TopBarBackgroundColor = new SBSDKUI2Color("#C8193C");
		cropScreenConfiguration.TopBarConfirmButton.Foreground.Color = new SBSDKUI2Color(uiColor: UIColor.White);

		// e.g. customize a UI element's text
		configuration.Localization.CroppingTopBarCancelButtonTitle = "Cancel";

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