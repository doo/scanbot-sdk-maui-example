using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public interface IClassicDocumentScannerViewResult
{
    void DidCompleteDocumentScanning(SBSDKScannedDocument scannedDocument);
}

public class ClassicDocumentScannerViewController : UIViewController
{
    private UIView _bottomButtonsContainer, _scanningContainerView;
    private UIButton _flashButton, _autoSnapButton;
    private SBSDKDocumentScannerViewController _documentScannerViewController;
    private bool _autoSnappingEnabled = false;

    internal IClassicDocumentScannerViewResult ResultDelegate;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var screenSize = UIScreen.MainScreen.Bounds.Size;

        // Create a view as container for bottom buttons:
        var buttonsContainerHeight = 120;
        _bottomButtonsContainer = new UIView(new CGRect(0, screenSize.Height - buttonsContainerHeight, screenSize.Width, buttonsContainerHeight));
        _bottomButtonsContainer.BackgroundColor = UIColor.Blue;
        View!.AddSubview(_bottomButtonsContainer);

        // Create a view as container to embed the Scanbot SDK SBSDKDocumentScannerViewController:
        _scanningContainerView = new UIView(new CGRect(0, 0, screenSize.Width, screenSize.Height - buttonsContainerHeight));
        View.AddSubview(_scanningContainerView);

        _documentScannerViewController = new SBSDKDocumentScannerViewController();
        this.AttachViewControllerInView(_documentScannerViewController, _scanningContainerView);
        _documentScannerViewController.DidSnapDocumentImage += DidDetectDocument;

        // =================================================================
        // Please see the API docs of our native Scanbot SDK for iOS, since all those methods and properties
        // are also available as Scanbot .NET bindings.
        // =================================================================

        // Get the default configuration
        var defaultConfigurations = _documentScannerViewController.CopyCurrentConfiguration();

        // We want unscaled images in full size:
        _documentScannerViewController.ImageScale = 1.0f;

        // The minimum score in percent (0 - 100) of the perspective distortion to accept a detected document. 
        // Default is 75.0. Set lower values to accept more perspective distortion. Warning: Lower values result in more blurred document images.
        defaultConfigurations.Parameters.AcceptedAngleScore = 75;

        // The minimum size in percent (0 - 100) of the screen size to accept a detected document. It is sufficient that height or width match the score. 
        // Default is 80.0. Warning: Lower values result in low resolution document images.
        defaultConfigurations.Parameters.AcceptedSizeScore = 80;

        // Sensitivity factor for automatic capturing. Must be in the range [0.0...1.0]. Invalid values are threated as 1.0. 
        // Defaults to 0.66 (1 sec).s A value of 1.0 triggers automatic capturing immediately, a value of 0.0 delays the automatic by 3 seconds.
        _documentScannerViewController.AutoSnappingSensitivity = 0.7f;

        // Set the updated configurations
        _documentScannerViewController.SetConfiguration(defaultConfigurations);

