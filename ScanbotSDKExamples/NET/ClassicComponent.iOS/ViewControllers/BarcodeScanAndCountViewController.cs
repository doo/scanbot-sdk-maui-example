// This file has been autogenerated from a class added in the UI designer.

using ClassicComponent.iOS.Utils;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    interface IBarcodeScanAndCountViewDelegate
    {
        List<SBSDKBarcodeScannerAccumulatingResult> ScannedBarcodes { get; set; }
        void UpdateScannedItems();
    }

    public partial class BarcodeScanAndCountViewController : UIViewController, IBarcodeScanAndCountViewDelegate
    {
        SBSDKBarcodeScanAndCountViewController scannerViewController;
        public BarcodeScanAndCountViewController (IntPtr handle) : base (handle)
		{
		}

        #region IBarcodeScanAndCountViewDelegate Implementation

        public List<SBSDKBarcodeScannerAccumulatingResult> ScannedBarcodes { get; set; }

        public void UpdateScannedItems()
        {
            var count = (int)ScannedBarcodes.Sum(item => item.ScanCount);
            btnBarcodeCount.Title = string.Format("{0}: {1}", Texts.TotalItemsScanned, count);
        }

        #endregion

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ScannedBarcodes = new List<SBSDKBarcodeScannerAccumulatingResult>();
            scannerViewController = new SBSDKBarcodeScanAndCountViewController(parentViewController: this,
                                                                      containerView, new BarcodeScanAndCountViewDelegate(this));
            scannerViewController.AcceptedBarcodeTypes = SBSDKBarcodeType.AllTypes;
        }

        partial void BtnShowResults_Action(UIBarButtonItem sender)
        {
            var viewController = Utilities.GetViewController<BarcodeScanAndCountResultViewController>(Texts.ClassicComponentStoryboard);
            viewController.NavigateData(ScannedBarcodes);
            this.NavigationController?.PushViewController(viewController, true);
        }
    }

    internal class BarcodeScanAndCountViewDelegate : SBSDKBarcodeScanAndCountViewControllerDelegate
    {
        private IBarcodeScanAndCountViewDelegate scanAndCountViewDelegate;

        internal BarcodeScanAndCountViewDelegate(IBarcodeScanAndCountViewDelegate scanAndCountViewDelegate)
        {
            this.scanAndCountViewDelegate = scanAndCountViewDelegate;
        }

        public override void DidDetectBarcodes(SBSDKBarcodeScanAndCountViewController controller, SBSDKBarcodeScannerResult[] codes)
        {
            foreach (var code in codes)
            {
                var existingBarcode = scanAndCountViewDelegate.ScannedBarcodes.Find(item => item.Code.Type == code.Type && item.Code.RawTextString == code.RawTextString);
                if (existingBarcode != null)
                {
                    existingBarcode.ScanCount += 1;
                    existingBarcode.Code.DateOfDetection = code.DateOfDetection;
                }
                else
                {
                    scanAndCountViewDelegate.ScannedBarcodes.Add(new SBSDKBarcodeScannerAccumulatingResult(code));
                }
            }
            scanAndCountViewDelegate.UpdateScannedItems();
        }

        public override UIView OverlayForBarcode(SBSDKBarcodeScanAndCountViewController controller, SBSDKBarcodeScannerResult code)
        {
            return new UIImageView(image: UIImage.CheckmarkImage);
        }
    }
}