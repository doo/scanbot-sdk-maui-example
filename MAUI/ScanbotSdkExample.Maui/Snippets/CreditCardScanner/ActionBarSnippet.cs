using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class ActionBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        
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
        var scannedOutput = await ScanbotSDKMain.Rtu.CreditCard.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed credit card.
        var creditCard = new CreditCard(scannedOutput.Result.CreditCard);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Card number: {creditCard.CardNumber.Value.Text}, Cardholder Name: {creditCard.CardholderName?.Value?.Text}");
    } 
}