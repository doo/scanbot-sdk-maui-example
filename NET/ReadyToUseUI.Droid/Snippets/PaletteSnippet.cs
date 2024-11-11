using Google.Android.Material.Tabs.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ReadyToUseUI.Droid.Snippets;

public class PaletteSnippet : AppCompatActivity
{
	private const int ScanDocumentRequestCode = 001;
	
	private void StartScanning()
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
}