using ScanbotSdkExample.iOS.Models;

namespace ScanbotSdkExample.iOS.View
{
    public sealed class ButtonContainer : UIView
    {
        private UIView TitleContainerView { get; set; }
        private UILabel Title { get; set; }
        public List<ScannerButton> Buttons { get; private set; }

        private const float Padding = 15;
        private readonly float _titleHeight;
        private readonly float _buttonHeight;
        public float Height =>
            _titleHeight +
            (Buttons.Count * _buttonHeight) + 
            (Buttons.Count * Padding) + Padding;

        public ButtonContainer(string title, List<ListItem> data)
        {
            _titleHeight = 40;
            _buttonHeight = 30;
            
            Title = new UILabel();
            Title.Text = title;
            Title.Font = UIFont.FromName("HelveticaNeue-Bold", 16f);
            Title.TextColor = Colors.NearWhite;
            Title.BackgroundColor = UIColor.Clear;
            
            TitleContainerView = new UIView(new CGRect(0, 0, this.Frame.Width, _titleHeight));
            TitleContainerView.BackgroundColor = Colors.ScanbotRed;
            TitleContainerView.Add(Title);

            AddSubview(TitleContainerView);
            Buttons = new List<ScannerButton>();

            foreach(ListItem item in data)
            {
                var button = new ScannerButton(item);
                AddSubview(button);
                Buttons.Add(button);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float x = Padding;
            float y;
            float w = (float)(Frame.Width - 2 * Padding);
            float h = _titleHeight;

            TitleContainerView.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, h);
            Title.Frame = new CGRect(x, 0, w, h);
            
            y = h + Padding;
            h = _buttonHeight;

            foreach(ScannerButton button in Buttons)
            {
                button.Frame = new CGRect(x, y, w, h);
                y += h + Padding;
            }
        }
    }
}
