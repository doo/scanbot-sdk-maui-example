using DocumentSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Utils;
using ScannedItem = ReadyToUseUI.Maui.Models.ScannedPage;
using ReadyToUseUI.Maui.SubViews.ActionBar;

namespace ReadyToUseUI.Maui.Pages
{

    public class ImageDetailPage : ContentPage
    {
        public Image Image { get; private set; }

        public BottomActionBar BottomBar { get; private set; }

        public ImageFilter CurrentFilter { get; set; }

        public ImageDetailPage()
        {
            Image = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray,
                Aspect = Aspect.AspectFit,
            };
            Image.SizeChanged += delegate
            {
                // Don't allow images larger than 2/3 of the screen
                Image.HeightRequest = Content.Height / 3 * 2;
            };

            BottomBar = new BottomActionBar(true);

            var gridView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { Image, BottomBar },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                }
            };

            gridView.SetRow(Image, 0);
            gridView.SetRow(BottomBar, 1);
            Content = gridView;

            BottomBar.AddClickEvent(BottomBar.CropButton, OnCropButtonClick);
            BottomBar.AddClickEvent(BottomBar.FilterButton, OnFilterButtonClick);
            BottomBar.AddClickEvent(BottomBar.DeleteButton, OnDeleteButtonClick);

            LoadImage();
        }

        async void LoadImage()
        {
            // If encryption is enabled, load the decrypted document.
            // Else accessible via Document or DocumentPreview
            Image.Source = await ReadyToUseUI.Maui.Models.ScannedPage.Instance.SelectedPage.DecryptedDocument();
            //Image.Source = ScannedPage.Instance.SelectedPage.Document;
        }

        async void OnCropButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }
            if (!SDKUtils.CheckPage(this, ScannedItem.Instance.SelectedPage)) { return; }

            await DocumentSDK.MAUI.ScanbotSDK.ReadyToUseUIService.LaunchCroppingScreenAsync(ScannedItem.Instance.SelectedPage);
            await ScannedItem.Instance.UpdateSelection();

            Image.Source = null;
            LoadImage();
        }

        async void OnFilterButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }
            if (!SDKUtils.CheckPage(this, ScannedItem.Instance.SelectedPage)) { return; }

            var buttons = Enum.GetNames(typeof(ImageFilter));
            var action = await DisplayActionSheet("Filter", "Cancel", null, buttons);

            ImageFilter filter;
            Enum.TryParse(action, out filter);
            CurrentFilter = filter;

            await ScannedItem.Instance.UpdateFilterForSelection(filter);
            LoadImage();
        }

        async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            ScannedItem.Instance.RemoveSelection();
            Image.Source = null;
            await Navigation.PopAsync();
        }

    }

}