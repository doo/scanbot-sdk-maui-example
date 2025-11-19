using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class LaunchSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

        // Present the view controller modally.
        var scannedOutput = await ScanbotSdkMain.MrzScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed mrz class.
        var mrz = new MRZ(scannedOutput.Result.MrzDocument);

        // Retrieve the values.
        // e.g
        Console.WriteLine($"Birth Date: {mrz.BirthDate.Value.Text}, Nationality: {mrz.Nationality.Value.Text}");
    }
}