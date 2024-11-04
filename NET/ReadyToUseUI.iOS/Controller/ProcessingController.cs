using ReadyToUseUI.iOS.Repository;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public class ProcessingController : UIViewController
    {
        public UIImageView ImageView { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Process Image";

            SetUpPreview();
            SetupToolbar();
        }

        private void SetUpPreview()
        {
            View.BackgroundColor = UIColor.White;

            ImageView = new UIImageView();
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageView.Image = PageRepository.Current.DocumentImage;
            ImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview(ImageView);

            ImageView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.9f).Active = true;
            ImageView.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.75f).Active = true;
            ImageView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            ImageView.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
        }

        private void SetupToolbar()
        {
            this.SetToolbarItems(new UIBarButtonItem[]
            {
                new UIBarButtonItem("Filter", UIBarButtonItemStyle.Plain, OpenFilterScreen),
                new UIBarButtonItem("Delete", UIBarButtonItemStyle.Plain, DeleteImage),
            }, true);
           this.NavigationController.SetToolbarHidden(false, false);
        }

        private void CheckQuality(object sender, EventArgs e)
        {
            var quality = new SBSDKDocumentQualityAnalyzer().AnalyzeOnImage(PageRepository.Current.DocumentImage);
            Utils.Alert.Show(this, "Document Quality", quality.ToString());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ImageView.Image = PageRepository.Current.DocumentImage;
        }

        private void OpenFilterScreen(object sender, EventArgs e)
        {
            var controller = new FilterController();
            NavigationController.PushViewController(controller, true);
        }

        private void DeleteImage(object sender, EventArgs e)
        {
            PageRepository.Remove(PageRepository.Current);
            NavigationController.PopViewController(true);
        }
    }

    public class CroppingFinishedArgs : EventArgs
    {
        public SBSDKDocumentPage Page { get; set; }
    }
}