using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Creditcard.Entity;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.Ui_v2.Creditcard;
using IO.Scanbot.Sdk.Ui_v2.Creditcard.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Document;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;

namespace ScanbotSdkExample.Droid.Snippets.DocumentEnhancer;

public class DocumentCleanUp : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
    private const int DocumentCleanUpdRequestCode = 001;
    private Document _scannedDocument;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);

        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            LaunchDocumentCleanUp(_scannedDocument.Uuid, _scannedDocument.Pages.First().Uuid);
        }
    }

    private void LaunchDocumentCleanUp(string documentUuid, string pageUuid)
    {
        // Create the default configuration object. You may access the IScannedDocument to get the unique identifiers
        var configuration = DocumentCleanupActivityConfiguration.Init(documentUuid, pageUuid);

        // If true, the cleanup tool will not allow erasing text. But it takes some time to process OCR on the image initially,
        configuration.Cleanup.EngineConfiguration.KeepText = false;

        // The maximum number of undo/redo operations that can be performed. Make it less to save up memory
        configuration.Cleanup.EngineConfiguration.MaxUndoRedoStackSize = 4;

        // Downscales stroke area to this value in pixels (width x height) to speed up the cleanup process. The smaller the value the faster but quality will be lower too.
        configuration.Cleanup.EngineConfiguration.MaxCleanupResolution = 1_200_000;

        // Customize the top bar.
        configuration.Cleanup.TopBarBackButton.Text = "Cancel";
        configuration.Cleanup.TopBarConfirmButton.Text = "Done";
        configuration.Cleanup.TopBarTitle.Text = "Clean up the page";

        // background color
        configuration.Cleanup.BackgroundColor = new ScanbotColor("#222222");

        // Configure the toolbar buttons (undo / redo / reset).
        configuration.Cleanup.Toolbar.UndoButton.Title.Text = "Undo";
        configuration.Cleanup.Toolbar.RedoButton.Title.Text = "Redo";
        configuration.Cleanup.Toolbar.ResetButton.Title.Text = "Reset";

        configuration.Cleanup.Toolbar.StrokeSizeSlider.Visible = true;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.MinStrokeSize = 1;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.MaxStrokeSize = 50;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.Title.Text = "Brush size";

        // Optional: show an introduction screen the first time the user opens cleanup.
        configuration.Cleanup.Introduction.ShowAutomatically = true;

        // Customize the alert dialogs shown for Reset and for cancelling with unsaved changes.
        configuration.Cleanup.ResetAllEditsAlertDialog.Title.Text = "Reset all edits?";
        configuration.Cleanup.ResetAllEditsAlertDialog.Subtitle.Text = "This will revert all cleanup operations on this page.";

        configuration.Cleanup.DiscardChangesAlertDialog.Title.Text = "Discard changes?";
        configuration.Cleanup.DiscardChangesAlertDialog.Subtitle.Text = "Your cleanup edits on this page will be lost.";

        // Launch the scanner
        var intent = DocumentCleanupActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, DocumentCleanUpdRequestCode);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
    {
        base.OnActivityResult(requestCode, resultCode, intent);

        // Check if the result was canceled
        if (resultCode != Result.Ok)
        {
            return;
        }

        // Indicates that the cancel button was tapped.
        if (requestCode != DocumentCleanUpdRequestCode)
        {
            return;
        }
        
        var resultEntity = (DocumentCleanupUiResult)intent?.GetParcelableExtra(ActivityConstants.ExtraKeyRtuResult);
        if (resultEntity?.DocumentUuid != null)
        {
            // Create Document object from the Unique document identifier
        }
    }
}