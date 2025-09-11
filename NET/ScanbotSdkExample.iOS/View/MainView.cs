using ScanbotSdkExample.iOS.Models;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.View
{
    public sealed class MainView : UIScrollView
    {
        private List<ButtonContainer> ButtonContainers { get; set; } = [];
        public UILabel LicenseIndicator { get; private set; }
        
        public List<ScannerButton> AllButtons => ButtonContainers.Aggregate(new List<ScannerButton>(), (buttons, container) =>
        {
            buttons.AddRange(container.Buttons);
            return buttons;
        });

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float largePadding = 20;
            float x = 0;
            float y = largePadding;
            float w = (float)Frame.Width;
            float h = ScanbotSDKGlobal.IsLicenseValid ? 0 : 50;

            LicenseIndicator.Frame = new CGRect(x, y, w, h);
            y += h;
            foreach (var container in ButtonContainers)
            {
                container.Frame = new CGRect(x, y, Frame.Width, container.Height);
                y += container.Height;
            }

            var height = ButtonContainers.Last().Frame.Bottom + 10;
            ContentSize = new CGSize(Frame.Width, height);
        }

        public MainView()
        {
            BackgroundColor = Colors.DarkGray;

            LicenseIndicator = new UILabel();
            LicenseIndicator.TextColor = UIColor.White;
            LicenseIndicator.BackgroundColor = Colors.ScanbotRed;
            LicenseIndicator.Font = UIFont.FromName("HelveticaNeue", 13);
            LicenseIndicator.Lines = 0;
            LicenseIndicator.TextAlignment = UITextAlignment.Center;

            AddSubview(LicenseIndicator);
        }

        public void AddContent(string title, List<ListItem> items)
        {
            var container = new ButtonContainer(title, items);
            ButtonContainers.Add(container);
            AddSubview(container);
        }
    }
}