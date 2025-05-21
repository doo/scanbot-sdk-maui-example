using ClassicComponent.iOS.Utils;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    public class CheckRecognizerDemoViewController : UIViewController
    {
        private SBSDKCheckScannerViewController _recognizerViewController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.TranslatesAutoresizingMaskIntoConstraints = true;
            _recognizerViewController = new SBSDKCheckScannerViewController();
            this.AttachViewControllerInView(_recognizerViewController, View);
            _recognizerViewController.Delegate = new Delegate(ShowResult);
            _recognizerViewController.IsFlashLightEnabled = false;
        }

        private void ShowResult(SBSDKCheckScanningResult result)
        {
            var alert = UIAlertController.Create("Recognized check", result.Check.ToFormattedString(), UIAlertControllerStyle.Alert);
            var okAction = UIAlertAction.Create("OK", UIAlertActionStyle.Default, delegate
            {
                alert.PresentedViewController?.DismissViewController(true, null);
            });

            alert.AddAction(okAction);
            PresentViewController(alert, true, null);
        }

        private class Delegate(Action<SBSDKCheckScanningResult> action) : SBSDKCheckScannerViewControllerDelegate
        {
            public override void DidScanCheck(SBSDKCheckScannerViewController controller, SBSDKCheckScanningResult result)
            {
                action.Invoke(result);
            }
        }
    }
}