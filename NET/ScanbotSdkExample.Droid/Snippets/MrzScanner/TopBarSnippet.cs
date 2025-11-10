using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Documentdata.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Mrz;
using IO.Scanbot.Sdk.Ui_v2.Mrz.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.MrzScanner;

public class TopBarSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanMrzRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
		
        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchMrzScanner();
        }
    }
    private void LaunchMrzScanner()
    {
        var configuration = new MrzScannerScreenConfiguration();

        // Configure top bar
        var topBar = configuration.TopBar;
        topBar.Mode = TopBarMode.Gradient;
        topBar.BackgroundColor = new ScanbotColor("#C8193C");
        topBar.StatusBarMode = StatusBarMode.Light;

        // Configure cancel button
        topBar.CancelButton.Text = "Cancel";
        topBar.CancelButton.Foreground.Color = new ScanbotColor("#FFFFFF");

        // Launch the scanner    
        var intent = MrzScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanMrzRequestCode);
    }


    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (MrzScannerUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.MrzDocument == null)
        {
            return;
        }

        var mrz = new MRZ(resultEntity.MrzDocument);
        var givenName = mrz.GivenNames.Value.Text;
        var birthDate = mrz.BirthDate.Value.Text;
        var expiryDate = mrz.ExpiryDate.Value.Text;
        Toast.MakeText(
                this,
                $"Given Name: {givenName}, Birth Date: {birthDate}, Expiry Date: {expiryDate}",
                ToastLength.Long
            )
            ?.Show();
    }
}