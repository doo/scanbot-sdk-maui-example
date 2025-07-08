namespace ScanbotSdkExample.iOS.View
{
    public sealed class PopupView : UIView
    {
        public UILabel Label { get; private set; }

        public PopupButton CloseButton { get; private set; }

        public PopupImageContainer ImageContainer { get; internal set; }

        public PopupView(string text)
        {
            SetUpView();
            Label.Text = text;
        }
        
        public PopupView(NSAttributedString text)
        {
            SetUpView();
            Label.AttributedText = text;
        }

        private void SetUpView()
        {
            BackgroundColor = UIColor.White;
            Layer.CornerRadius = 5;

            ImageContainer = new PopupImageContainer();
            AddSubview(ImageContainer);

            Label = new UILabel();
            Label.Lines = 0;
            Label.LineBreakMode = UILineBreakMode.WordWrap;
            Label.TextColor = UIColor.Black;
            AddSubview(Label);

            CloseButton = new PopupButton("CLOSE");
            AddSubview(CloseButton);

            ClipsToBounds = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float buttonW = (float)Frame.Width / 2;
            float buttonH = buttonW / 3.5f;

            float imagesH = 0;

            if (ImageContainer.Items.Count > 0)
            {
                imagesH = (float)Frame.Width / 5;
            }

            float padding = 5;

            float x = padding;
            float y = padding;
            float w = (float)Frame.Width - 2 * padding;
            float h = imagesH;

            ImageContainer.Frame = new CGRect(x, y, w, h);

            y += h + padding;
            h = (float)Frame.Height - (3 * padding + buttonH + imagesH);

            Label.Frame = new CGRect(x, y, w, h);

            y = (float)Frame.Height - buttonH;
            h = buttonH;

            CloseButton.Frame = new CGRect(x, y, w, h);
        }
    }

    public sealed class PopupButton : UIView
    {
        public EventHandler<EventArgs> Click;
        private readonly UILabel _label;

        public PopupButton(string text)
        {
            _label = new UILabel();
            _label.Text = text;
            _label.TextColor = Models.Colors.AppleBlue;
            _label.ClipsToBounds = true;
            _label.TextAlignment = UITextAlignment.Center;
            _label.Font = UIFont.FromName("HelveticaNeue-Bold", 15);
            AddSubview(_label);

            ClipsToBounds = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _label.Frame = Bounds;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            Layer.Opacity = 0.5f;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            Layer.Opacity = 1.0f;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            Layer.Opacity = 1.0f;
            Click?.Invoke(this, EventArgs.Empty);
        }
    }

    public class PopupImageContainer : UIView
    {
        private readonly List<UIImageView> _views = new List<UIImageView>();
        private List<UIImage> _images;
        public List<UIImage> Items
        {
            get => _images;
            set
            {
                _images = value;
                foreach (var image in _images)
                {
                    var view = new UIImageView();
                    view.Image = image;
                    _views.Add(view);
                    AddSubview(view);
                }
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var padding = 3;

            float x = 0;
            float y = 0;
            float w = ((float)Frame.Width - 2 * padding) / 3;
            float h = (float)Frame.Height;

            foreach (var view in _views)
            {
                view.Frame = new CGRect(x, y, w, h);
                x += w + padding;
            }
        }
    }
}
