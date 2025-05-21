using ClassicComponent.iOS.Models;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    public class CroppingDemoController : SBSDKImageEditingViewControllerDelegate
    {
        private UIImage _image;
        private ImageProcessingParameters _imageParameters;
        private SBSDKImageEditingViewController _imageEditingViewController;
        private IModifyDocumentControllerDelegate _modifyDocumentDelegate;

        internal static UINavigationController GetViewController(UIImage originalDocumentImage, ImageProcessingParameters imageParameters, IModifyDocumentControllerDelegate controllerDelegate)
        {
            var cropViewController = new CroppingDemoController
            {
                _image = originalDocumentImage,
                _imageParameters = imageParameters,
                _modifyDocumentDelegate = controllerDelegate
            };

            var navigationController = new UINavigationController(cropViewController.GetCroppingViewController());
            navigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            navigationController.NavigationBar.TintColor = UIColor.White;
            navigationController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            return navigationController;
        }

        internal SBSDKImageEditingViewController GetCroppingViewController()
        {
            _imageEditingViewController = SBSDKImageEditingViewController.CreateWithImage(_image,_imageParameters.Polygon);
            _imageEditingViewController.Delegate = this;
            _imageEditingViewController.IsRotationEnabled = true;
            _imageEditingViewController.Rotations = _imageParameters?.Rotation ?? 0;
            return _imageEditingViewController;
        }

        [Export("imageEditingViewControllerToolbarStyle:")]
        public override UIBarStyle ToolbarStyle(SBSDKImageEditingViewController editingViewController)
        {
            return UIBarStyle.Default;
        }

        [Export("imageEditingViewControllerToolbarItemTintColor:")]
        public override UIColor ToolbarItemTintColor(SBSDKImageEditingViewController editingViewController)
        {
            return UIColor.White;
        }

        [Export("imageEditingViewControllerToolbarTintColor:")]
        public override UIColor ToolbarTintColor(SBSDKImageEditingViewController editingViewController)
        {
            return UIColor.Black;
        }

        [Export("imageEditingViewController:didApplyChangesWithPolygon:croppedImage:")]
        public override void DidApplyChangesWith(SBSDKImageEditingViewController editingViewController, SBSDKPolygon polygon, UIImage croppedImage)
        {
            _modifyDocumentDelegate?.DidUpdateDocumentImage(editingViewController.Polygon, -editingViewController.Rotations);
            editingViewController.NavigationController?.DismissViewController(true, null);
        }

        [Export("imageEditingViewControllerDidCancelChanges:")]
        public override void DidCancelChanges(SBSDKImageEditingViewController editingViewController)
        {
            editingViewController.NavigationController?.DismissViewController(true, null);
        }

        [Export("imageEditingViewControllerApplyButtonItem:")]
        public override UIBarButtonItem ApplyButtonItem(SBSDKImageEditingViewController editingViewController)
        {
            return new UIBarButtonItem(UIImage.FromBundle("tickIcon"), UIBarButtonItemStyle.Plain, null);
        }

        [Export("imageEditingViewControllerCancelButtonItem:")]
        public override UIBarButtonItem CancelButtonItem(SBSDKImageEditingViewController editingViewController)
        {
            return new UIBarButtonItem(UIImage.FromBundle("closeIcon"), UIBarButtonItemStyle.Plain, null);
        }

        [Export("imageEditingViewControllerRotateClockwiseToolbarItem:")]
        public override UIBarButtonItem RotateClockwiseToolbarItem(SBSDKImageEditingViewController editingViewController)
        {
            return new UIBarButtonItem(UIImage.FromBundle("rotateIcon"), UIBarButtonItemStyle.Plain, (_,_) =>
            {
                _imageEditingViewController?.RotateInputImageClockwise(true, true);
            });
        }
    }
}
