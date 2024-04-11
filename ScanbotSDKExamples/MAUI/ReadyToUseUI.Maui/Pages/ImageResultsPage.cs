using ScanbotSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.SubViews.ActionBar;
using ReadyToUseUI.Maui.SubViews.Cells;
using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI.Models;
using ScanbotSDK.MAUI.Services;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;
using ScanbotSDK.MAUI;
using System.Collections.ObjectModel;

namespace ReadyToUseUI.Maui.Pages
{
    public class ImageResultsPage : ContentPage
    {
        private Grid pageGridView;
        private ListView resultList;
        private BottomActionBar bottomBar;
        private ActivityIndicator loader;

        private ObservableCollection<IScannedPage> scannedPages = new ObservableCollection<IScannedPage>();

        public ImageResultsPage()
        {
            Title = "Image Results";
            BackgroundColor = Colors.White;
            resultList = new ListView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.White,
                RowHeight = 120,
                HeightRequest = Application.Current.MainPage.Height,
                ItemTemplate = new DataTemplate(typeof(ImageResultCell)),
                ItemsSource = scannedPages
            };

            bottomBar = new BottomActionBar(false);
            bottomBar.VerticalOptions = LayoutOptions.End;

            loader = new ActivityIndicator
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Application.Current.MainPage.Height / 3 * 2,
                WidthRequest = Application.Current.MainPage.Width,
                Color = SBColors.ScanbotRed,
                IsRunning = true,
                IsEnabled = true,
                Scale = (DeviceInfo.Platform == DevicePlatform.iOS) ? 2 : 0.3
            };

            pageGridView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { resultList, bottomBar },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                }
            };

            pageGridView.SetRow(resultList, 0);
            pageGridView.SetRow(bottomBar, 1);

            Content = new Grid
            {
                Children = { pageGridView, loader },
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            bottomBar.AddClickEvent(bottomBar.AddButton, OnAddButtonClick);
            bottomBar.AddClickEvent(bottomBar.SaveButton, OnSaveButtonClick);
            bottomBar.AddClickEvent(bottomBar.DeleteAllButton, OnDeleteButtonClick);

            resultList.ItemTapped += OnItemClick;
        }

        private bool isLoading
        {
            get
            {
                return loader.IsRunning;
            }
            set
            {
                if (loader.IsRunning != value)
                {
                    loader.IsVisible = loader.IsRunning = value;
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                isLoading = true;
                var savedPages = await PageStorage.Instance.LoadAsync();

                scannedPages.Clear();

                resultList.HeightRequest = Height;
                savedPages.ForEach(scannedPages.Add);
            }
            finally
            {
                isLoading = false;
            }
        }

        private void OnItemClick(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is IScannedPage selectedPage && selectedPage != null)
            {
                Navigation.PushAsync(new ImageDetailPage(selectedPage));
            }
        }

        async void OnAddButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

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
            var result = await SBSDK.ReadyToUseUIService.LaunchDocumentScannerAsync(configuration);

            if (result.Status == OperationResult.Ok)
            {
                foreach (var page in result.Pages)
                {
                    await PageStorage.Instance.CreateAsync(page);
                    scannedPages.Add(page);
                }
            }
        }

        async void OnSaveButtonClick(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var documentSources = scannedPages
                .Where(p => p.Document != null)
                .Select(p => p.Document)
                .ToList();

            if (documentSources.Count == 0)
            {
                ViewUtils.Alert(this, "Oops!", "Please import or scan a document first");
                return;
            }

            var parameters = new string[] { "PDF", "PDF with OCR", "TIFF (1-bit, B&W)" };
            string action = await DisplayActionSheet("Save Image as", "Cancel", null, parameters);

            if (action == null || action.Equals("Cancel"))
            {
                return;
            }

            try
            {
                isLoading = true;

                if (action.Equals(parameters[0]))
                {
                    var fileUri = await SBSDK.SDKService.CreatePdfAsync(documentSources.OfType<FileImageSource>(), PDFPageSize.A4);
                    ViewUtils.Alert(this, "Success: ", "Wrote documents to: " + fileUri.AbsolutePath);
                }
                else if (action.Equals(parameters[1]))
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    string pdfFilePath = Path.Combine(path, Guid.NewGuid() + ".pdf");
                    var languages = new[] { "en" };
                    var result = await SBSDK.SDKService.PerformOcrAsync(documentSources.OfType<FileImageSource>(), OcrConfig.From(languages), pdfFilePath);

                    // You can access the results with: result.Pages
                    ViewUtils.Alert(this, "PDF with OCR layer stored: ", pdfFilePath);
                }
                else if (action.Equals(parameters[2]))
                {
                    var fileUri = await SBSDK.SDKService.WriteTiffAsync(
                        documentSources.OfType<FileImageSource>(),
                        new TiffOptions { OneBitEncoded = true, Dpi = 300, Compression = TiffCompressionOptions.CompressionCcittT6 }
                    );
                    ViewUtils.Alert(this, "Success: ", "Wrote documents to: " + fileUri.AbsolutePath);
                }

            }
            catch (Exception ex)
            {
                // Making the error prettier.
                var errorMessage = ex.Message.Substring(ex.Message.LastIndexOf(':')).Trim('{', '}');
                ViewUtils.Alert(this, "Error: ", $"An error occurred while saving the document: {errorMessage}");
            }
            finally
            {
                isLoading = false;
            }            
        }

        private async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            var message = "Do you really want to delete all image data?";
            var result = await this.DisplayAlert("Attention!", message, "Yes", "No");
            if (result)
            {
                await PageStorage.Instance.ClearAsync();
                await SBSDK.SDKService.CleanUp();
                scannedPages.Clear();
            }
        }
    }
}