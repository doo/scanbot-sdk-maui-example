using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentDataExtractor;

class LaunchSnippet: UIViewController {
    
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