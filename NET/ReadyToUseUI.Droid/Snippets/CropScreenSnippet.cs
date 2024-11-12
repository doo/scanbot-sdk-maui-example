using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ReadyToUseUI.Droid.Snippets;

public class CropScreenSnippet : AppCompatActivity
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

		// MARK: Set the limit for the number of pages you want to scan.
		configuration.OutputSettings.PagesScanLimit = 30;

		// Pass the DOCUMENT_UUID here to resume an old session, or pass null to start a new session or to resume a draft session.
		configuration.DocumentUuid = null;

		// Controls whether to resume an existing draft session or start a new one when DOCUMENT_UUID is null.
		configuration.CleanScanningSession = true;

		// MARK: Configure the bottom bar and the bottom bar buttons.
		// Set the background color of the bottom bar.
		configuration.Appearance.BottomBarBackgroundColor = new ScanbotColor("#C8193C");
		
		// e.g. configure .
		configuration.Appearance.TopBarBackgroundColor = new ScanbotColor( Color.Red);
		
		// Retrieve the camera screen configuration.
		// e.g. customize a UI element's text
		configuration.Localization.CroppingTopBarCancelButtonTitle = "Cancel";

		// e.g disable the rotation feature.
		configuration.Screens.Cropping.BottomBar.RotateButton.Visible = false;

		configuration.Screens.Cropping.TopBarConfirmButton.Foreground.Color = new ScanbotColor(Color.White);

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
			var documentUuid = data?.GetStringExtra(IO.Scanbot.Sdk.Ui_v2.Common.Activity.ActivityConstants.ExtraKeyRtuResult);
		}
	}
}