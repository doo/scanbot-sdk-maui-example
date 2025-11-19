using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Creditcard.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CreditCardScanner;

public class ActionBarSnippet : AppCompatActivity
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

        // Retrieve and configure the Action Bar
        var actionBar = configuration.ActionBar;

        // Flash button
        actionBar.FlashButton.Visible = true;
        
        // Configure the inactive state of the flash button.
        actionBar.FlashButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlashButton.ForegroundColor = new ScanbotColor("#FFFFFF");
        
        // Configure the active state of the flash button.
        actionBar.FlashButton.ActiveBackgroundColor = new ScanbotColor("#FFCE5C");
        actionBar.FlashButton.ActiveForegroundColor = new ScanbotColor("#000000");

        // Zoom button
        actionBar.ZoomButton.Visible = true;
        actionBar.ZoomButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.ZoomButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Flip camera button
        actionBar.FlipCameraButton.Visible = true;
        actionBar.FlipCameraButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlipCameraButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Launch the scanner
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