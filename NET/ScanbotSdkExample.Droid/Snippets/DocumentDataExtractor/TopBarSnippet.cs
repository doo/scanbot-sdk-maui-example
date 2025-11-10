using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Documentdata;
using IO.Scanbot.Sdk.Ui_v2.Documentdata.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.DocumentDataExtractor;

public class TopBarSnippet : AppCompatActivity
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

        // Configure top bar
        var topBar = configuration.TopBar;
        topBar.Mode = TopBarMode.Gradient;
        topBar.BackgroundColor = new ScanbotColor("#C8193C");
        topBar.StatusBarMode = StatusBarMode.Light;

        // Configure cancel button
        topBar.CancelButton.Text = "Cancel";
        topBar.CancelButton.Foreground.Color = new ScanbotColor("#FFFFFF");

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