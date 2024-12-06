using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ReadyToUseUI.Droid.Snippets;

public class PaletteSnippet : AppCompatActivity
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

		// Configure the colors.
		// The palette already has the default colors set, so you don't have to always set all the colors.
		configuration.Palette.SbColorPrimary = new ScanbotColor("#C8193C");
		configuration.Palette.SbColorPrimaryDisabled = new ScanbotColor("#F5F5F5");
		configuration.Palette.SbColorNegative = new ScanbotColor("#FF3737");
		configuration.Palette.SbColorPositive = new ScanbotColor("#4EFFB4");
		configuration.Palette.SbColorWarning = new ScanbotColor("#FFCE5C");
		configuration.Palette.SbColorSecondary = new ScanbotColor("#FFEDEE");
		configuration.Palette.SbColorSecondaryDisabled = new ScanbotColor("#F5F5F5");
		configuration.Palette.SbColorOnPrimary = new ScanbotColor("#FFFFFF");
		configuration.Palette.SbColorOnSecondary = new ScanbotColor("#C8193C");
		configuration.Palette.SbColorSurface = new ScanbotColor("#FFFFFF");
		configuration.Palette.SbColorOutline = new ScanbotColor("#EFEFEF");
		configuration.Palette.SbColorOnSurfaceVariant = new ScanbotColor("#707070");
		configuration.Palette.SbColorOnSurface = new ScanbotColor("#000000");
		configuration.Palette.SbColorSurfaceLow = new ScanbotColor("#26000000");
		configuration.Palette.SbColorSurfaceHigh = new ScanbotColor("#7A000000");
		configuration.Palette.SbColorModalOverlay = new ScanbotColor("#A3000000");
		
		// Launch the scanner here .. 
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