using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Vin;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.VinScanner;

public class UserGuidanceSnippet : AppCompatActivity
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
        
        // Configure user guidance's
        
        // Top user guidance
        var topUserGuidance = configuration.TopUserGuidance;
        
        // Show the user guidance.
        topUserGuidance.Visible = true;
        
        // Configure the title.
        topUserGuidance.Title.Text = "Scan your VIN";
        topUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        
        // Configure the background.
        topUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");
        
        // Finder overlay user guidance
        var finderViewUserGuidance = configuration.FinderViewUserGuidance;
        // Show the user guidance.
        finderViewUserGuidance.Visible = true;
        
        // Configure the title.
        finderViewUserGuidance.Title.Text = "Scan the Vin";
        finderViewUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        
        // Configure the background.
        finderViewUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");

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