        SetCustomShutterButton();
    }

    private void SetCustomShutterButton()
    {
        // Create the frame.
        var padding = 150;
        var buttonSize = 100;
        var x = View.Center.X - (buttonSize / 2);
        var y = View.Frame.Height - (buttonSize + padding);
        var frame = new CGRect(x, y, buttonSize, buttonSize);
        
        
        // NOTE: Update as per your requirement. If you do not wish to use the Scanbot shutter button.
        var useScanbotShutterButton = true;
        UIButton shutterButton = null;
        
        if (useScanbotShutterButton)
        {
            var customSnapButton = new SBSDKShutterButton(frame);

            customSnapButton.TouchUpInside += (s, e) => { _documentScannerViewController.CaptureDocumentImage(); };

            customSnapButton.ButtonScannedBackgroundColor = UIColor.Red;
            customSnapButton.ButtonScannedColor = UIColor.Orange;
            shutterButton = customSnapButton;
        }
        else
        {
            shutterButton = new UIButton(frame);

            shutterButton.TouchUpInside += (s, e) => { _documentScannerViewController.CaptureDocumentImage(); };

            shutterButton.SetTitle("Capture", UIControlState.Normal);
            shutterButton.BackgroundColor = UIColor.Orange;
            shutterButton.SetTitleColor(UIColor.White, UIControlState.Normal);
        }
        
        _documentScannerViewController.HideSnapButton = false;
        View!.AddSubview(shutterButton);
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
        var customSnapButton = new SBSDKShutterButton(new CGRect(200, 200, 100, 100));
        customSnapButton.TranslatesAutoresizingMaskIntoConstraints = false;
        customSnapButton.BackgroundColor = UIColor.Yellow;
        _documentScannerViewController.HideSnapButton = false;
        _documentScannerViewController.CustomSnapButton = customSnapButton;
    }

    public override void ViewWillAppear(bool animated)
    {
        base.ViewWillAppear(animated);

        AddAutosnapToggleButton();
        SetAutoSnapEnabled(_autoSnappingEnabled);
        AddFlashToggleButton();
        SetupDefaultShutterButtonColors();
    }

    public override bool ShouldAutorotate()
    {
        return true;
    }

    public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
    {
        return UIInterfaceOrientationMask.AllButUpsideDown;
    }

    public override UIStatusBarStyle PreferredStatusBarStyle()
    {
        // White statusbar
        return UIStatusBarStyle.LightContent;
    }

    private void SetupDefaultShutterButtonColors()
    {
        var shutterButton = _documentScannerViewController.SnapButton;
        shutterButton.ButtonSearchingColor = UIColor.Red;
        shutterButton.ButtonScannedColor = UIColor.Green;
    }

    private void AddAutosnapToggleButton()
    {
        _autoSnapButton = new UIButton(new CGRect(40, _bottomButtonsContainer.Frame.Height - 80, 40, 40));
        _autoSnapButton.AddTarget(delegate
        {
            _autoSnappingEnabled = !_autoSnappingEnabled;
            SetAutoSnapEnabled(_autoSnappingEnabled);
        }, UIControlEvent.TouchUpInside);

        _autoSnapButton.SetImage(UIImage.FromBundle("ui_autosnap_off"), UIControlState.Normal);
        _autoSnapButton.SetImage(UIImage.FromBundle("ui_autosnap_on"), UIControlState.Selected);

        _bottomButtonsContainer.AddSubview(_autoSnapButton);
        _bottomButtonsContainer.BringSubviewToFront(_autoSnapButton);
    }

    private void AddFlashToggleButton()
    {
        _flashButton = new UIButton(new CGRect(_bottomButtonsContainer.Frame.Width - 80, _bottomButtonsContainer.Frame.Height - 80, 40, 40));
        _flashButton.AddTarget(delegate
        {
            _documentScannerViewController.IsFlashLightEnabled = !_documentScannerViewController.IsFlashLightEnabled;
            _flashButton.Selected = _documentScannerViewController.IsFlashLightEnabled;
        }, UIControlEvent.TouchUpInside);

        _flashButton.SetImage(UIImage.FromBundle("ui_flash_off"), UIControlState.Normal);
        _flashButton.SetImage(UIImage.FromBundle("ui_flash_on"), UIControlState.Selected);

        _flashButton.Selected = _documentScannerViewController.IsFlashLightEnabled;

        _bottomButtonsContainer.AddSubview(_flashButton);
        _bottomButtonsContainer.BringSubviewToFront(_flashButton);
    }

    private void SetAutoSnapEnabled(bool enabled)
    {
        _autoSnapButton.Selected = enabled;
        _documentScannerViewController.AutoSnappingMode = enabled ? SBSDKAutoSnappingMode.Enabled : SBSDKAutoSnappingMode.Disabled;
        _documentScannerViewController.SuppressDetectionStatusLabel = !enabled;
        _documentScannerViewController.SnapButton.ScannerStatus = enabled ? SBSDKScannerStatus.Scanning : SBSDKScannerStatus.Idle;
    }

    private void DidDetectDocument(object sender, SnapDocumentImageOnImageWithResultEventArgs args)
    {
        if (args?.OriginalImage == null)
            return;
        
        NavigationController?.PopViewController(true);
        
        
        // Creating a SBSDKScannedDocument object from the captured original image.
        
        // Create an instance of a document
        var document = new SBSDKScannedDocument();
        
        // Add page to the document using the image and the detected polygon on the image (if any)
        document.AddPageWith(args.OriginalImage, args.Result?.Polygon ?? new SBSDKPolygon(), []);
        
        // set the result to navigate to the document listing page.
        ResultDelegate?.DidCompleteDocumentScanning(document);
    }
}