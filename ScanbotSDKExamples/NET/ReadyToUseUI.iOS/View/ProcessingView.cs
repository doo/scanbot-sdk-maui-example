using CoreGraphics;
using UIKit;

namespace ReadyToUseUI.iOS.View
{
    public class ProcessingView : UIView
    {
        public UIImageView ImageView { get; private set; }

        public ImageProcessingBar ProcessingBar { get; private set; }

        public ProcessingView()
        {
            BackgroundColor = UIColor.White;
            ImageView = new UIImageView();
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageView.Layer.BorderColor = Models.Colors.LightGray.CGColor;
            ImageView.Layer.BorderWidth = 1;
            ImageView.BackgroundColor = Models.Colors.NearWhite;
            AddSubview(ImageView);

            ProcessingBar = new ImageProcessingBar();
            AddSubview(ProcessingBar);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 5;
            float barHeight = 50;

            float x = padding;
            float y = padding;
            
            float w = (float)Frame.Width - 2 * padding;
            float h = (float)Frame.Height - (barHeight + 2 * padding);

            ImageView.Frame = new CGRect(x, y, w, h);

            x = 0;
            y += h + padding;
            w = (float)Frame.Width;
            h = barHeight;

            ProcessingBar.Frame = new CGRect(x, y, w, h);
        }
    }
}
