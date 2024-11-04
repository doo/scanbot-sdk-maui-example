
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Repository
{
	public class PageRepository
	{
        public static SBSDKScannedPage Current { get; set; }

        public static List<SBSDKScannedPage> Items { get; private set; } = new List<SBSDKScannedPage>();

        public static NSUrl[] DocumentImageURLs => Items.Select(x => x.DocumentImageURI).ToArray();

        public static void Remove(SBSDKScannedPage page)
        {
            SBSDKDocumentPageFileStorage.DefaultStorage.RemovePageFileID(new NSUuid(page.Uuid));
            Items.Remove(page);
        }

        public static void Add(SBSDKScannedPage page)
        {
            Items.Add(page);
        }

        public static SBSDKScannedPage Add(UIImage image, SBSDKPolygon polygon)
        {
            var document = new SBSDKScannedDocument();
            var scannedPage = document.AddPageWith(image, polygon, new[] { new SBSDKParametricFilter() });
            Add(scannedPage);
            return scannedPage;
        }

        public static void Update(SBSDKScannedPage page)
        {
            var existing = Items.Where(p => p.Uuid.Equals(page.Uuid)).ToList()[0];
            Items.Remove(existing);

            Items.Add(page);
        }

        public static void UpdateCurrent(UIImage image, SBSDKPolygon polygon)
        {
            var document = new SBSDKScannedDocument();
            var  page = document.AddPageWith(image, polygon, Current.Filters);
            
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
            foreach (SBSDKScannedPage page in Items)
            {
                page.Filters = new[] { filter };
            }
        }

        public static SBSDKScannedPage Apply(SBSDKParametricFilter filter, SBSDKScannedPage page)
        {
            foreach (SBSDKScannedPage item in Items)
            {
                if (page.Uuid.Equals(item.Uuid))
                {
                    item.Filters = new[] { filter };
                    return item;
                }
            }

            return null;
        }

        public static SBSDKScannedPage DuplicateCurrent(SBSDKParametricFilter type)
        {
            var document = new SBSDKScannedDocument();
            return document.AddPageWith(Current.OriginalImage, Current.Polygon, new[] { type });
        }
    }
}