using System.Diagnostics;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public interface IClassicDocumentScannerViewResult
{
    void DidCompleteDocumentScanning(SBSDKScannedDocument scannedDocument);
}

internal class ScanningStatus(UIColor color, string message)
{
    public UIColor Color { get; set; } = color;
    public string Message { get; set; } = message;
}

public class ClassicDocumentScannerViewController : UIViewController
{   
    private UIView _bottomButtonsContainer, _scanningContainerView;
    private UIButton _flashButton, _autoSnapButton;
    private SBSDKShutterButton _shutterButton;
    private SBSDKDocumentScannerViewController _documentScannerViewController;
    private bool _autoSnappingEnabled = false;

    internal IClassicDocumentScannerViewResult ResultDelegate;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        Title = "Document Classic UI";

        // Create a view as container for bottom buttons:
        _bottomButtonsContainer = InitBottomBarView();

        // Create a view as container to embed the Scanbot SDK SBSDKDocumentScannerViewController:
        _scanningContainerView = new UIView
        {
            BackgroundColor = UIColor.Red,
            TranslatesAutoresizingMaskIntoConstraints = false
        };
        
        View.AddSubviews(_scanningContainerView, _bottomButtonsContainer);
        View.TranslatesAutoresizingMaskIntoConstraints = true;
        
        var bottomBarHeight = View.Frame.Size.Height * 0.15f;
        bottomBarHeight = bottomBarHeight > 100 ? 100 : bottomBarHeight;
        
        NSLayoutConstraint.ActivateConstraints([
            _scanningContainerView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
            _scanningContainerView.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor),
            _scanningContainerView.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor),
            _scanningContainerView.BottomAnchor.ConstraintEqualTo(_bottomButtonsContainer.TopAnchor),
            
