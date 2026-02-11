using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.VinScanner;

class LaunchSnippet: UIViewController {
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Start scanning here. Usually this is an action triggered by some button or menu.
        StartScanning();
    }

    void StartScanning()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();

        // Present the view controller modally.
        SBSDKUI2VINScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }
            
            // Handle the result
            Console.WriteLine($"Vin Scanner result: {result.TextResult.RawText}");
        });
    }
}