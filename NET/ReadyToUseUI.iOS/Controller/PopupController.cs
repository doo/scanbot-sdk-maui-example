using ReadyToUseUI.iOS.View;

namespace ReadyToUseUI.iOS.Controller
{
    public class PopupController : UIViewController
    {
        public PopupView Content { get; set; }
        private readonly string _text;
        private readonly NSAttributedString _attributedString;
        private readonly List<UIImage> _images;
        private readonly bool _isAttributedText;

        public PopupController(string text)
        {
            this._text = text;
            this._images = new List<UIImage>();
        }
        
        public PopupController(NSAttributedString text)
        {
            this._attributedString = text;
            this._images = new List<UIImage>();
            _isAttributedText = true;
        }
        
        public PopupController(string text, List<UIImage> images)
        {
            this._text = text;
            this._images = images;

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

            Content = new PopupView(_text);
            Content.ImageContainer.Items = _images;
            Content.Frame = new CGRect(x, y, w, h);
            
            if (_isAttributedText)
            {
                Content.Label.AttributedText = _attributedString;
            }
            else
            {
                Content.Label.Text = _text;
            }

            View.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);
            View.AddSubview(Content);
        }

        public void Dismiss()
        {
            DismissViewController(true, null);
            Content.CloseButton.Click = null;
        }
    }
}
