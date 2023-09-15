using CoreGraphics;
using ScanbotSDK.iOS;
using UIKit;

namespace ReadyToUseUI.iOS.View.Collection
{
    public class ImageCell : UICollectionViewCell
    {
        public const string Identifier = "ImageCell";

        public UIImageView ImageView { get; private set; }

        public SBSDKUIPage Page { get; private set; }
        public ImageCell()
        {
            Initialize();
        }

        public ImageCell(IntPtr handle) : base (handle)
        {
            Initialize();
        }

        void Initialize()
        {
            ImageView = new UIImageView();
            AddSubview(ImageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            
            var padding = 5;

            float x = padding;
            float y = padding;
            float w = (float)Frame.Width - 2 * padding;
            float h = (float)Frame.Height - 2 * padding;

            ImageView.Frame = new CGRect(x, y, w, h);
        }

        public void Update(SBSDKUIPage page)
        {
            ImageView.Image = page.DocumentImage;
            Page = page;
        }
    }
}
