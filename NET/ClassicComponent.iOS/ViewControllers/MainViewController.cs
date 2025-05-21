using System.Diagnostics;
using ClassicComponent.iOS.Models;
using ClassicComponent.iOS.Utils;
using ClassicComponent.iOS;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    internal interface ISDKServiceSource
    {
        List<SdkService> SDKServices { get; set; }
        void RowSelected(int index);
    }

    interface IModifyDocumentControllerDelegate
    {
        public void DidUpdateDocumentImage(SBSDKPolygon polygon = null, nint? rotation = null, SBSDKParametricFilter? filter = null);
    }

    internal interface ICameraDemoViewControllerDelegate
    {
        void DidCaptureDocumentImage(UIImage documentImage, UIImage originalImage, SBSDKPolygon polygon);
    }

    public partial class MainViewController : UIViewController, ISDKServiceSource, ICameraDemoViewControllerDelegate, IModifyDocumentControllerDelegate
    {
        public List<SdkService> SDKServices { get; set; }
        public ProgressHUD ProgressIndicator { get; private set; }

        private ImageProcessingParameters imageParameters;

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                ProgressIndicator?.ToggleLoading(value);
            }
        }

        private UIImage _originalDocumentImage, _editedDocumentImage;

        public MainViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationItem.Title = Texts.AppTitle;
            if (View != null) ProgressIndicator = ProgressHUD.Load(View.Frame);
            imageParameters = new ImageProcessingParameters();
            LoadFeatures();
            SetUpTableView();
            UpdateHintLabel(string.Empty);
            UpdateDocumentImageView(null);
            SetTapGestureToDocumentImage();
        }

        private void LoadFeatures()
        {
            SDKServices = new List<SdkService>
            {
                new SdkService { Title = SdkServiceTitle.SCANNING_UI, ServiceAction = LaunchScanningUi },
                new SdkService { Title = SdkServiceTitle.CROPPING_UI, ServiceAction = LaunchCroppingUi },
                new SdkService { Title = SdkServiceTitle.CHECK_RECOGNIZER, ServiceAction = () => NavigationController.PushViewController(new CheckRecognizerDemoViewController(), true)},
                new SdkService { Title = SdkServiceTitle.VIN_SCANNER, ServiceAction = LaunchVinScanner },
            };
        }

        private void UpdateDocumentImageView(UIImage updatedImage)
        {
            if (updatedImage == null)
            {
                imageViewVisibleConstraint.Active = false;
                imageViewHiddenConstraint.Active = true;
            }
            else
            {
                imageViewDocument.Image = updatedImage;
                imageViewHiddenConstraint.Active = false;
                imageViewVisibleConstraint.Active = true;
                UpdateHintLabel(string.Empty);
            }
        }

        private void UpdateHintLabel(string message)
        {
            if (message == null)
                message = string.Empty;

            if (message.Length == 0 && ScanbotSDKGlobal.LicenseStatus == SBSDKLicenseStatus.Trial)
            {
                message = Texts.TrialLicenseHint;
            }

            if (message.Equals(Texts.LicenseExpired))
            {
                lblHint.TextColor = UIColor.Red;
            }
            else
            {
                lblHint.TextColor = UIColor.SystemGray;
            }

            lblHint.Text = message;
        }

        private void SetUpTableView()
        {
            tableViewSDKFeatures.Source = new SdkServiceSource(this);
            tableViewSDKFeatures.TableFooterView = new UIView();
        }

        private void SetTapGestureToDocumentImage()
        {
            imageViewDocument.UserInteractionEnabled = true;
            imageViewDocument.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var viewController = new ViewFullScreenDocumentViewController(_editedDocumentImage);
                NavigationController?.PresentViewController(viewController, true, null);
            }));
        }

        private bool CheckScanbotSdkLicense()
        {
            if (ScanbotSDKGlobal.IsLicenseValid)
            {
                // Trial period, valid trial license or valid production license.
                return true;
            }

            UpdateHintLabel(message: Texts.LicenseExpired);
            return false;
        }

        public void RowSelected(int index)
        {
            if (!CheckScanbotSdkLicense()) { return; }

            if (index < 0 && index > SDKServices.Count)
            {
                return;
            }

            SDKServices[index].ServiceAction?.Invoke();
        }

        private void LaunchVinScanner()
        {
            var viewController = Utilities.GetViewController<VINScannerViewController>(Texts.ClassicComponentStoryboard);
            NavigationController?.PushViewController(viewController, true);
        }

        #region Document Scanning UI

        private void LaunchScanningUi()
        {
            var cameraViewController = new CameraDemoViewController { CameraViewControllerDelegate = this };
            NavigationController?.PushViewController(cameraViewController, true);
        }

        public void DidCaptureDocumentImage(UIImage documentImage, UIImage originalImage, SBSDKPolygon polygon)
        {
            this.imageParameters = new ImageProcessingParameters { Polygon = polygon };
            _originalDocumentImage = originalImage;
            _editedDocumentImage = documentImage;
            UpdateDocumentImageView(documentImage);
        }

        #endregion

        private void LaunchCroppingUi()
        {
            if (!CheckScanbotSdkLicense()) { return; }
            if (!CheckOriginalImageUrl()) { return; }

            UINavigationController viewController = CroppingDemoController.GetViewController(_originalDocumentImage, imageParameters, this);
            NavigationController?.PresentViewController(viewController, true, null);
        }

        private bool CheckOriginalImageUrl()
        {
            if (_originalDocumentImage == null)
            {
                UpdateHintLabel("Please snap a document image via Scanning UI or run Document Detection on an image file from the PhotoLibrary");
                tableViewSDKFeatures.ScrollsToTop = true;
                return false;
            }
            return true;
        }

        public void DidUpdateDocumentImage(SBSDKPolygon polygon = null, nint? rotation = null, SBSDKParametricFilter? filter = null)
        {
            if (rotation != null)
            {
                imageParameters.Rotation = (int)rotation.Value;
            }

            if (polygon != null)
            {
                imageParameters.Polygon = polygon;
            }

            if (filter != null)
            {
                imageParameters.Filter = filter;
            }

            _editedDocumentImage = Utilities.GetProcessedImage(ref _originalDocumentImage, imageParameters);
            UpdateDocumentImageView(_editedDocumentImage);
        }
    }

    internal class SdkServiceSource : UITableViewSource
    {
        private ISDKServiceSource sourceDelegate;

        public SdkServiceSource(ISDKServiceSource sourceDelegate)
        {
            this.sourceDelegate = sourceDelegate;
        }

        public override nint RowsInSection(UITableView tableView, nint section) => sourceDelegate.SDKServices.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
            var title = sourceDelegate.SDKServices[indexPath.Row].Title;
            Utilities.ConfigureDefaultCell(cell, title);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            sourceDelegate?.RowSelected(indexPath.Row);
        }
    }
}
