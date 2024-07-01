namespace ClassicComponent.iOS
{
    public partial class ThumbnailCollectionViewCell : UICollectionViewCell
    {
        public ThumbnailCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        public void ShowThumbnail(UIImage image)
        {
            thumbnailImage.Image = image;
        }
    }
}