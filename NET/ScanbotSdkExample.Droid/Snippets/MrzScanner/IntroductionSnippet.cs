using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Documentdata.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Mrz;
using IO.Scanbot.Sdk.Ui_v2.Mrz.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.MrzScanner;

public class IntroductionSnippet : AppCompatActivity
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

        // Retrieve the instance of the intro screen from the configuration object.
        var introScreen = configuration.IntroScreen;
        
        // Show the introduction screen automatically when the screen appears.
        introScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        introScreen.BackgroundColor = new ScanbotColor("#FFFFFF");

        // Configure the title for the intro screen.
        introScreen.Title.Text = "How to scan an MRZ";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        introScreen.Image = new MrzIntroNoImage();
        
        // For a custom image...
        introScreen.Image = new MrzIntroCustomImage("PathToImage");
        // Or use the default image.
        introScreen.Image = new MrzIntroDefaultImage();

        // Configure the color of the handler on top.
        introScreen.HandlerColor = new ScanbotColor("#EFEFEF");

        // Configure the color of the divider.
        introScreen.DividerColor = new ScanbotColor("#EFEFEF");

        // Configure the text.
        introScreen.Explanation.Color = new ScanbotColor("#000000");
        introScreen.Explanation.Text = "The Machine Readable Zone (MRZ) is a special code on your ID document (such as a passport or ID card) that contains your personal information in a machine-readable format.\n\nTo scan it, simply hold your camera over the document, so that it aligns with the MRZ section. Once scanned, the data will be automatically processed, and you will be directed to the results screen.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        introScreen.DoneButton.Text = "Start Scanning";
        introScreen.DoneButton.Background.FillColor = new ScanbotColor("#C8193C");

        // Launch the scanner.
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
                ToastLength.Long)?.Show();
    }
}