using ReadyToUseUI.iOS.Repository;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
    
    private void ScanDocument()
    {
        var config = SBSDKUIDocumentScannerConfiguration.DefaultConfiguration;

        config.BehaviorConfiguration.CameraPreviewMode = SBSDKVideoContentMode.FitIn;
        config.BehaviorConfiguration.IgnoreBadAspectRatio = true;
        config.BehaviorConfiguration.IsMultiPageEnabled = true;
        config.TextConfiguration.PageCounterButtonTitle = "%d Page(s)";
        config.TextConfiguration.TextHintOK = "Don't move.\nScanning document...";

        // further configuration properties
        //config.UiConfiguration.BottomBarBackgroundColor = UIColor.Blue;
        //config.UiConfiguration.BottomBarButtonsColor = UIColor.White;
        //config.UiConfiguration.FlashButtonHidden = true;
        // and so on...

        var controller = SBSDKUIDocumentScannerViewController
            .CreateNew(configuration: config, @delegate: null);

        controller.DidFinishWithDocument += OnScanComplete;
        controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        PresentViewController(controller, false, null);
    }

    private void ScanDocumentWithFinder()
    {
        var config = SBSDKUIFinderDocumentScannerConfiguration.DefaultConfiguration;

        config.BehaviorConfiguration.CameraPreviewMode = SBSDKVideoContentMode.FitIn;
        config.BehaviorConfiguration.IgnoreBadAspectRatio = true;
        config.TextConfiguration.TextHintOK = "Don't move.\nScanning document...";
        config.UiConfiguration.OrientationLockMode = SBSDKOrientationLock.Portrait;
        config.UiConfiguration.FinderAspectRatio = new SBSDKAspectRatio(21.0, 29.7); // a4 portrait

        // further configuration properties
        //config.UiConfiguration.FinderLineColor = UIColor.Red;
        //config.UiConfiguration.TopBarBackgroundColor = UIColor.Blue;
        //config.UiConfiguration.FlashButtonHidden = true;
        // and so on...

        var controller = SBSDKUIFinderDocumentScannerViewController
            .CreateNew(configuration: config, @delegate: null);

        controller.DidFinishWithDocument += OnScanComplete;
        controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        PresentViewController(controller, false, null);
    }

    private void OnScanComplete(object _, FinishWithDocumentEventArgs args)
    {
        if (args?.Document?.Pages == null && args.Document.Pages.Length == 0)
        {
            return;
        }

        for (int i = 0; i < args.Document.Pages.Length; ++i)
        {
            var page = args.Document.PageAtIndex(i);
            var result = page.DetectDocumentAndApplyPolygonIfOkay(true);
            PageRepository.Add(page);
        }
        OpenImageListController();
    }

    private async void ImportImage()
    {
        var image = await ImagePicker.Instance.PickImageAsync();
        var page = PageRepository.Add(image, new SBSDKPolygon());
        var result = page.DetectDocumentAndApplyPolygonIfOkay(true);
        Console.WriteLine("Attempted document detection on imported page: " + result.Status);

        OpenImageListController();
    }

    private void OpenImageListController()
    {
        var controller = new ImageListController();
        NavigationController.PushViewController(controller, true);
    }
}