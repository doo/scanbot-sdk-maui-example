using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.CheckDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CheckScanner;

public class PaletteSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CheckScannerScreenConfiguration();
        
        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;
        
        // Configure the colors.
        // The palette already has the default colors set, so you don't have to always set all the colors.
        palette.SbColorPrimary = new ColorValue("#C8193C");
        palette.SbColorPrimaryDisabled = new ColorValue( "#F5F5F5");
        palette.SbColorNegative = new ColorValue( "#FF3737");
        palette.SbColorPositive = new ColorValue( "#4EFFB4");
        palette.SbColorWarning = new ColorValue( "#FFCE5C");
        palette.SbColorSecondary = new ColorValue( "#FFEDEE");
        palette.SbColorSecondaryDisabled = new ColorValue( "#F5F5F5");
        palette.SbColorOnPrimary = new ColorValue( "#FFFFFF");
        palette.SbColorOnSecondary = new ColorValue( "#C8193C");
        palette.SbColorSurface = new ColorValue( "#FFFFFF");
        palette.SbColorOutline = new ColorValue( "#EFEFEF");
        palette.SbColorOnSurfaceVariant = new ColorValue( "#707070");
        palette.SbColorOnSurface = new ColorValue( "#000000");
        palette.SbColorSurfaceLow = new ColorValue( "#26000000");
        palette.SbColorSurfaceHigh = new ColorValue( "#7A000000");

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.CheckScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed check.
        var check = new USACheck(scannedOutput.Result.Check);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Account number: {check.AccountNumber.Value.Text}");
        Console.WriteLine($"Transit Number: {check.TransitNumber.Value.Text}");
        Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
    } 
}