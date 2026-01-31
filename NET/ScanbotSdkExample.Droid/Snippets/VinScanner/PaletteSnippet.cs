using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Vin;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.VinScanner;

public class PaletteSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanVinRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
		
        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchVinScanner();
        }
    }
    private void LaunchVinScanner()
    {
        // Create the default configuration object.
        var configuration = new VinScannerScreenConfiguration();
        
        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;

        // Configure the colors.
        // The palette already has the default colors set, so you don't have to always set all the colors.
        palette.SbColorPrimary = new ScanbotColor("#C8193C");
        palette.SbColorPrimaryDisabled = new ScanbotColor("#F5F5F5");
        palette.SbColorNegative = new ScanbotColor("#FF3737");
        palette.SbColorPositive = new ScanbotColor("#4EFFB4");
        palette.SbColorWarning = new ScanbotColor("#FFCE5C");
        palette.SbColorSecondary = new ScanbotColor("#FFEDEE");
        palette.SbColorSecondaryDisabled = new ScanbotColor("#F5F5F5");
        palette.SbColorOnPrimary = new ScanbotColor("#FFFFFF");
        palette.SbColorOnSecondary = new ScanbotColor("#C8193C");
        palette.SbColorSurface = new ScanbotColor("#FFFFFF");
        palette.SbColorOutline = new ScanbotColor("#EFEFEF");
        palette.SbColorOnSurfaceVariant = new ScanbotColor("#707070");
        palette.SbColorOnSurface = new ScanbotColor("#000000");
        palette.SbColorSurfaceLow = new ScanbotColor("#26000000");
        palette.SbColorSurfaceHigh = new ScanbotColor("#7A000000");
        palette.SbColorModalOverlay = new ScanbotColor("#A3000000");
		
        var intent = VinScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanVinRequestCode);
    }
    
    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (VinScannerUiResult)intent.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        if (resultEntity?.TextResult == null)
        {
            return;
        }
       
        // Retrieve the values.
        Console.WriteLine($"Vin Scanner result: {resultEntity.TextResult?.RawText}");
    }
}