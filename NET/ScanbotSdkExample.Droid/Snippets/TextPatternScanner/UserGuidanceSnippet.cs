using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Textpattern;
using IO.Scanbot.Sdk.Ui_v2.Textpattern.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.TextPatternScanner;

public class UserGuidanceSnippet : AppCompatActivity
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
        
        // Top user guidance
        var topUserGuidance = configuration.TopUserGuidance;
        topUserGuidance.Visible = true;
        topUserGuidance.Title.Text = "Scan your Identity Document";
        topUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        topUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");
        
        // Finder view user guidance
        var finderUserGuidance = configuration.FinderViewUserGuidance;
        finderUserGuidance.Visible = true;
        finderUserGuidance.Title.Text = "DD.MM.YY";
        finderUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        finderUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");

        // Launch the scanner
        var intent = TextPatternScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanTextPatternRequestCode);
    }

    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (TextPatternScannerUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.RawText == null)
        {
            return;
        }

        Toast.MakeText(this, resultEntity.RawText, ToastLength.Long)?.Show();
    }
}