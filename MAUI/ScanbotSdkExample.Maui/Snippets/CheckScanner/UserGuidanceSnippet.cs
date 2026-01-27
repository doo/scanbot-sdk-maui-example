using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.CheckDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CheckScanner;

public class UserGuidanceSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new CheckScannerScreenConfiguration();

        // Top user guidance
        // Retrieve the instance of the top user guidance from the configuration object.
        var topUserGuidance = configuration.TopUserGuidance;
        
        // Show the user guidance.
        topUserGuidance.Visible = true;
        
        // Configure the title.
        topUserGuidance.Title.Text = "Scan your Check";
        topUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        
        // Configure the background.
        topUserGuidance.Background.FillColor = new ColorValue("#7A000000");

        // Scan status user guidance
        // Retrieve the instance of the user guidance from the configuration object.
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        scanStatusUserGuidance.Visibility = true;
        
        // Configure the title.
        scanStatusUserGuidance.Title.Text = "Scan check";
        scanStatusUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        
        // Configure the background.
        scanStatusUserGuidance.Background.FillColor = new ColorValue("#7A000000");
        
        // Present the view controller modally.
        var result = await ScanbotSDKMain.Check.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Wrap the resulted generic document to the strongly typed check.
        var check = new USACheck(result.Value.Check);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Account number: {check.AccountNumber.Value.Text}");
        Console.WriteLine($"Transit Number: {check.TransitNumber.Value.Text}");
        Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
    } 
}