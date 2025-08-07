using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.CheckDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CheckScanner;

public class LaunchSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CheckScannerScreenConfiguration();

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.CheckScanner.LaunchAsync(configuration);
        if (result.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed check.
        var check = new USACheck(result.Result.Check);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Account number: {check.AccountNumber.Value.Text}");
        Console.WriteLine($"Transit Number: {check.TransitNumber.Value.Text}");
        Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
    } 
}