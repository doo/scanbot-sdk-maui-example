using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Common;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets;

public class SinglePageWithFinderSnippet : AppCompatActivity
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

		// Set the page limit.
		configuration.OutputSettings.PagesScanLimit = 1;

		// Set the visibility of the view finder.
		configuration.Screens.Camera.ViewFinder.Visible = true;

		// Create the instance of the style, either `FinderCorneredStyle` or `FinderStrokedStyle`.
		// Set the configured style.
		var style = FinderStyle.FinderCorneredStyle();
		style.StrokeColor = new ScanbotColor("#FFFFFFFF");
		style.StrokeWidth = 3.0;
		style.CornerRadius = 10.0;
		configuration.Screens.Camera.ViewFinder.Style = style;

		// Set the desired aspect ratio of the view finder.
		configuration.Screens.Camera.ViewFinder.AspectRatio = new AspectRatio(1.0, 1.0);

		// Set the overlay color.
		configuration.Screens.Camera.ViewFinder.OverlayColor = new ScanbotColor("#26000000");

		// Enable the tutorial screen.
		configuration.Screens.Camera.Introduction.ShowAutomatically = true;

		// Disable the acknowledgment screen.
		configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.None;

		// Disable the review screen.
		configuration.Screens.Review.Enabled = false;

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