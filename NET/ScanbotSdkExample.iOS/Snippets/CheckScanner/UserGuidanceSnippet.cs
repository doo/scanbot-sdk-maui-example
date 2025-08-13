using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CheckScanner;

class UserGuidanceSnippet : UIViewController
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

        // Configure user guidances

        // Top user guidance
        var topUserGuidance = configuration.TopUserGuidance;
        topUserGuidance.Visible = true;
        topUserGuidance.Title.Text = "Scan your Identity Document";
        topUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        topUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

        // Scan status user guidance
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        scanStatusUserGuidance.Visibility = true;
        scanStatusUserGuidance.Title.Text = "Scan check";
        scanStatusUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        scanStatusUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

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
