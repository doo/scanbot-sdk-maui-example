﻿using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.View.Collection
{
    public class CollectionEventArgs
    {
        public SBSDKScannedPage Page { get; set; }
    }

    public class ImageCollection : UICollectionView, IUICollectionViewSource,
        IUICollectionViewDelegate, IUICollectionViewDelegateFlowLayout
    {
        public EventHandler<CollectionEventArgs> Selected;

        public List<SBSDKScannedPage> Pages { get; set; } = new List<SBSDKScannedPage>();

        private static readonly UICollectionViewFlowLayout _layout;
        
        static ImageCollection()
        {
            _layout = new UICollectionViewFlowLayout();
            _layout.ScrollDirection = UICollectionViewScrollDirection.Vertical;
        }

        public ImageCollection(CGRect frame) : base(frame, _layout)
        {
            DataSource = this;
            Delegate = this;

            RegisterClassForCell(typeof(ImageCell), ImageCell.Identifier);
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (ImageCell)collectionView.DequeueReusableCell(ImageCell.Identifier, indexPath);

            var page = Pages[indexPath.Row];

            cell.Update(page);

            return cell;
        }

        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Pages.Count;
        }

        public override nint NumberOfItemsInSection(nint section)
        {
            return Pages.Count;
        }
        
        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var page = Pages[indexPath.Row];
            Selected?.Invoke(this, new CollectionEventArgs { Page = page });
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var size = collectionView.Bounds.Width / 3;
            return new CGSize(size, size);
        }

        [Export("collectionView:layout:insetForSectionAtIndex:")]
        public UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return UIEdgeInsets.Zero;
        }

        [Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
        public float GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return 0;
        }

        [Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
        public float GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return 0;
        }
    }
}
