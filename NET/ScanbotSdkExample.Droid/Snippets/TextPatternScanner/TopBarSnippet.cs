using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.TextPatternScanner;

public class TopBarSnippet : AppCompatActivity
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

        // Configure top bar
        var topBar = configuration.TopBar;
        topBar.Mode = TopBarMode.Gradient;
        topBar.BackgroundColor = new ScanbotColor("#C8193C");
        topBar.StatusBarMode = StatusBarMode.Light;

        // Configure cancel button
        topBar.CancelButton.Text = "Cancel";
        topBar.CancelButton.Foreground.Color = new ScanbotColor("#FFFFFF");

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