using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Check.Entity;
using IO.Scanbot.Sdk.Ui_v2.Check;
using IO.Scanbot.Sdk.Ui_v2.Check.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CheckScanner;

public class TopBarSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanCheckRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
		
        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchCheckScanner();
        }
    }
    private void LaunchCheckScanner()
    {
        var configuration = new CheckScannerScreenConfiguration();

        // Configure top bar
        var topBar = configuration.TopBar;
        topBar.Mode = TopBarMode.Gradient;
        topBar.BackgroundColor = new ScanbotColor("#C8193C");
        topBar.StatusBarMode = StatusBarMode.Light;

        // Configure cancel button
        topBar.CancelButton.Text = "Cancel";
        topBar.CancelButton.Foreground.Color = new ScanbotColor("#FFFFFF");

        // Launch the scanner    
        var intent = CheckScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanCheckRequestCode);
    }

    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (CheckScannerUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.Check == null)
        {
            return;
        }

        // Wrap the resulted generic document to the strongly typed check.
        var check = new USACheck(resultEntity.Check);
           
        // Retrieve the values.
        Console.WriteLine($"Account number: {check.AccountNumber?.Value?.Text}");
        Console.WriteLine($"Transit Number: {check.TransitNumber?.Value?.Text}");
        Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
    }
}