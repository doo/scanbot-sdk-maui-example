using ReadyToUseUI.iOS.View;

namespace ReadyToUseUI.iOS.Controller
{
    public class PopupController : UIViewController
    {
        public PopupView Content { get; set; }
        private string text;
        private List<UIImage> images;

        public PopupController(string text)
        {
            this.text = text;
            this.images = new List<UIImage>();
        }
        public PopupController(string text, List<UIImage> images)
        {
            this.text = text;
            this.images = images;

            ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
        }

        public override void ViewDidLoad()
        {
            float hPadding = 20;
            float vPadding = (float)View.Frame.Height / 6;

            float x = hPadding;
            float y = vPadding;
            float w = (float)View.Frame.Width - 2 * hPadding;
            float h = (float)View.Frame.Height - 2 * vPadding;

            Content = new PopupView(text);
            Content.ImageContainer.Items = images;
            Content.Frame = new CGRect(x, y, w, h);
            Content.Label.Text = text;
            
            View.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);
            View.AddSubview(Content);
        }

        public void Dismiss()
        {
            DismissModalViewController(true);
            Content.CloseButton.Click = null;
        }
    }
}
