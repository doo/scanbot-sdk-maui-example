using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Documentscanner;
using IO.Scanbot.Sdk.Geometry;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets.DocumentScanner;

public class DocumentEnhancer : AppCompatActivity
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

        // Create the parameters as required
        configuration.OutputSettings.StraighteningParameters.StraighteningMode = DocumentStraighteningMode.None;
        configuration.OutputSettings.StraighteningParameters.StraighteningMode = DocumentStraighteningMode.Straighten;

        // The straightening parameters can be customized to fit the expected aspect ratio of the document
        // to be straightened. This can help the straightening algorithm to achieve better results.
        configuration.OutputSettings.StraighteningParameters.AspectRatios =
        [
            new AspectRatio(width: 1, height: 1),
            new AspectRatio(width: 16, height: 9),
            new AspectRatio(width: 3, height: 4)
        ];


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