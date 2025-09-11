using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Vin;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.VinScanner;

public class ActionBarSnippet : AppCompatActivity
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
        var configuration = new VinScannerScreenConfiguration();

        // Retrieve and configure the Action Bar
        var actionBar = configuration.ActionBar;

        // Flash button
        actionBar.FlashButton.Visible = true;
        
        // Configure the inactive state of the flash button.
        actionBar.FlashButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlashButton.ForegroundColor = new ScanbotColor("#FFFFFF");
        
        // Configure the active state of the flash button.
        actionBar.FlashButton.ActiveBackgroundColor = new ScanbotColor("#FFCE5C");
        actionBar.FlashButton.ActiveForegroundColor = new ScanbotColor("#000000");

        // Zoom button
        actionBar.ZoomButton.Visible = true;
        actionBar.ZoomButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.ZoomButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Flip camera button
        actionBar.FlipCameraButton.Visible = true;
        actionBar.FlipCameraButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlipCameraButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Launch the scanner
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