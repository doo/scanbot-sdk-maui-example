using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentDataExtractor;

class IntroductionSnippet : UIViewController
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
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a document";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = SBSDKUI2DocumentDataExtractorIntroImage.DocumentDataIntroNoImage;
        
        // For a custom image...
        configuration.IntroScreen.Image = SBSDKUI2DocumentDataExtractorIntroImage.DocumentDataIntroCustomImageWithUri("PathToImage");
        
        // Or you can also use our default image.
        configuration.IntroScreen.Image = SBSDKUI2DocumentDataExtractorIntroImage.DocumentDataIntroDefaultImage;

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new SBSDKUI2Color("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To quickly and securely scan your document details, please hold your device over the document, so that the camera aligns with all the information on the document.\n\nThe scanner will guide you to the optimal scanning position. Once the scan is complete, your document details will automatically be extracted and processed.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new SBSDKUI2Color("#C8193C");

        // Present the view controller modally.
        SBSDKUI2DocumentDataExtractorViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (result?.Document == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Wraps the resulted document to the strongly typed DeIdCardFront class.
            if (result.Document.Type.Name == SBSDKDocumentsModelRootType.DeIdCardFront.Name)
            {
                var idCardFront = new SBSDKDocumentsModelDeIdCardFront(result.Document);
                PrintDeIdCardFront(idCardFront);
                return;
            }

            // Iterate through all the document fields
            foreach (var field in result.Document.Fields)
            {
                Console.WriteLine($"{field.Type.Name}: {field.Value?.Text}");
            }
        });
    }

    private static void PrintDeIdCardFront(SBSDKDocumentsModelDeIdCardFront deIdCardFront)
    {
        if (deIdCardFront == null) return;
            
        Console.WriteLine($"Card access number: {deIdCardFront.CardAccessNumber?.Value?.Text}");
        Console.WriteLine($"Surname: {deIdCardFront.Surname?.Value?.Text}");
        Console.WriteLine($"Nationality: {deIdCardFront.Nationality?.Value?.Text}");
        Console.WriteLine($"Birthdate: {deIdCardFront.BirthDate?.Value?.Text}");
        Console.WriteLine($"Birth place: {deIdCardFront.Birthplace?.Value?.Text}");
        Console.WriteLine($"Confidence: {deIdCardFront.Id?.ConfidenceWeight}");
        Console.WriteLine($"Maiden name: {deIdCardFront.MaidenName?.Value?.Text}");
        Console.WriteLine($"Series: {deIdCardFront.Series?.Value?.Text}");
    }
}