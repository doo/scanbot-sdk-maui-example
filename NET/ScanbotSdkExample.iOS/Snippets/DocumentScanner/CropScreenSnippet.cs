using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

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

        // display presentation error
    }
}