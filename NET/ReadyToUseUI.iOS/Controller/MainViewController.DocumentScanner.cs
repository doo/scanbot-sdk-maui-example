using ReadyToUseUI.iOS.Repository;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
    private UIColor AppAccentColor = UIColor.FromRGBA(200, 23, 60, 1);
    
    private void SingleDocumentScanning()
    {
      // Initialize document scanner configuration object using default configurations
      var configuration = new SBSDKUI2DocumentScanningFlow();
        
        // Disable the multiple page behavior
        configuration.OutputSettings.PagesScanLimit = 1;
        
        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = false;
        
        // Enable/Disable Auto Snapping behavior
        configuration.Screens.Camera.CameraConfiguration.AutoSnappingEnabled = true;
        
        // Configure the animation
        // You can choose between genie animation or checkmark animation
        // Note: Both modes can be further configured to your liking
        
        // e.G for genie animation
        configuration.Screens.Camera.CaptureFeedback.SnapFeedbackMode = new SBSDKUI2PageSnapFunnelAnimation();
        // or for checkmark animation
        configuration.Screens.Camera.CaptureFeedback.SnapFeedbackMode = new SBSDKUI2PageSnapCheckMarkAnimation();
        
        // Hide the auto snapping enable/disable button
        configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Visible = false;
        configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Visible = false;
        configuration.Screens.Camera.BottomBar.ImportButton.Title.Visible = true;
        configuration.Screens.Camera.BottomBar.TorchOnButton.Title.Visible = true;
        configuration.Screens.Camera.BottomBar.TorchOffButton.Title.Visible = true;
        
        // Set colors
        configuration.Palette.SbColorPrimary = new SBSDKUI2Color(uiColor: AppAccentColor);
        configuration.Palette.SbColorOnPrimary = new SBSDKUI2Color(uiColor: UIColor.White);
        
        // Configure the hint texts for different scenarios
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";
        
        // Create the default configuration object.
        var controller = SBSDKUI2DocumentScannerController.CreateWithConfiguration(configuration, DidCompleteDocumentScanning);
        PresentViewController(controller, true, null);
    }

    private void MultipleDocumentScanning()
    {
         // Initialize document scanner configuration object using default configurations
         var configuration = new SBSDKUI2DocumentScanningFlow();
        
        // Enable the multiple page behavior
        configuration.OutputSettings.PagesScanLimit = 0;
        
        // Enable/Disable Auto Snapping behavior
        configuration.Screens.Camera.CameraConfiguration.AutoSnappingEnabled = true;
        
        // Hide/Unhide the auto snapping enable/disable button
        configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Visible = true;
        configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Visible = true;
        
        // Set colors
        configuration.Palette.SbColorPrimary = new SBSDKUI2Color(uiColor: AppAccentColor);
        configuration.Palette.SbColorOnPrimary = new SBSDKUI2Color(uiColor: UIColor.White);
        
        // Configure the hint texts for different scenarios
        // e.G
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";
        
        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = true;
        
        // Configure bottom bar (further properties like title, icon and  background can also be set for these buttons)
        configuration.Screens.Review.BottomBar.AddButton.Visible = true;
        configuration.Screens.Review.BottomBar.RetakeButton.Visible = true;
        configuration.Screens.Review.BottomBar.CropButton.Visible = true;
        configuration.Screens.Review.BottomBar.RotateButton.Visible = true;
        configuration.Screens.Review.BottomBar.DeleteButton.Visible = true;
        
        // Configure `more` popup on review screen
        // e.G
        configuration.Screens.Review.MorePopup.ReorderPages.Icon.Visible = true;
        configuration.Screens.Review.MorePopup.DeleteAll.Icon.Visible = true;
        configuration.Screens.Review.MorePopup.DeleteAll.Title.Text = "Delete all pages";
        
        // Configure reorder pages screen
        // e.G
        configuration.Screens.ReorderPages.TopBarTitle.Text = "Reorder Pages";
        configuration.Screens.ReorderPages.Guidance.Title.Text = "Reorder Pages";
        
        // Configure cropping screen
        // e.G
        configuration.Screens.Cropping.TopBarTitle.Text = "Cropping Screen";
        configuration.Screens.Cropping.BottomBar.ResetButton.Visible = true;
        configuration.Screens.Cropping.BottomBar.RotateButton.Visible = true;
        configuration.Screens.Cropping.BottomBar.DetectButton.Visible = true;
        
        // Create the default configuration object.
        var controller = SBSDKUI2DocumentScannerController.CreateWithConfiguration(configuration, DidCompleteDocumentScanning);
        PresentViewController(controller, true, null);
    }

    private void SingleFinderDocumentScanning()
    {
        // Initialize document scanner configuration object using default configurations
        var configuration = new SBSDKUI2DocumentScanningFlow();
        
        // Disable the multiple page behavior
        configuration.OutputSettings.PagesScanLimit = 1;
        
        // Enable view finder
        configuration.Screens.Camera.ViewFinder.Visible = true;
        configuration.Screens.Camera.ViewFinder.AspectRatio = new SBSDKUI2AspectRatio(width: 3, height: 4);
        
        // Enable/Disable the review screen.
        configuration.Screens.Review.Enabled = false;
        
        // Enable/Disable Auto Snapping behavior
        configuration.Screens.Camera.CameraConfiguration.AutoSnappingEnabled = true;
        
        // Hide the auto snapping enable/disable button
        configuration.Screens.Camera.BottomBar.AutoSnappingModeButton.Visible = false;
        configuration.Screens.Camera.BottomBar.ManualSnappingModeButton.Visible = false;
        
        // Set colors
        configuration.Palette.SbColorPrimary = new SBSDKUI2Color(uiColor: AppAccentColor);
        configuration.Palette.SbColorOnPrimary = new SBSDKUI2Color(uiColor: UIColor.White);
        
        // Configure the hint texts for different scenarios
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooDark = "Need more lighting to detect a document";
        configuration.Screens.Camera.UserGuidance.StatesTitles.TooSmall = "Document too small";
        configuration.Screens.Camera.UserGuidance.StatesTitles.NoDocumentFound = "Could not detect a document";
        
        // Create the default configuration object.
        var controller = SBSDKUI2DocumentScannerController.CreateWithConfiguration(configuration, DidCompleteDocumentScanning);
        PresentViewController(controller, true, null);
    }

    private void DidCompleteDocumentScanning(SBSDKScannedDocument scannedDocument)
    {
        if (scannedDocument?.Pages == null && scannedDocument.Pages.Length == 0)
        {
            return;
        }

        for (int i = 0; i < scannedDocument.Pages.Length; ++i)
        {
            var page = scannedDocument.PageAt(i);
            PageRepository.Add(page);
        }

        OpenImageListController();
    }
    

    private async void ImportImage()
    {
        var image = await ImagePicker.Instance.PickImageAsync();
        
        // Create an instance of a detector
        var detector = new SBSDKDocumentDetector();
            
        // Run detection on the image
        var result = detector.DetectPhotoPolygonOnImage(image, CoreGraphics.CGRect.Empty, smoothingEnabled: false);
        
        Console.WriteLine("Attempted document detection on imported page: " + result.Status);
        OpenImageListController();
    }

    private void OpenImageListController()
    {
        var controller = new ImageListController();
        NavigationController.PushViewController(controller, true);
    }
}