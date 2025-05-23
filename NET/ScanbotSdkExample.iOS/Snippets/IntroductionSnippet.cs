using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets;

public class IntroductionSnippet : UIViewController
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

		// Retrieve the instance of the introduction configuration from the main configuration object.
		var introductionConfiguration = configuration.Screens.Camera.Introduction;

		// Show the introduction screen automatically when the screen appears.
		introductionConfiguration.ShowAutomatically = true;

		// Create a new introduction item.
		var firstExampleEntry = new SBSDKUI2IntroListEntry();

		// Configure the introduction image to be shown.
		firstExampleEntry.Image = SBSDKUI2DocumentIntroImage.ReceiptsIntroImage;

		// Configure the text.
		firstExampleEntry.Text = new SBSDKUI2StyledText(visible: true,
							text: "Some text explaining how to scan a receipt",
							color: new SBSDKUI2Color(colorString: "#000000"), useShadow: false);

		// Create a second introduction item.
		var secondExampleEntry = new SBSDKUI2IntroListEntry();

		// Configure the introduction image to be shown.
		secondExampleEntry.Image = SBSDKUI2DocumentIntroImage.CheckIntroImage;

		// Configure the text.
		secondExampleEntry.Text = new SBSDKUI2StyledText(visible: true,
							text: "Some text explaining how to scan a check",
							color: new SBSDKUI2Color(colorString: "#000000"), useShadow: false);

		// Set the items into the configuration.
		introductionConfiguration.Items = [firstExampleEntry, secondExampleEntry];

		// Set a screen title.
		introductionConfiguration.Title = new SBSDKUI2StyledText(visible: true, "Introduction",
							new SBSDKUI2Color(colorString: "#000000"), true);

		// Apply the introduction configuration.
		configuration.Screens.Camera.Introduction = introductionConfiguration;

		// Present the recognizer view controller modal on this view controller.
		SBSDKUI2DocumentScannerController.PresentOn(this, configuration,
			(document) =>
		    {
			    // Compvarion handler to process the result.
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