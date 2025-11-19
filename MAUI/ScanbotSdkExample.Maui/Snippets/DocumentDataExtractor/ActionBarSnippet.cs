using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentData;
using ScanbotSDK.MAUI.DocumentsModel;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class ActionBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
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
        var scannedOutput = await ScanbotSdkMain.DocumentDataExtractor.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wraps the resulted document to the strongly typed DeIdCardFront class.
        if (scannedOutput.Result.Document.Type.Name == DocumentsModelRootType.DeIdCardFront.DocumentType.Name)
        {
            var idCardFront = new DeIdCardFront(scannedOutput.Result.Document);
            PrintDeIdCardFront(idCardFront);
            return;
        }
        
        // Iterate through all the document fields
        foreach (var field in scannedOutput.Result.Document.Fields)
        {
            Console.WriteLine($"{field.Type.Name}: {field.Value.Text}");
        }
    }

    private static void PrintDeIdCardFront(DeIdCardFront deIdCardFront)
    {
        Console.WriteLine($"Card access number: {deIdCardFront.CardAccessNumber.Value.Text}");
        Console.WriteLine($"Surname: {deIdCardFront.Surname.Value.Text}");
        Console.WriteLine($"Nationality: {deIdCardFront.Nationality.Value.Text}");
        Console.WriteLine($"Birthdate: {deIdCardFront.BirthDate.Value.Text}");
        Console.WriteLine($"Birth place: {deIdCardFront.Birthplace.Value.Text}");
        Console.WriteLine($"Confidence: {deIdCardFront.ID.ConfidenceWeight}");
        Console.WriteLine($"Maiden name: {deIdCardFront.MaidenName?.Value.Text}");
        Console.WriteLine($"Series: {deIdCardFront.Series.Value.Text}");
    }
}