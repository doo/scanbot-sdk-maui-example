using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Check.Entity;
using IO.Scanbot.Sdk.Ui_v2.Check;
using IO.Scanbot.Sdk.Ui_v2.Check.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CheckScanner;

public class UserGuidanceSnippet : AppCompatActivity
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
        // Create the default configuration object.
        var configuration = new CheckScannerScreenConfiguration();
        
        // Configure user guidance's
        
        // Top user guidance
        var topUserGuidance = configuration.TopUserGuidance;
        
        // Show the user guidance.
        topUserGuidance.Visible = true;
        
        // Configure the title.
        topUserGuidance.Title.Text = "Scan your Check";
        topUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        
        // Configure the background.
        topUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");
        
        // Finder overlay user guidance
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        // Show the user guidance.
        scanStatusUserGuidance.Visibility = true;
        
        // Configure the title.
        scanStatusUserGuidance.Title.Text = "Scan check";
        scanStatusUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        
        // Configure the background.
        scanStatusUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");

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