using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Documentdata;
using IO.Scanbot.Sdk.Ui_v2.Documentdataextractor.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.DocumentDataExtractor;

public class UserGuidanceSnippet : AppCompatActivity
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
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
        // Configure user guidance's
        
        // Top user guidance
        var topUserGuidance = configuration.TopUserGuidance;
        
        // Show the user guidance.
        topUserGuidance.Visible = true;
        
        // Configure the title.
        topUserGuidance.Title.Text = "Scan your Identity Document";
        topUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        
        // Configure the background.
        topUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");
        
        // Finder overlay user guidance
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        // Show the user guidance.
        scanStatusUserGuidance.Visibility = true;
        
        // Configure the title.
        scanStatusUserGuidance.Title.Text = "How to scan an ID document";
        scanStatusUserGuidance.Title.Color = new ScanbotColor("#FFFFFF");
        
        // Configure the background.
        scanStatusUserGuidance.Background.FillColor = new ScanbotColor("#7A000000");

        // Launch the scanner
        var intent = DocumentDataExtractorActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanDocumentDataExtractorRequestCode);
    }

    public override void StartActivityForResult(Intent intent, int requestCode, Bundle options)
    {
        base.StartActivityForResult(intent, requestCode, options);
        var resultEntity = (DocumentDataExtractorUiResult)intent.GetParcelableExtra(RtuConstants.ExtraKeyRtuResult);
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