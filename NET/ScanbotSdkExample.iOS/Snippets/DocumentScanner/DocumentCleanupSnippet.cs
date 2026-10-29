using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class DocumentCleanupSnippet : UIViewController
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

        // Retrieve the instance of the document cleanup configuration from the main configuration object.
        var cleanupScreenConfiguration = configuration.Screens.Cleanup;

        // e.g. enable/disable the buttons. They are by default ON
        cleanupScreenConfiguration.Toolbar.RedoButton.Visible = true;
        cleanupScreenConfiguration.Toolbar.UndoButton.Visible = true;

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new SBSDKUI2Color("#C8193C");
        cleanupScreenConfiguration.TopBarConfirmButton.Foreground.Color = new SBSDKUI2Color("#FFFFFF");;

        // e.g. customize a UI element's text
        configuration.Localization.DocumentCleanupTopBarCancelButtonTitle = "Cancel";

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