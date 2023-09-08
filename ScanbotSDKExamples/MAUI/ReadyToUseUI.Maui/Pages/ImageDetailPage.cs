using DocumentSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Utils;
using ReadyToUseUI.Maui.SubViews.ActionBar;
using DocumentSDK.MAUI.Services;
using ReadyToUseUI.Maui.Models;

namespace ReadyToUseUI.Maui.Pages
{

    public class ImageDetailPage : ContentPage
    {
        private Image image;
        private BottomActionBar bottomBar;
        private IScannedPageService selectedPage;

        public ImageDetailPage(IScannedPageService selectedPage)
        {
            this.selectedPage = selectedPage;
            image = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray,
                Aspect = Aspect.AspectFit,
            };
            image.SizeChanged += delegate
            {
                // Don't allow images larger than 2/3 of the screen
                image.HeightRequest = Content.Height / 3 * 2;
            };

            bottomBar = new BottomActionBar(true);

            var gridView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { image, bottomBar },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                }
            };

            gridView.SetRow(image, 0);
            gridView.SetRow(bottomBar, 1);
            Content = gridView;

            bottomBar.AddClickEvent(bottomBar.CropButton, OnCropButtonClick);
            bottomBar.AddClickEvent(bottomBar.FilterButton, OnFilterButtonClick);
            bottomBar.AddClickEvent(bottomBar.DeleteButton, OnDeleteButtonClick);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            image.Source = await PageDocument();
        }


        async Task<ImageSource> PageDocument()
        {
            // If encryption is enabled, load the decrypted document.
             return await selectedPage.DecryptedDocument();

            // Else accessible via Document or DocumentPreview (uncomment as needed).
            //return selectedPage.Document;
        }

        async void OnCropButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            image.Source = null;
            await DocumentSDK.MAUI.ScanbotSDK.ReadyToUseUIService.LaunchCroppingScreenAsync(selectedPage);
            await PageStorage.Instance.UpdateAsync(selectedPage);
            image.Source = await PageDocument();
        }

        async void OnFilterButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var buttons = Enum.GetNames(typeof(ImageFilter));
            var action = await DisplayActionSheet("Filter", "Cancel", null, buttons);

            if (Enum.TryParse<ImageFilter>(action, out var filter))
            {
                image.Source = null;
                await selectedPage.SetFilterAsync(filter);
                await PageStorage.Instance.UpdateAsync(selectedPage);

                image.Source = await PageDocument();
            }
        }

        async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            image.Source = null;
            await Task.WhenAll(PageStorage.Instance.DeleteAsync(selectedPage), Navigation.PopAsync());
            selectedPage = null;
        }

    }

}