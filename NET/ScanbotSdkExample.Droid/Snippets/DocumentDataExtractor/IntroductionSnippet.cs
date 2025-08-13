using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Documentdata;
using IO.Scanbot.Sdk.Ui_v2.Documentdataextractor.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;

namespace ScanbotSdkExample.Droid.Snippets.DocumentDataExtractor;

public class IntroductionSnippet : AppCompatActivity
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

        // Retrieve the instance of the intro screen from the configuration object.
        var introScreen = configuration.IntroScreen;
        introScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        introScreen.BackgroundColor = new ScanbotColor("#FFFFFF");

        // Configure the title for the intro screen.
        introScreen.Title.Text = "How to scan a DE Id Card";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        introScreen.Image = new DocumentDataIntroNoImage();
        // For a custom image...
        introScreen.Image = new DocumentDataIntroCustomImage("PathToImage");
        // Or you can also use our default image.
        introScreen.Image = new DocumentDataIntroDefaultImage();

        // Configure the color of the handler on top.
        introScreen.HandlerColor = new ScanbotColor("#EFEFEF");

        // Configure the color of the divider.
        introScreen.DividerColor = new ScanbotColor("#EFEFEF");

        // Configure the text.
        introScreen.Explanation.Color = new ScanbotColor("#000000");
        introScreen.Explanation.Text =
            "To scan your ID, position the document within the viewfinder, ensuring it is properly aligned and all key details are clearly visible. The scanner will automatically extract essential information, such as your name, date of birth, and document number. Once the scan is complete, the scanner will close, and the extracted data will be processed accordingly.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        introScreen.DoneButton.Text = "Start Scanning";
        introScreen.DoneButton.Background.FillColor = new ScanbotColor("#C8193C");

        // Launch the scanner.
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