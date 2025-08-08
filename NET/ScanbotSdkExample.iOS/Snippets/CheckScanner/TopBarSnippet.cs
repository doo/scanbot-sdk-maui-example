using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CheckScanner;

class TopBarSnippet : UIViewController
{
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Start scanning here. Usually this is an action triggered by some button or menu.
        StartScanning();
    }

    void StartScanning()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2CheckScannerScreenConfiguration();

        // Set the top bar mode.
        configuration.TopBar.Mode = SBSDKUI2TopBarMode.Gradient;

        // Set the background color which will be used as a gradient.
        configuration.TopBar.BackgroundColor = new SBSDKUI2Color("#C8193C");

        // Set the status bar mode.
        configuration.TopBar.StatusBarMode = SBSDKUI2StatusBarMode.Light;

        // Configure the cancel button.
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new SBSDKUI2Color("#FFFFFF");

        // Present the view controller modally.
        SBSDKUI2CheckScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }
            
            // Wrap the resulted generic document to the strongly typed check.
            var check = new SBSDKCheckDocumentModelUSACheck(result.Check);
           
            // Retrieve the values.
            Console.WriteLine($"Account number: {check.AccountNumber?.Value?.Text}");
            Console.WriteLine($"Transit Number: {check.TransitNumber?.Value?.Text}");
            Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
        });
    }
}