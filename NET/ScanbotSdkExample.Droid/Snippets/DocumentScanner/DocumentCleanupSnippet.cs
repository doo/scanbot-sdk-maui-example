using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets;

public class DocumentCleanupSnippet : AppCompatActivity
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

        // Retrieve the instance of the document cleanup configuration from the main configuration object.
        var cleanupScreenConfiguration = configuration.Screens.Cleanup;

        // e.g. enable/disable the buttons. They are by default ON
        cleanupScreenConfiguration.Toolbar.RedoButton.Visible = true;
        cleanupScreenConfiguration.Toolbar.UndoButton.Visible = true;

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new ScanbotColor("#C8193C");
        cleanupScreenConfiguration.TopBarConfirmButton.Foreground.Color = new ScanbotColor("#FFFFFF");;

        // e.g. customize a UI element's text
        configuration.Localization.DocumentCleanupTopBarCancelButtonTitle = "Cancel";

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