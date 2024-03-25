using ScanbotSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Utils;
using ReadyToUseUI.Maui.SubViews.ActionBar;
using ScanbotSDK.MAUI.Services;
using ReadyToUseUI.Maui.Models;

namespace ReadyToUseUI.Maui.Pages
{
    public class ImageDetailPage : ContentPage
    {
        private Image documentImage;
        private BottomActionBar bottomBar;
        private IScannedPage selectedPage;

        public ImageDetailPage(IScannedPage selectedPage)
        {
            this.selectedPage = selectedPage;
            documentImage = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray,
                Aspect = Aspect.AspectFit,
            };
            documentImage.SizeChanged += delegate
            {
                // Don't allow images larger than 2/3 of the screen
                documentImage.HeightRequest = Content.Height / 3 * 2;
            };

            bottomBar = new BottomActionBar(true);

            var gridView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { documentImage, bottomBar },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                }
            };

            gridView.SetRow(documentImage, 0);
            gridView.SetRow(bottomBar, 1);
            Content = gridView;

            bottomBar.AddClickEvent(bottomBar.CropButton, OnCropButtonClick);
            bottomBar.AddClickEvent(bottomBar.FilterButton, OnFilterButtonClick);
            bottomBar.AddClickEvent(bottomBar.DeleteButton, OnDeleteButtonClick);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            documentImage.Source = await PageDocument();
        }


        private async Task<ImageSource> PageDocument()
        {
            // If encryption is enabled, load the decrypted document.
            if (ScanbotSDK.MAUI.ScanbotSDK.SDKService.IsEncryptionEnabled)
            {
                return await selectedPage.DecryptedDocument();
            }
            
            // Else accessible via Document or DocumentPreview (uncomment as needed).
            return selectedPage.Document;
        }

        private async void OnCropButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.LaunchCroppingScreenAsync(selectedPage);

            if (result.Status == OperationResult.Ok)
            {
                documentImage.Source = null;
                await PageStorage.Instance.UpdateAsync(selectedPage);
                documentImage.Source = await PageDocument();
            }
        }

        private async void OnFilterButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var buttons = Enum.GetNames(typeof(ImageFilter));
            var action = await DisplayActionSheet("Filter", "Cancel", null, buttons);

            if (Enum.TryParse<ImageFilter>(action, out var filter))
            {
                documentImage.Source = null;
                await selectedPage.SetFilterAsync(filter);
                await PageStorage.Instance.UpdateAsync(selectedPage);

                documentImage.Source = await PageDocument();
            }
        }

        private async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            documentImage.Source = null;
            await Task.WhenAll(PageStorage.Instance.DeleteAsync(selectedPage), Navigation.PopAsync());
            selectedPage = null;
        }
    }
}