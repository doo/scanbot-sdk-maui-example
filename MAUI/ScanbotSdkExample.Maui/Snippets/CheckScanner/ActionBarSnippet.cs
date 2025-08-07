using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.CheckDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CheckScanner;

public class ActionBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CheckScannerScreenConfiguration();
        
        // Retrieve the instance of the action bar from the configuration object.
        var actionBar = configuration.ActionBar;

        // Show the flash button.
        actionBar.FlashButton.Visible = true;

        // Configure the inactive state of the flash button.
        actionBar.FlashButton.BackgroundColor = new ColorValue("#7A000000");
        actionBar.FlashButton.ForegroundColor = new ColorValue("#FFFFFF");

        // Configure the active state of the flash button.
        actionBar.FlashButton.ActiveBackgroundColor = new ColorValue("#FFCE5C");
        actionBar.FlashButton.ActiveForegroundColor = new ColorValue("#000000");

        // Show the zoom button.
        actionBar.ZoomButton.Visible = true;

        // Configure the zoom button.
        actionBar.ZoomButton.BackgroundColor = new ColorValue("#7A000000");
        actionBar.ZoomButton.ForegroundColor = new ColorValue("#FFFFFF");

        // Show the flip camera button.
        actionBar.FlipCameraButton.Visible = true;

        // Configure the flip camera button.
        actionBar.FlipCameraButton.BackgroundColor = new ColorValue("#7A000000");
        actionBar.FlipCameraButton.ForegroundColor = new ColorValue("#FFFFFF");

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.CheckScanner.LaunchAsync(configuration);
        if (result.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed check.
        var check = new USACheck(result.Result.Check);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Account number: {check.AccountNumber.Value.Text}");
        Console.WriteLine($"Transit Number: {check.TransitNumber.Value.Text}");
        Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
    } 
}