using DocumentSDK.MAUI.Constants;
using DocumentSDK.MAUI.Services;

namespace ReadyToUseUI.Maui.Models
{
    public class ScannedPage
    {
        public static readonly ScannedPage Instance = new ScannedPage();

        public List<IScannedPageService> List { get; set; } = new List<IScannedPageService>();

        public IEnumerable<ImageSource> DocumentSources
        {
            get => List.Select(p => p.Document).Where(image => image != null);
        }

        public IScannedPageService SelectedPage { get; set; }


        private ScannedPage() { }

        public async Task<int> RemoveSelection()
        {
            var result = await PageStorage.Instance.Delete(SelectedPage);
            List.Remove(SelectedPage);
            SelectedPage = null;
            return result;
        }

        public async Task<int> UpdateFilterForSelection(ImageFilter filter)
        {
            await SelectedPage.SetFilterAsync(filter);
            return await UpdateSelection();
        }

        public async Task<int> UpdateSelection()
        {
            return await PageStorage.Instance.Update(SelectedPage);
        }

        public async Task<bool> Add(IScannedPageService page, bool save = true)
        {
            List.Add(page);
            if (save)
            {
                await PageStorage.Instance.Save(page);
            }
            return true;
        }

        public async Task<bool> LoadFromStorage()
        {
            var pages = await PageStorage.Instance.Load();
            foreach (var page in pages)
            {
                var reconstructed = await DocumentSDK.MAUI.ScanbotSDK.SDKService.ReconstructPage(
                    page.Id,
                    page.CreatePolygon(),
                    (ImageFilter)page.Filter,
                    (DocumentDetectionStatus)page.DetectionStatus
                );
                await Add(reconstructed, false);
            }
            return true;
        }

        public async Task<int> Clear()
        {
            List.Clear();
            return await PageStorage.Instance.Clear();
        }
    }
}
