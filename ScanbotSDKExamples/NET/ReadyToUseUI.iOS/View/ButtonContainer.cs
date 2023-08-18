using CoreGraphics;
using ReadyToUseUI.iOS.Models;
using UIKit;

namespace ReadyToUseUI.iOS.View
{
    public class ButtonContainer : UIView
    {
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
                    (Buttons.Count * padding);
            }
        }

        public ButtonContainer(string title, List<ListItem> data)
        {
            Title = new UILabel();
            Title.Text = title;
            Title.Font = UIFont.FromName("HelveticaNeue-Bold", 13f);
            Title.TextColor = Models.Colors.DarkGray;
            AddSubview(Title);

            Buttons = new List<ScannerButton>();

            foreach(ListItem item in data)
            {
                var button = new ScannerButton(item);
                AddSubview(button);
                Buttons.Add(button);
            }

            titleHeight = 20;
            buttonHeight = 30;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float x = padding;
            float y = 0;
            float w = (float)(Frame.Width - 2 * padding);
            float h = titleHeight;

            Title.Frame = new CGRect(x, y, w, h);

            y += h + padding;
            h = buttonHeight;

            foreach(ScannerButton button in Buttons)
            {
                button.Frame = new CGRect(x, y, w, h);
                y += h + padding;
            }
        }
    }
}
