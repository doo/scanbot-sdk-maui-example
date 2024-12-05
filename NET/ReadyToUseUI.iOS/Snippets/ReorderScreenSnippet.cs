using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Snippets;

public class ReorderScreenSnippet : UIViewController
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
		
		// Retrieve the instance of the reorder pages configuration from the main configuration object.
		var reorderScreenConfiguration = configuration.Screens.ReorderPages;
        
		// Hide the guidance view.
		reorderScreenConfiguration.Guidance.Visible = false;
        
		// Set the title for the reorder screen.
		reorderScreenConfiguration.TopBarTitle.Text = "Reorder Pages Screen";
        
		// Set the title for the guidance.
		reorderScreenConfiguration.Guidance.Title.Text = "Reorder";
        
		// Set the color for the page number text.
		reorderScreenConfiguration.PageTextStyle.Color = new SBSDKUI2Color(uiColor: UIColor.Black);
        
		// Apply the configurations.
		configuration.Screens.ReorderPages = reorderScreenConfiguration;

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