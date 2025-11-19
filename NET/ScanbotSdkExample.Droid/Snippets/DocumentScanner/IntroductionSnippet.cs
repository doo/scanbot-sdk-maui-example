using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets;

public class IntroductionSnippet : AppCompatActivity
{
	private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
	private const int ScanDocumentRequestCode = 001;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Returns the singleton instance of the Sdk.
		_scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);

		if (_scanbotSdk.LicenseInfo.IsValid)
		{
			LaunchDocumentScanner();
		}
	}

	private void LaunchDocumentScanner()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		configuration.Screens.Camera.Introduction.ShowAutomatically = true;
		
		// Create a new introduction item.
		var firstExampleEntry = new IntroListEntry();

		// Configure the introduction image to be shown.
		firstExampleEntry.Image = IntroImage.ReceiptsIntroImage();

		// Configure the text.
		firstExampleEntry.Text = new StyledText(true, "Some text explaining how to scan a receipt", new ScanbotColor("#000000"), false);
		
		// Create a second introduction item.
		var secondExampleEntry = new IntroListEntry();

		// Configure the introduction image to be shown.
		secondExampleEntry.Image = IntroImage.CheckIntroImage();

		// Configure the text.
		secondExampleEntry.Text =  new StyledText(true, "Some text explaining how to scan a check", new ScanbotColor("#000000"), false);

		// Set the items into the configuration.
		configuration.Screens.Camera.Introduction.Items = new [] {firstExampleEntry, secondExampleEntry };
		
		// Set a screen title.
		configuration.Screens.Camera.Introduction.Title = new StyledText(true,"Introduction",new ScanbotColor("#000000"), false);

		// Start the Document Scanner activity.
		var intent = DocumentScannerActivity.NewIntent(this, configuration);
		StartActivityForResult(intent, ScanDocumentRequestCode);
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		base.OnActivityResult(requestCode, resultCode, data);

		// Check if the result was cancelled
		if (resultCode != Result.Ok)
		{
			return;
		}

		// Indicates that the cancel button was tapped.
		if (requestCode == ScanDocumentRequestCode)
		{
			// Handle the document result ("documentUuid").
			var documentUuid = data?.GetStringExtra(ActivityConstants.ExtraKeyRtuResult);
		}
	}
}