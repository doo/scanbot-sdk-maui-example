using ObjCRuntime;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.View.Collection
{
    public class ImageCell : UICollectionViewCell
    {
        public const string Identifier = "ImageCell";

        public UIImageView ImageView { get; private set; }

        public SBSDKUIPage Page { get; private set; }

        public ImageCell(NativeHandle handle) : base(handle)
        {
            Initialize();
        }

        public ImageCell()
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
