using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Check.Entity;
using IO.Scanbot.Sdk.Ui_v2.Check;
using IO.Scanbot.Sdk.Ui_v2.Check.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CheckScanner;

public class IntroductionSnippet : AppCompatActivity
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

        // Retrieve the instance of the intro screen from the configuration object.
        var introScreen = configuration.IntroScreen;
        introScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        introScreen.BackgroundColor = new ScanbotColor("#FFFFFF");

        // Configure the title for the intro screen.
        introScreen.Title.Text = "How to scan a check";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        introScreen.Image = new CheckNoImage();
        // For a custom image...
        introScreen.Image = new CheckIntroCustomImage("PathToImage");
        // Or you can also use our default image.
        introScreen.Image = new CheckIntroDefaultImage();

        // Configure the color of the handler on top.
        introScreen.HandlerColor = new ScanbotColor("#EFEFEF");

        // Configure the color of the divider.
        introScreen.DividerColor = new ScanbotColor("#EFEFEF");

        // Configure the text.
        introScreen.Explanation.Color = new ScanbotColor("#000000");
        introScreen.Explanation.Text =
            "To quickly and securely input your check details, please hold your device over the check, so that the camera aligns with the document.\n\nThe scanner will guide you to the optimal scanning position. Once the scan is complete, your card details will automatically be extracted and processed.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        introScreen.DoneButton.Text = "Start Scanning";
        introScreen.DoneButton.Background.FillColor = new ScanbotColor("#C8193C");

        // Launch the scanner.
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