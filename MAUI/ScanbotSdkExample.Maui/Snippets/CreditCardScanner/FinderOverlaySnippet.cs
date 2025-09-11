using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class FinderOverlaySnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        
        // Set the example overlay visibility.
        configuration.ExampleOverlayVisible = true;

        // Configure the view finder.
        // Set the style for the view finder.
        // Choose between cornered or stroked style.
        // For default stroked style.
        configuration.ViewFinder.Style = new FinderStrokedStyle();
        // For default cornered style.
        configuration.ViewFinder.Style = new FinderCorneredStyle();
        // You can also set each style's stroke width, stroke color or corner radius.
        // e.g
        configuration.ViewFinder.Style = new FinderStrokedStyle
        {
            StrokeColor = new ColorValue("#7A000000"),
            CornerRadius = 3.0f,
            StrokeWidth = 2.0f
        };

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.CreditCardScanner.LaunchAsync(configuration);
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