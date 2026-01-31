using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Creditcard.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CreditCardScanner;

public class PaletteSnippet : AppCompatActivity
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
        
        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;

        // Configure the colors.
        // The palette already has the default colors set, so you don't have to always set all the colors.
        palette.SbColorPrimary = new ScanbotColor("#C8193C");
        palette.SbColorPrimaryDisabled = new ScanbotColor("#F5F5F5");
        palette.SbColorNegative = new ScanbotColor("#FF3737");
        palette.SbColorPositive = new ScanbotColor("#4EFFB4");
        palette.SbColorWarning = new ScanbotColor("#FFCE5C");
        palette.SbColorSecondary = new ScanbotColor("#FFEDEE");
        palette.SbColorSecondaryDisabled = new ScanbotColor("#F5F5F5");
        palette.SbColorOnPrimary = new ScanbotColor("#FFFFFF");
        palette.SbColorOnSecondary = new ScanbotColor("#C8193C");
        palette.SbColorSurface = new ScanbotColor("#FFFFFF");
        palette.SbColorOutline = new ScanbotColor("#EFEFEF");
        palette.SbColorOnSurfaceVariant = new ScanbotColor("#707070");
        palette.SbColorOnSurface = new ScanbotColor("#000000");
        palette.SbColorSurfaceLow = new ScanbotColor("#26000000");
        palette.SbColorSurfaceHigh = new ScanbotColor("#7A000000");
        palette.SbColorModalOverlay = new ScanbotColor("#A3000000");
		
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