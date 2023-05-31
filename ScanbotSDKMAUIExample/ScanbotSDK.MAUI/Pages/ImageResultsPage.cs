using BarcodeSDK.MAUI.Constants;
using DocumentSDK.MAUI.Constants;
using DocumentSDK.MAUI.Example.Models;
using DocumentSDK.MAUI.Example.SubViews.ActionBar;
using DocumentSDK.MAUI.Example.SubViews.Cells;
using DocumentSDK.MAUI.Example.Utils;
using DocumentSDK.MAUI.Models;
using DocumentSDK.MAUI.Services;

namespace DocumentSDK.MAUI.Example.Pages
{
    public class ImageResultsPage : ContentPage
    {
        public bool IsLoading
        {
            get
            {
                return Loader.IsRunning;
            }
            set
            {
                if (Loader.IsRunning != value)
                {
                    Loader.IsVisible  = Loader.IsRunning = value;
                }
            }
        }

        public Grid PageGridView { get; private set; }

        public ListView List { get; private set; }

        public BottomActionBar BottomBar { get; private set; }

        public ActivityIndicator Loader { get; set; }

        public ImageResultsPage()
        {
            Title = "Image Results";
            List = new ListView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.White,
                RowHeight = 120,
                ItemTemplate = new DataTemplate(typeof(ImageResultCell)),  
            };

            BottomBar = new BottomActionBar(false);
            BottomBar.VerticalOptions = LayoutOptions.End;

            Loader = new ActivityIndicator
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Application.Current.MainPage.Height / 3 * 2,
                WidthRequest = Application.Current.MainPage.Width,
                Color = SBColors.ScanbotRed,
                IsRunning = true,
                IsEnabled = true,
                BackgroundColor = Colors.Purple,
                Scale = (DeviceInfo.Platform == DevicePlatform.iOS) ? 2 : 0.3
            };

            PageGridView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { List, BottomBar },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                }
            };

            PageGridView.SetRow(List, 0);
            PageGridView.SetRow(BottomBar, 1);

            Content = new Grid
            {
                Children = { PageGridView, Loader},
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            BottomBar.AddClickEvent(BottomBar.AddButton, OnAddButtonClick);
            BottomBar.AddClickEvent(BottomBar.SaveButton, OnSaveButtonClick);
            BottomBar.AddClickEvent(BottomBar.DeleteAllButton, OnDeleteButtonClick);

            List.ItemTapped += OnItemClick;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ReloadData();
        }

        private void OnItemClick(object sender, ItemTappedEventArgs e)
        {
            ScannedPage.Instance.SelectedPage = (IScannedPageService)e.Item;
            Navigation.PushAsync(new ImageDetailPage());
        }

        async void OnAddButtonClick(object sender, EventArgs e)
        {
            var configuration = new DocumentScannerConfiguration
            {
                CameraPreviewMode = CameraPreviewMode.FitIn,
                IgnoreBadAspectRatio = true,
                MultiPageEnabled = true,
                PolygonColor = Colors.Red,
                PolygonColorOK = Colors.Green,
                BottomBarBackgroundColor = Colors.Blue,
                PageCounterButtonTitle = "%d Page(s)",

            };
            var result = await ScanbotSDK.ReadyToUseUIService.LaunchDocumentScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                foreach (var page in result.Pages)
                {
                    ScannedPage.Instance.List.Add(page);

                }
            }
        }

        async void OnSaveButtonClick(object sender, EventArgs e)
        {
            var parameters = new string[] { "PDF", "PDF with OCR", "TIFF (1-bit, B&W)" };
            string action = await DisplayActionSheet("Save Image as", "Cancel", null, parameters);

            if (action == null || action.Equals("Cancel"))
            {
                return;
            }

            IsLoading = true;
            if (!SDKUtils.CheckLicense(this)) { return; }
            if (!SDKUtils.CheckDocuments(this, ScannedPage.Instance.DocumentSources)) { return; }

            if (action.Equals(parameters[0]))
            {
                var fileUri = await ScanbotSDK.SDKService
                .CreatePdfAsync(ScannedPage.Instance.DocumentSources, PDFPageSize.FixedA4);
                ViewUtils.Alert(this, "Success: ", "Wrote documents to: " + fileUri.AbsolutePath);
            }
            else if (action.Equals(parameters[1]))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string pdfFilePath = Path.Combine(path, Guid.NewGuid() + ".pdf");
                var languages = new[] { "en" };
                var result = await ScanbotSDK.SDKService.PerformOcrAsync(ScannedPage.Instance.DocumentSources, languages, pdfFilePath);

                // You can access the results with: result.Pages
                ViewUtils.Alert(this, "PDF with OCR layer stored: ", pdfFilePath);
            }
            else if (action.Equals(parameters[2]))
            {
                var fileUri = await ScanbotSDK.SDKService.WriteTiffAsync(
                    ScannedPage.Instance.DocumentSources,
                    new TiffOptions { OneBitEncoded = true, Dpi = 300, Compression = TiffCompressionOptions.CompressionCcittT6 }
                );
                ViewUtils.Alert(this, "Success: ", "Wrote documents to: " + fileUri.AbsolutePath);
            }
            IsLoading = false;
        }

        private async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            var message = "Do you really want to delete all image data?";
            var result = await this.DisplayAlert("Attention!", message, "Yes", "No");
            if (result)
            {
                await ScannedPage.Instance.Clear();
                await ScanbotSDK.SDKService.CleanUp();
                ReloadData();
            }

        }

        void ReloadData()
        {
            List.ItemsSource = null;
            List.ItemsSource = ScannedPage.Instance.List;
            IsLoading = false;
        }
    }
}