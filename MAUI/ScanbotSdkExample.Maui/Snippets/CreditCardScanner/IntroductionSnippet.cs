using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class IntroductionSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        
        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new ColorValue("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a credit card";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = new CreditCardNoImage();
        
        // For a custom image...
        configuration.IntroScreen.Image = new CreditCardIntroCustomImage
        {
            Uri = "PathToImage"
        };
        
        // Or you can also use our default one side image.
        configuration.IntroScreen.Image = new CreditCardIntroOneSideImage();
        
        // Or you can also use our default two sides image.
        configuration.IntroScreen.Image = new CreditCardIntroTwoSidesImage();

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new ColorValue("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new ColorValue("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new ColorValue("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To quickly and securely input your credit card details, please hold your device over the credit card, so that the camera aligns with the numbers on the front of the card.\n\n" +
            "The scanner will guide you to the optimal scanning position. Once the scan is complete, your card details will automatically be extracted and processed.\n\n" +
            "Press 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new ColorValue("#C8193C");

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