using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Vin;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.View.Base;
using Double = Java.Lang.Double;

namespace ScanbotSdkExample.Droid.Snippets.VinScanner;

public class ScanningSnippet : AppCompatActivity
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

        // Configure camera properties
        configuration.CameraConfiguration.FlashEnabled = false;
        configuration.CameraConfiguration.PinchToZoomEnabled = true;
        configuration.CameraConfiguration.ZoomSteps = [Double.ValueOf(1.0), Double.ValueOf(2.0), Double.ValueOf(5.0)]; // To Java.Double types

        // Configure UI elements
        configuration.TopBarOpenIntroScreenButton.Visible = true;
        configuration.TopBarOpenIntroScreenButton.Color = new ScanbotColor("#FFFFFF");

        // Cancel button
        var cancelButton = configuration.TopBar.CancelButton;
        cancelButton.Visible = true;
        cancelButton.Text = "Cancel";
        cancelButton.Foreground.Color = new ScanbotColor("#FFFFFF");
        cancelButton.Background.FillColor = new ScanbotColor("#00000000");

        // Configure success overlay
        configuration.SuccessOverlay.IconColor = new ScanbotColor("#FFFFFF");
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.Message.Color = new ScanbotColor("#FFFFFF");

        // Configure sound
        configuration.Sound.SuccessBeepEnabled = true;
        configuration.Sound.SoundType = SoundType.ModernBeep;

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