using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class PalleteSnippet : UIViewController
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

		// Retrieve the instance of the palette from the configuration object.
		var palette = configuration.Palette;

		// Configure the colors.
		// The palette already has the default colors set, so you don't have to always set all the colors.
		palette.SbColorPrimary = new SBSDKUI2Color(colorString: "#C8193C");
		palette.SbColorPrimaryDisabled = new SBSDKUI2Color(colorString: "#F5F5F5");
		palette.SbColorNegative = new SBSDKUI2Color(colorString: "#FF3737");
		palette.SbColorPositive = new SBSDKUI2Color(colorString: "#4EFFB4");
		palette.SbColorWarning = new SBSDKUI2Color(colorString: "#FFCE5C");
		palette.SbColorSecondary = new SBSDKUI2Color(colorString: "#FFEDEE");
		palette.SbColorSecondaryDisabled = new SBSDKUI2Color(colorString: "#F5F5F5");
		palette.SbColorOnPrimary = new SBSDKUI2Color(colorString: "#FFFFFF");
		palette.SbColorOnSecondary = new SBSDKUI2Color(colorString: "#C8193C");
		palette.SbColorSurface = new SBSDKUI2Color(colorString: "#FFFFFF");
		palette.SbColorOutline = new SBSDKUI2Color(colorString: "#EFEFEF");
		palette.SbColorOnSurfaceVariant = new SBSDKUI2Color(colorString: "#707070");
		palette.SbColorOnSurface = new SBSDKUI2Color(colorString: "#000000");
		palette.SbColorSurfaceLow = new SBSDKUI2Color(colorString: "#26000000");
		palette.SbColorSurfaceHigh = new SBSDKUI2Color(colorString: "#7A000000");
		palette.SbColorModalOverlay = new SBSDKUI2Color(colorString: "#A3000000");

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