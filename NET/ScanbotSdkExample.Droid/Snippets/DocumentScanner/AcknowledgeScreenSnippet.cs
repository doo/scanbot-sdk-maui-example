using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Documentqualityanalyzer;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets.DocumentScanner;

public class AcknowledgementScreenSnippet : AppCompatActivity
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

        // Set the acknowledgment mode
        // Modes:
        // - UnacceptableQuality: The acknowledgment screen will only be shown when the quality of a scanned page is unacceptable.
        //                        The quality threshold is determined by the document quality analyzer parameters.
        // - Always: The acknowledgment screen will always be shown after each snap, regardless of the scanned page's quality.
        // - None: The acknowledgment screen will be disabled, in effect never shown.
        configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.Always;

        // Set the minimum threshold of unacceptable and uncertain qualities.
        configuration.Screens.Camera.DocumentQualityAnalyzerConfiguration.QualityUnacceptableUncertainThreshold = 0.75;

        // Set the background color for the acknowledgment screen.
        configuration.Screens.Camera.Acknowledgement.BackgroundColor = new ScanbotColor("#EFEFEF");

        // You can also configure the buttons in the bottom bar of the acknowledgment screen.
        // e.g. to force the user to retake, if the captured document is not acceptable.
        configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenAcceptableButton.Visible = false;

        // Hide the titles of the buttons.
        configuration.Screens.Camera.Acknowledgement.BottomBar.AcceptWhenAcceptableButton.Title.Visible = false;
        configuration.Screens.Camera.Acknowledgement.BottomBar.ProceedAnywayButton.UnacceptableQuality.Title.Visible = false;
        configuration.Screens.Camera.Acknowledgement.BottomBar.RetakeButton.Title.Visible = false;

        // Configure the acknowledgment screen's hint message which is shown if the least acceptable quality is not met.
        configuration.Screens.Camera.Acknowledgement.UnacceptableQualityWarning.Visible = true;

        // Launch the scanner here.
        // Start the Document Scanner activity.
        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanDocumentRequestCode);
    }
}