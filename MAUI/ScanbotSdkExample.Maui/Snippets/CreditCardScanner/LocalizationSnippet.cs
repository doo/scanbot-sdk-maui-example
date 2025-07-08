using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class LocalizationSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";
        
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