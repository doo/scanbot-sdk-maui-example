using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ReadyToUseUI.Droid.Snippets;

public class ReorderScreenSnippet : AppCompatActivity
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

		// Retrieve the instance of the reorder pages configuration from the main configuration object.
		// Hide the guidance view.
		configuration.Screens.ReorderPages.Guidance.Visible = false;

		// Set the title for the reorder screen.
		configuration.Screens.ReorderPages.TopBarTitle.Text = "Reorder Pages Screen";

		// Set the title for the guidance.
		configuration.Screens.ReorderPages.Guidance.Title.Text = "Reorder";

		// Set the color for the page number text.
		configuration.Screens.ReorderPages.PageTextStyle.Color = new ScanbotColor(Android.Graphics.Color.Black);

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
			var documentUuid = data?.GetStringExtra(IO.Scanbot.Sdk.Ui_v2.Common.Activity.ActivityConstants
								.ExtraKeyRtuResult);
		}
	}
}