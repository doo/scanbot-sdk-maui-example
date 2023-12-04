using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    public delegate void CropViewControllerDidFinish(UIImage croppedImage);

    public class CroppingDemoNavigationController : UINavigationController
    {
        private UIImage Image;

        private SBSDKImageEditingViewController imageEditingViewController;

        public CropViewControllerDidFinish croppingDelegate;

        public CroppingDemoNavigationController(UIImage image)
        {
            Image = image;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            imageEditingViewController = new SBSDKImageEditingViewController();
            imageEditingViewController.Image = Image;
            imageEditingViewController.WeakDelegate = this;

            if (imageEditingViewController.Polygon == null)
            {
                // if no polygon was detected, we set a default polygon
                imageEditingViewController.Polygon = new SBSDKPolygon(); // {0,0}, {1,0}, {1,1}, {0,1}
            }

            PushViewController(imageEditingViewController, false);
        }

        [Export("imageEditingViewControllerToolbarStyle:")]
        public UIBarStyle ImageEditingViewControllerToolbarStyle(SBSDKImageEditingViewController editingViewController)
        {
            return UIBarStyle.Default;
        }

        [Export("imageEditingViewControllerToolbarItemTintColor:")]
        public UIColor ImageEditingViewControllerToolbarItemTintColor(SBSDKImageEditingViewController editingViewController)
        {
            return UIColor.White;
        }

        [Export("imageEditingViewControllerToolbarTintColor:")]
        public UIColor ImageEditingViewControllerToolbarTintColor(SBSDKImageEditingViewController editingViewController)
        {
            return UIColor.Black;
        }

        [Export("imageEditingViewController:didApplyChangesWithPolygon:croppedImage:")]
        public void ImageEditingViewController(SBSDKImageEditingViewController editingViewController, SBSDKPolygon polygon, UIImage croppedImage)
        {
            croppingDelegate?.Invoke(croppedImage);
            DismissViewController(true, null);
        }

        [Export("imageEditingViewControllerDidCancelChanges:")]
        public void ImageEditingViewControllerDidCancelChanges(SBSDKImageEditingViewController editingViewController)
        {
            DismissViewController(true, null);
        }

        [Export("imageEditingViewControllerApplyButtonItem:")]
        UIBarButtonItem ImageEditingViewControllerApplyButtonItem(SBSDKImageEditingViewController editingViewController)
        {
            return new UIBarButtonItem(UIImage.FromBundle("ui_action_checkmark"), UIBarButtonItemStyle.Plain, null);
        }

        [Export("imageEditingViewControllerCancelButtonItem:")]
        UIBarButtonItem ImageEditingViewControllerCancelButtonItem(SBSDKImageEditingViewController editingViewController)
        {
            return new UIBarButtonItem(UIImage.FromBundle("ui_action_close"), UIBarButtonItemStyle.Plain, null);
        }

        [Export("imageEditingViewControllerRotateClockwiseToolbarItem:")]
        public UIBarButtonItem ImageEditingViewControllerRotateClockwiseToolbarItem(SBSDKImageEditingViewController editingViewController)
        {
            return new UIBarButtonItem(UIImage.FromBundle("ui_edit_rotate"), UIBarButtonItemStyle.Plain, (sender, e) =>
            {
                imageEditingViewController.RotateInputImageClockwise(true, true);
            });
        }
    }
}
