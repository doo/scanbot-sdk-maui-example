using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentData;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class PaletteSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
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
        var result = await ScanbotSDKMain.DocumentDataExtractor.StartExtractorScreenAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }
        
        // Iterate through all the document fields
        foreach (var field in result.Value.Document.Fields)
        {
            Console.WriteLine($"{field.Type.Name}: {field.Value.Text}");
        }
    } 
}