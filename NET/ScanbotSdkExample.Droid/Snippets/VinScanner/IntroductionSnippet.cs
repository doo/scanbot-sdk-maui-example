using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Vin;
using IO.Scanbot.Sdk.Ui_v2.Vin.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.VinScanner;

public class IntroductionSnippet : AppCompatActivity
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

        // Retrieve the instance of the intro screen from the configuration object.
        var introScreen = configuration.IntroScreen;
        introScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        introScreen.BackgroundColor = new ScanbotColor("#FFFFFF");

        // Configure the title for the intro screen.
        introScreen.Title.Text = "How to scan a VIN";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        introScreen.Image = new VinIntroNoImage();
        // For a custom image...
        introScreen.Image = new VinIntroCustomImage("PathToImage");
        // Or you can also use our default image.
        introScreen.Image = new VinIntroDefaultImage();

        // Configure the color of the handler on top.
        introScreen.HandlerColor = new ScanbotColor("#EFEFEF");

        // Configure the color of the divider.
        introScreen.DividerColor = new ScanbotColor("#EFEFEF");

        // Configure the text.
        introScreen.Explanation.Color = new ScanbotColor("#000000");
        introScreen.Explanation.Text =
            "The VIN (Vehicle Identification Number) is a unique code you'll find on your windshield or inside the driver's door.\n\nTo read the VIN, hold your camera over it. Make sure it's aligned in the frame. Your VIN will be automatically extracted.\n\nTap 'Start Scanning' to begin.";

        // Configure the done button.
        introScreen.DoneButton.Text = "Start Scanning";
        introScreen.DoneButton.Background.FillColor = new ScanbotColor("#C8193C");

        // Launch the scanner.
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