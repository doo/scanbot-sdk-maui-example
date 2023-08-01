using CoreGraphics;
using DocumentSDK.MAUI.Example.Native.iOS.View.Collection;
using UIKit;

namespace DocumentSDK.MAUI.Example.Native.iOS.View
{
    public class ImageCollectionView : UIView
    {
        public ImageCollection Collection { get; private set; }

        public ImageCollectionView()
        {
            Collection = new ImageCollection(CGRect.Empty);
            Collection.BackgroundColor = UIColor.White;
            AddSubview(Collection);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float x = 0;
            float y = 0;
            float w = (float)Frame.Width;
            float h = (float)Frame.Height;

            Collection.Frame = new CGRect(x, y, w, h);
        }
    }
}
