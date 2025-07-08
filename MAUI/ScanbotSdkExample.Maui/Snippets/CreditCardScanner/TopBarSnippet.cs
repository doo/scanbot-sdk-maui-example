using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class TopBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        
        // Set the top bar mode.
        configuration.TopBar.Mode = TopBarMode.Gradient;

        // Set the background color which will be used as a gradient.
        configuration.TopBar.BackgroundColor = new ColorValue("#C8193C");

        // Set the status bar mode.
        configuration.TopBar.StatusBarMode = StatusBarMode.Light;

        // Configure the cancel button.
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new ColorValue("#FFFFFF");
        
        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.CreditCard.LaunchAsync(configuration);
        if (result?.Result?.CreditCard == null || result.Result.RecognitionStatus != CreditCardScanningStatus.Success)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }
        
        // Wrap the resulted generic document to the strongly typed credit card.
        var creditCard = new CreditCard(result.Result.CreditCard);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Card number: {creditCard.CardNumber.Value.Text}, Cardholder Name: {creditCard.CardholderName?.Value?.Text}");
    } 
}