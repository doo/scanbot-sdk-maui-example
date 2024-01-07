using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.View
{
    public class MainView : UIScrollView
    {
        public UILabel LicenseIndicator { get; private set; }

        public List<ButtonContainer> ButtonContainers { get; private set; } = new List<ButtonContainer> { };

        public List<ScannerButton> AllButtons => ButtonContainers.Aggregate(new List<ScannerButton>(), (buttons, container) =>
            {
                buttons.AddRange(container.Buttons);
                return buttons;
            });

        public MainView()
        {
            BackgroundColor = UIColor.White;

            LicenseIndicator = new UILabel();
            LicenseIndicator.TextColor = UIColor.White;
            LicenseIndicator.BackgroundColor = Models.Colors.ScanbotRed;
            LicenseIndicator.Layer.CornerRadius = 5;
            LicenseIndicator.Font = UIFont.FromName("HelveticaNeue", 13);
            LicenseIndicator.Lines = 0;
            LicenseIndicator.ClipsToBounds = true;
            LicenseIndicator.TextAlignment = UITextAlignment.Center;

            AddSubview(LicenseIndicator);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float largePadding = 20;

            float x = largePadding;
            float y = largePadding;
            float w = (float)(float)Frame.Width - 2 * x;
            float h = (float)(float)Frame.Width / 6;

            if (ScanbotSDKGlobal.IsLicenseValid)
            {
                h = 0;
            }

            LicenseIndicator.Frame = new CGRect(x, y, w, h);

            x = 0;
            y += h + largePadding;
            w = (float)(float)Frame.Width;
            h = ButtonContainers[0].Height;

            ButtonContainers[0].Frame = new CGRect(x, y, w, h);

            y += h + largePadding;
            h = ButtonContainers[1].Height;

            ButtonContainers[1].Frame = new CGRect(x, y, w, h);
            if (ButtonContainers.Count >= 3)
            {
                y += h + largePadding;
                h = ButtonContainers[2]?.Height ?? 0;

                ButtonContainers[2].Frame = new CGRect(x, y, w, h);
            }
            ContentSize = new CGSize(Frame.Width, ButtonContainers[1].Frame.Bottom);
        }

        public void AddContent(string title, List<ListItem> items)
        {
            var container = new ButtonContainer(title, items);
            ButtonContainers.Add(container);
            AddSubview(container);
        }
    }
}
