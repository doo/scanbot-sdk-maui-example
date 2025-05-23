using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets;

public class SinglePageWithFinderSnippet : UIViewController
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
		
		// Set the visibility of the view finder.
		configuration.Screens.Camera.ViewFinder.Visible = true;
        
		// Create the instance of the style, either `SBSDKUI2FinderCorneredStyle` or `SBSDKUI2FinderStrokedStyle`.
		var style = new SBSDKUI2FinderCorneredStyle(strokeColor: new SBSDKUI2Color(colorString: "#FFFFFFFF"),
													strokeWidth: 3.0,
													cornerRadius: 10.0);
		
		// Set the configured style.
		configuration.Screens.Camera.ViewFinder.Style = style;
        
		// Set the desired aspect ratio of the view finder.
		configuration.Screens.Camera.ViewFinder.AspectRatio = new SBSDKAspectRatio(width: 4.0, height: 5.0);
        
		// Set the overlay color.
		configuration.Screens.Camera.ViewFinder.OverlayColor = new SBSDKUI2Color(colorString: "#26000000");
        
		// Set the page limit.
		configuration.OutputSettings.PagesScanLimit = 1;
        
		// Enable the tutorial screen.
		configuration.Screens.Camera.Introduction.ShowAutomatically = true;
        
		// Disable the acknowledgment screen.
		configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = SBSDKUI2AcknowledgementMode.None;
        
		// Disable the review screen.
		configuration.Screens.Review.Enabled = false;

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