using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public class MrzScannerDemoViewController : UIViewController
{
      private UIView _bottomButtonsContainer, _scanningContainerView;
      private UIButton _flashButton, _autoSnapButton;
      private SBSDKMRZScannerViewController _mrzScannerViewController;

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

            // Basic -- I was using forever
            _mrzScannerViewController = new SBSDKMRZScannerViewController();
            this.AttachViewControllerInView(_mrzScannerViewController, _scanningContainerView);
            _mrzScannerViewController.DidScanMRZ += DidScanMrzCode;
            
            // New one -- Works fine
            var viewModel = new SBSDKMRZScannerViewModel(new SBSDKMRZScannerConfiguration(), SBSDKMRZScannerViewModel.DefaultCameraConfigurationWithDevice(SBSDKCameraDevice.DefaultBackFacingCamera), out var error);
            _mrzScannerViewController = new SBSDKMRZScannerViewController(viewModel: viewModel, @delegate: null);
            this.AttachViewControllerInView(_mrzScannerViewController, _scanningContainerView);
            _mrzScannerViewController.DidScanMRZ += DidScanMrzCode;
            
            // New one -- Crashes
            // _mrzScannerViewController = new SBSDKMRZScannerViewController(viewModel: new SBSDKMRZScannerViewModel(), @delegate: null);
            // this.AttachViewControllerInView(_mrzScannerViewController, _scanningContainerView);
            // _mrzScannerViewController.DidScanMRZ += DidScanMrzCode;
      }


      private void DidScanMrzCode(object sender, ScanMRZEventArgs e)
      {
            var mrzResult = e.Result.RawMRZ;
            Console.WriteLine($"MRZ scanned: " + mrzResult);
      }
}