            _bottomButtonsContainer.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor),
            _bottomButtonsContainer.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor),
            _bottomButtonsContainer.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor),
            _bottomButtonsContainer.HeightAnchor.ConstraintEqualTo(bottomBarHeight),
        ]);

        InitDocumentScannerViewController();
    }

    private UIView InitBottomBarView()
    {
        var bottomBarView = new UIView();
        bottomBarView.TranslatesAutoresizingMaskIntoConstraints = false;
        
        // Auto Snap button, To enable auto snapping
        _autoSnapButton = GetBottomButton("AutoSnap");
        _autoSnapButton.AddTarget(delegate
        {
            _autoSnappingEnabled = !_autoSnappingEnabled;
            SetAutoSnapEnabled(_autoSnappingEnabled);
        }, UIControlEvent.TouchUpInside);
        
        // Flash button
        _flashButton = GetBottomButton("Flash");
        _flashButton.AddTarget(delegate
        {
            _documentScannerViewController.IsFlashLightEnabled = !_documentScannerViewController.IsFlashLightEnabled;
            _flashButton.Selected = _documentScannerViewController.IsFlashLightEnabled;
        }, UIControlEvent.TouchUpInside);
        
        // Shutter Button
       _shutterButton = new SBSDKShutterButton
        {
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _shutterButton.AddTarget(delegate
        {
            // Captures the document Image. The Delegate method is invoked immediately after the document is captured.
            _documentScannerViewController.CaptureDocumentImage();
        }, UIControlEvent.TouchUpInside);
        
        
        bottomBarView.AddSubviews(_autoSnapButton, _shutterButton, _flashButton);
        
        NSLayoutConstraint.ActivateConstraints([
            
            // Auto-Snap button (on the left)
            _autoSnapButton.LeadingAnchor.ConstraintEqualTo(bottomBarView.LeadingAnchor, 12),
            _autoSnapButton.CenterYAnchor.ConstraintEqualTo(bottomBarView.CenterYAnchor),
            _autoSnapButton.HeightAnchor.ConstraintEqualTo(50),
            
            // Shutter button (on the center)
            _shutterButton.CenterXAnchor.ConstraintEqualTo(bottomBarView.CenterXAnchor),
            _shutterButton.CenterYAnchor.ConstraintEqualTo(bottomBarView.CenterYAnchor),
            _shutterButton.WidthAnchor.ConstraintEqualTo(90),
            _shutterButton.HeightAnchor.ConstraintEqualTo(90),

            // Flash button (on the right)
            _flashButton.CenterYAnchor.ConstraintEqualTo(bottomBarView.CenterYAnchor),
            _flashButton.TrailingAnchor.ConstraintEqualTo(bottomBarView.TrailingAnchor, -12),
            _flashButton.HeightAnchor.ConstraintEqualTo(50),
        ]);

        return bottomBarView;
    }

    private UIButton GetBottomButton(string buttonText)
    {
        var button = new UIButton
        {
            TranslatesAutoresizingMaskIntoConstraints = false
        };
        button.SetTitle($"{buttonText} ON", UIControlState.Selected);
        button.SetTitle($"{buttonText} OFF", UIControlState.Normal);
        button.TitleLabel.Font = UIFont.SystemFontOfSize(14f);
        return button;
    }

    private void InitDocumentScannerViewController()
    {
        // Initialize and attach the DocumentScannerViewController to a View.
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

        // Sensitivity factor for automatic capturing. Must be in the range [0.0...1.0]. Invalid values are treated as 1.0.
        // Defaults to 0.66 (1 sec).s A value of 1.0 triggers automatic capturing immediately, a value of 0.0 delays the automatic by 3 seconds.
        _documentScannerViewController.AutoSnappingSensitivity = 0.7f;

        // Set the updated configurations
        _documentScannerViewController.SetConfiguration(defaultConfigurations);
        
        // hide the default snapping button
        _documentScannerViewController.HideSnapButton = true;
        
      SetAutoSnapEnabled(true);

      _documentScannerViewController.ConfigureScanningStatusLabel += ConfigureUserGuidanceLabel;
      _documentScannerViewController.PolygonConfigurationFor += TestPolygonConfiguration;
    }

    private SBSDKDocumentScannerPolygonConfiguration TestPolygonConfiguration(SBSDKDocumentScannerViewController controller, SBSDKDocumentDetectionStatus status)
    {
        var scanningInfo = StatusDictionary[status];

        controller.AutoSnapProgressPolygonConfiguration.StrokeColor = scanningInfo.Color;
        return controller.AutoSnapProgressPolygonConfiguration;
    } 
    
    internal Dictionary<SBSDKDocumentDetectionStatus, ScanningStatus> StatusDictionary = new Dictionary<SBSDKDocumentDetectionStatus, ScanningStatus>
    {
        { SBSDKDocumentDetectionStatus.Ok,                  new ScanningStatus( UIColor.Black, "The document is Ok") },
        { SBSDKDocumentDetectionStatus.OkButTooSmall,        new ScanningStatus( UIColor.White, "Please move the camera closer to the document.") },
        { SBSDKDocumentDetectionStatus.OkButBadAngles,       new ScanningStatus( UIColor.White, "Please hold the camera in parallel over the document.") },
        { SBSDKDocumentDetectionStatus.OkButBadAspectRatio,  new ScanningStatus( UIColor.White, "The document size is too long.") },

        { SBSDKDocumentDetectionStatus.ErrorNothingDetected, new ScanningStatus( UIColor.Purple, "Unable to detect the document.") },
        { SBSDKDocumentDetectionStatus.OkButTooDark,         new ScanningStatus( UIColor.Purple, "Unable to detect due to dark lighting conditions.") },
        { SBSDKDocumentDetectionStatus.ErrorTooNoisy,        new ScanningStatus( UIColor.Purple, "Unable to detect document due to too much noise in the preview.") },
        { SBSDKDocumentDetectionStatus.NotAcquired,          new ScanningStatus( UIColor.Purple, "Unable to acquire the document.") },
        { SBSDKDocumentDetectionStatus.OkButOrientationMismatch,new ScanningStatus( UIColor.Purple, "Please check the orientation of the document.") },
    };
    
    private void ConfigureUserGuidanceLabel(object sender, ConfigureScanningStatusLabelForScanningResultEventArgs e)
    {
        if (e?.Result == null)
        {
            return;
        }
        Debug.WriteLine("Document Detection Status: " + e.Result.Status);
        
        var scanningInfo = StatusDictionary[e.Result.Status];
        e.Label.Text = scanningInfo.Message;
        e.Label.BackgroundColor = scanningInfo.Color.ColorWithAlpha(0.5f);
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
    
    private void SetAutoSnapEnabled(bool enabled)
    {
        // Set the button selection
        _autoSnapButton.Selected = enabled;

        // Set the AutoSnapping behaviour
        _documentScannerViewController.AutoSnappingMode = enabled ? SBSDKAutoSnappingMode.Enabled : SBSDKAutoSnappingMode.Disabled;
        
        // Suppress the User guidance label when you have to show custom implementation.
        _documentScannerViewController.SuppressDetectionStatusLabel = !enabled;
        _documentScannerViewController.SnapButton.ScannerStatus = enabled ? SBSDKScannerStatus.Scanning : SBSDKScannerStatus.Idle;
        
        // Suppress the default Polygon layer when you have to customize the polygon.
        _documentScannerViewController.SuppressPolygonLayer = !enabled;

        // Get the AutoSnapProgressPolygonConfiguration to customize and reset it.
        var autoSnapConfig = _documentScannerViewController.AutoSnapProgressPolygonConfiguration;
        autoSnapConfig.Enabled = enabled;
        autoSnapConfig.StrokeColor = UIColor.DarkGray;
        _documentScannerViewController.AutoSnapProgressPolygonConfiguration = autoSnapConfig;
        
        
        // Enables when the document is not accepted(may be the camera is too far or the document is too small) to be scanned. (Auto snapping doesn't start)
        // _documentScannerViewController.PolygonConfigurationRejected.StrokeColor = UIColor.Gray;
        
        // Enables when the document is accepted to be scanned. (Auto snapping kicks in here)
        // _documentScannerViewController.PolygonConfigurationAccepted.StrokeColor = UIColor.Yellow;
    }
}