using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Creditcard.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CreditCardScanner;

public class IntroductionSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanCreditCardRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
		
        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchCreditCardScanner();
        }
    }
    private void LaunchCreditCardScanner()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();

        // Retrieve the instance of the intro screen from the configuration object.
        var introScreen = configuration.IntroScreen;
        introScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        introScreen.BackgroundColor = new ScanbotColor("#FFFFFF");

        // Configure the title for the intro screen.
        introScreen.Title.Text = "How to scan a Credit Card";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        introScreen.Image = new CreditCardNoImage();
        // For a custom image...
        introScreen.Image = new CreditCardIntroCustomImage("PathToImage");
        // Or you can also use our default image.
        introScreen.Image = new CreditCardIntroOneSideImage();

        // Configure the color of the handler on top.
        introScreen.HandlerColor = new ScanbotColor("#EFEFEF");

        // Configure the color of the divider.
        introScreen.DividerColor = new ScanbotColor("#EFEFEF");

        // Configure the text.
        introScreen.Explanation.Color = new ScanbotColor("#000000");
        introScreen.Explanation.Text =
            "To quickly and securely input your credit card details, please hold your device over the credit card, so that the camera aligns with the numbers on the front of the card.\n\n" +
            "The scanner will guide you to the optimal scanning position. Once the scan is complete, your card details will automatically be extracted and processed.\n\n" +
            "Press 'Start Scanning' to begin.";

        // Configure the done button.
        introScreen.DoneButton.Text = "Start Scanning";
        introScreen.DoneButton.Background.FillColor = new ScanbotColor("#C8193C");

        // Launch the scanner.
        var intent = CreditCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanCreditCardRequestCode);
    }


    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (CreditCardScannerUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.CreditCard == null)
        {
            return;
        }

        var creditCard = new CreditCard(resultEntity.CreditCard);
        var cardNumber = creditCard.CardNumber.Value.Text;
        var cardholderName = creditCard.CardholderName.Value.Text;
        var expiryDate = creditCard.ExpiryDate.Value.Text;
        Toast.MakeText(
            this,
            $"Card Number: {cardNumber}, Cardholder Name: {cardholderName}, Expiry Date: {expiryDate}",
            ToastLength.Long
        )?.Show();
    }
}