using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class LocalizationSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();
        
        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";
        
        // Present the view controller modally.
        var result = await ScanbotSDKMain.Mrz.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }
        
        // Wrap the resulted generic document to the strongly typed mrz class.
        var mrz = new MRZ(result.Value.MrzDocument);
        
        // Retrieve the values.
        // e.g
         Console.WriteLine($"Birth Date: {mrz.BirthDate.Value.Text}, Nationality: {mrz.Nationality.Value.Text}"); 
    } 
}