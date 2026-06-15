using ScanbotSDK.iOS;
using ScanbotSdkExample.iOS.Utils;

namespace ScanbotSdkExample.iOS.Snippets.DocumentScanner;

public class DocumentEnhancerSnippet : UIViewController
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

            // Create the parameters as required
            configuration.OutputSettings.StraighteningParameters.StraighteningMode = SBSDKDocumentStraighteningMode.Straighten;

            // The straightening parameters can be customized to fit the expected aspect ratio of the document
            // to be straightened. This can help the straightening algorithm to achieve better results.
            configuration.OutputSettings.StraighteningParameters.AspectRatios =
            [
                  new SBSDKAspectRatio(width: 1, height: 1),
                  new SBSDKAspectRatio(width: 16, height: 9),
                  new SBSDKAspectRatio(width: 3, height: 4)
            ];

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