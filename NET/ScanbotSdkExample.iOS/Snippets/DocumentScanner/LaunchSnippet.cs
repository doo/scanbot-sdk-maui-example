using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

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