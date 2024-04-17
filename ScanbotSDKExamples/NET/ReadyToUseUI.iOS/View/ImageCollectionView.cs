using ReadyToUseUI.iOS.View.Collection;

namespace ReadyToUseUI.iOS.View
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
