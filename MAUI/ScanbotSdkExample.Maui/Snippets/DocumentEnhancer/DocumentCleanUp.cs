using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Snippets.DocumentEnhancer;

public class DocumentCleanUp
{
    public static async void StartDocumentCleanupAsync(string documentUuid, string pageUuid)
    {
        var configuration = new DocumentCleanupStandaloneConfiguration
        {
            DocumentUuid = documentUuid,
            PageUuid = pageUuid
        };

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
        configuration.Cleanup.BackgroundColor = new ColorValue("#222222");

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

        var result = await ScanbotSDKMain.DocumentEnhancer.StartDocumentCleanupScreenAsync(configuration);

        if (result.IsCanceled) return;

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
        }
        
        // access the IScannedDocument Result object here.
    }
}