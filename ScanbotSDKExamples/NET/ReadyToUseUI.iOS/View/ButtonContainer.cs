using ReadyToUseUI.iOS.Models;

namespace ReadyToUseUI.iOS.View
{
    public class ButtonContainer : UIView
    {
        public UIView TitleContainerView { get; private set; }
        public UILabel Title { get; private set; }

        public List<ScannerButton> Buttons { get; private set; }

        float padding = 15;
        float titleHeight, buttonHeight;
        public float Height
        {
            get
            {
                return titleHeight +
                    (Buttons.Count * buttonHeight) + 
                    (Buttons.Count * padding) + padding;
            }
        }

        public ButtonContainer(string title, List<ListItem> data)
        {
            titleHeight = 40;
            buttonHeight = 30;
            
            Title = new UILabel();
            Title.Text = title;
            Title.Font = UIFont.FromName("HelveticaNeue-Bold", 16f);
            Title.TextColor = Models.Colors.NearWhite;
            Title.BackgroundColor = UIColor.Clear;
            
            TitleContainerView = new UIView(new CGRect(0, 0, this.Frame.Width, titleHeight));
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

            float x = padding;
            float y = padding;
            float w = (float)(Frame.Width - 2 * padding);
            float h = titleHeight;

            TitleContainerView.Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, h);
            Title.Frame = new CGRect(x, 0, w, h);
            
            y = h + padding;
            h = buttonHeight;

            foreach(ScannerButton button in Buttons)
            {
                button.Frame = new CGRect(x, y, w, h);
                y += h + padding;
            }
        }
    }
}
