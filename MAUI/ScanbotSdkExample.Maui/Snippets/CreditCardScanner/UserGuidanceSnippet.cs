using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class UserGuidanceSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        
        // Top user guidance
        // Retrieve the instance of the top user guidance from the configuration object.
        var topUserGuidance = configuration.TopUserGuidance;
        
        // Show the user guidance.
        topUserGuidance.Visible = true;
        
        // Configure the title.
        topUserGuidance.Title.Text = "Scan your Credit Card";
        topUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        
        // Configure the background.
        topUserGuidance.Background.FillColor = new ColorValue("#7A000000");

        // Scan status user guidance
        // Retrieve the instance of the user guidance from the configuration object.
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        scanStatusUserGuidance.Visibility = true;
        
        // Configure the title.
        scanStatusUserGuidance.Title.Text = "Scan credit card";
        scanStatusUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        
        // Configure the background.
        scanStatusUserGuidance.Background.FillColor = new ColorValue("#7A000000");
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.CreditCard.StartScannerAsync(configuration);
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