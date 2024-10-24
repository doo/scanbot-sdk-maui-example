﻿using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Repository
{
    public class PageRepository
    {
        public static SBSDKDocumentPage Current { get; set; }

        public static List<SBSDKDocumentPage> Items { get; private set; } = new List<SBSDKDocumentPage>();

        public static NSUrl[] DocumentImageURLs => Items.Select(x => x.DocumentImageURL).ToArray();

        public static void Remove(SBSDKDocumentPage page)
        {
            SBSDKDocumentPageFileStorage.DefaultStorage.RemovePageFileID(page.PageFileUUID);
            Items.Remove(page);
        }

        public static void Add(SBSDKDocumentPage page)
        {
            Items.Add(page);
        }

        public static SBSDKDocumentPage Add(UIImage image, SBSDKPolygon polygon)
        {
            var page = new SBSDKDocumentPage(image, polygon, SBSDKImageFilterType.None);
            Add(page);
            return page;
        }

        public static void Update(SBSDKDocumentPage page)
        {
            var existing = Items.Where(p => p.PageFileUUID.IsEqual(page.PageFileUUID)).ToList()[0];
            Items.Remove(existing);

            Items.Add(page);
        }

        public static void UpdateCurrent(UIImage image, SBSDKPolygon polygon)
        {
            var page = new SBSDKDocumentPage(image, polygon, Current.Filter);
            
            Remove(Current);
            Add(page);
            Current = page;
        }

        public static void Clear()
        {
            SBSDKDocumentPageFileStorage.DefaultStorage.RemoveAll();
            Items.Clear();
        }

        public static void Apply(SBSDKParametricFilter filter)
        {
            foreach (SBSDKDocumentPage page in Items)
            {
                page.ParametricFilters = new[] { filter };
            }
        }

        public static SBSDKDocumentPage Apply(SBSDKParametricFilter filter, SBSDKDocumentPage page)
        {
            foreach (SBSDKDocumentPage item in Items)
            {
                if (page.PageFileUUID.IsEqual(item.PageFileUUID))
                {
                    item.ParametricFilters = new[] { filter };
                    return item;
                }
            }

            return null;
        }

        public static SBSDKDocumentPage DuplicateCurrent(SBSDKParametricFilter type)
        {
            return new SBSDKDocumentPage(Current.OriginalImage, Current.Polygon, new[] { type });
        }
    }
}
