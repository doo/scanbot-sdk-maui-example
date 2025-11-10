using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Documentdata;
using IO.Scanbot.Sdk.Ui_v2.Documentdata.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.DocumentDataExtractor;

public class ActionBarSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int ScanDocumentDataExtractorRequestCode = 001;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);

        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchDocumentDataExtractor();
        }
    }

    private void LaunchDocumentDataExtractor()
    {
        var configuration = new DocumentDataExtractorScreenConfiguration();

        // Retrieve and configure the Action Bar
        var actionBar = configuration.ActionBar;

        // Flash button
        actionBar.FlashButton.Visible = true;

        // Configure the inactive state of the flash button.
        actionBar.FlashButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlashButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Configure the active state of the flash button.
        actionBar.FlashButton.ActiveBackgroundColor = new ScanbotColor("#FFCE5C");
        actionBar.FlashButton.ActiveForegroundColor = new ScanbotColor("#000000");

        // Zoom button
        actionBar.ZoomButton.Visible = true;
        actionBar.ZoomButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.ZoomButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Flip camera button
        actionBar.FlipCameraButton.Visible = true;
        actionBar.FlipCameraButton.BackgroundColor = new ScanbotColor("#7A000000");
        actionBar.FlipCameraButton.ForegroundColor = new ScanbotColor("#FFFFFF");

        // Launch the scanner
        var intent = DocumentDataExtractorActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanDocumentDataExtractorRequestCode);
    }

    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (DocumentDataExtractorUiResult)intent.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.Document == null)
        {
            return;
        }

        // Iterate through all the document fields
        foreach (var field in resultEntity.Document.Fields)
        {
            Console.WriteLine($"{field.Type.Name}: {field.Value?.Text}");
        }
    }
}