using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Vin;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.VinScanner;

public class FinderOverlaySnippet : AppCompatActivity
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

        // Configure finder overlay appearance
        configuration.ViewFinder.Style = new FinderStrokedStyle(new ScanbotColor("#FF00FF"), 2.0, 0.0);

        // Launch the scanner
        var intent = VinScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanVinRequestCode);
    }

    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (VinScannerUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.TextResult == null)
        {
            return;
        }
       
        // Retrieve the values.
        Console.WriteLine($"Vin Scanner result: {resultEntity.TextResult?.RawText}");
    }
}