using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.TextPatternScanner;

public class ActionBarSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanTextPatternRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
		
        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchTextPatternScanner();
        }
    }
    private void LaunchTextPatternScanner()
    {
        var configuration = new TextPatternScannerScreenConfiguration();

        // Retrieve and configure the Action Bar
        var actionBar = configuration.ActionBar;

        // Flash button
        actionBar.FlashButton.Visible = true;
        actionBar.FlashButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlashButton.ForegroundColor = new ScanbotColor("#FFFFFF");
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
        var intent = TextPatternScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanTextPatternRequestCode);
    }
    
    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (TextPatternScannerUiResult)intent.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
        if (resultEntity?.RawText == null)
        {
            return;
        }

        Toast.MakeText(this, resultEntity.RawText, ToastLength.Long)?.Show();
    }
}