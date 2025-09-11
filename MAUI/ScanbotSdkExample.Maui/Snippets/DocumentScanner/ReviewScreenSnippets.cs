using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class ReviewScreenSnippet
{
    private static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the review configuration from the main configuration object.
        var reviewScreenConfiguration = configuration.Screens.Review;

        // Enable / Disable the review screen.
        reviewScreenConfiguration.Enabled = true;

        // Hide the zoom button.
        reviewScreenConfiguration.ZoomButton.Visible = false;

        // Hide the add button.
        reviewScreenConfiguration.BottomBar.AddButton.Visible = false;

        // Retrieve the instance of the reorder pages configuration from the main configuration object.
        var reorderScreenConfiguration = configuration.Screens.ReorderPages;

        // Hide the guidance view.
        reorderScreenConfiguration.Guidance.Visible = false;

        // Set the title for the reorder screen.
        reorderScreenConfiguration.TopBarTitle.Text = "Reorder Pages Screen";

        // Retrieve the instance of the cropping configuration from the main configuration object.
        var croppingScreenConfiguration = configuration.Screens.Cropping;

        // Hide the reset button.
        croppingScreenConfiguration.BottomBar.ResetButton.Visible = false;

        // Retrieve the retake button configuration from the main configuration object.
        var retakeButtonConfiguration = configuration.Screens.Review.BottomBar.RetakeButton;

        // Show the retake button.
        retakeButtonConfiguration.Visible = true;

        // Configure the retake title color.
        retakeButtonConfiguration.Title.Color = Microsoft.Maui.Graphics.Colors.White;

        // Apply the retake configuration button to the review bottom bar configuration.
        configuration.Screens.Review.BottomBar.RetakeButton = retakeButtonConfiguration;

        // Apply the configurations.
        configuration.Screens.Review = reviewScreenConfiguration;
        configuration.Screens.ReorderPages = reorderScreenConfiguration;
        configuration.Screens.Cropping = croppingScreenConfiguration;

        // Launch the scanner
        var response = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (response.Status != OperationResult.Ok)
        {
            // Indicates that the cancel button was tapped.
            return;
        }
        
        // Handle the document.
        var scannerDocument = response.Result;
    }
}