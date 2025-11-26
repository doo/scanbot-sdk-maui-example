using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentData;
using ScanbotSDK.MAUI.DocumentsModel;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class LaunchSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.DocumentDataExtractor.StartScannerAsync(configuration);
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