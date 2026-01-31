using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Creditcard.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;
using Double = Java.Lang.Double;

namespace ScanbotSdkExample.Droid.Snippets.CreditCardScanner;

public class ScanningSnippet : AppCompatActivity
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
        var configuration = new CreditCardScannerScreenConfiguration();

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
        var intent = CreditCardScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanCreditCardRequestCode);
    }


    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (CreditCardScannerUiResult)intent.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
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