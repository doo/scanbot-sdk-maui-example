using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Documentdata.Entity;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Documentdata;
using IO.Scanbot.Sdk.Ui_v2.Documentdataextractor.Configuration;
using IO.Scanbot.Sdk.UI.View.Base;
using Double = Java.Lang.Double;

namespace ScanbotSdkExample.Droid.Snippets.DocumentDataExtractor;

public class ScanningSnippet : AppCompatActivity
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

        // Configure camera properties
        configuration.CameraConfiguration.FlashEnabled = false;
        configuration.CameraConfiguration.PinchToZoomEnabled = true;
        configuration.CameraConfiguration.ZoomSteps = [Double.ValueOf(1.0), Double.ValueOf(2.0), Double.ValueOf(5.0)]; // To Java.Double types

        // Configure UI elements
        configuration.TopBarOpenIntroScreenButton.Visible = true;
        configuration.TopBarOpenIntroScreenButton.Color = new ScanbotColor("#FFFFFF");

        // Cancel button
        var cancelButton = configuration.TopBar.CancelButton;
        cancelButton.Visible = true;
        cancelButton.Text = "Cancel";
        cancelButton.Foreground.Color = new ScanbotColor("#FFFFFF");
        cancelButton.Background.FillColor = new ScanbotColor("#00000000");

        // Configure success overlay
        configuration.SuccessOverlay.IconColor = new ScanbotColor("#FFFFFF");
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.Message.Color = new ScanbotColor("#FFFFFF");

        // Configure sound
        configuration.Sound.SuccessBeepEnabled = true;
        configuration.Sound.SoundType = SoundType.ModernBeep;

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

        // Wraps the resulted document to the strongly typed DeIdCardFront class.
        if (resultEntity.Document.Type.Name == DeIdCardFront.DocumentType)
        {
            var idCardFront = new DeIdCardFront(resultEntity.Document);
            PrintDeIdCardFront(idCardFront);
            return;
        }

        // Iterate through all the document fields
        foreach (var field in resultEntity.Document.Fields)
        {
            Console.WriteLine($"{field.Type.Name}: {field.Value?.Text}");
        }
    }

    private static void PrintDeIdCardFront(DeIdCardFront deIdCardFront)
    {
        if (deIdCardFront == null) return;

        Console.WriteLine($"Card access number: {deIdCardFront.CardAccessNumber?.Value?.Text}");
        Console.WriteLine($"Surname: {deIdCardFront.Surname?.Value?.Text}");
        Console.WriteLine($"Nationality: {deIdCardFront.Nationality?.Value?.Text}");
        Console.WriteLine($"Birthdate: {deIdCardFront.BirthDate?.Value?.Text}");
        Console.WriteLine($"Birth place: {deIdCardFront.Birthplace?.Value?.Text}");
        Console.WriteLine($"Confidence: {deIdCardFront.Id.Value.Confidence}");
        Console.WriteLine($"Maiden name: {deIdCardFront.MaidenName?.Value?.Text}");
        Console.WriteLine($"Series: {deIdCardFront.Series?.Value?.Text}");
    }
}