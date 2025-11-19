using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Creditcard.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.CreditCardScanner;

public class LaunchSnippet : AppCompatActivity
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