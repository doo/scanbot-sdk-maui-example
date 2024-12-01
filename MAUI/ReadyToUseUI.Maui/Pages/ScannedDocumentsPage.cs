using ReadyToUseUI.Maui.SubViews.ActionBar;
using ReadyToUseUI.Maui.SubViews.Cells;
using ReadyToUseUI.Maui.Utils;
using static ScanbotSDK.MAUI.ScanbotSDKMain;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Document.Legacy;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReadyToUseUI.Maui.SubViews;

namespace ReadyToUseUI.Maui.Pages
{
    public class ScannedDocumentsPage : ContentPage, INotifyPropertyChanged
    {
        private const string PDF = "PDF", OCR = "Perform OCR", SandwichPDF = "Sandwiched PDF", TIFF = "TIFF (1-bit, B&W)";
        private Grid pageGridView;
        private ListView resultList;
        private BottomActionBar bottomBar;
        private SBLoader loader;

        private ScannedDocument document;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                loader.IsBusy = value;
                this.OnPropertyChanged(nameof(IsLoading));
            }
        }

        private struct TempLoader : IDisposable
        {
            private ScannedDocumentsPage page;
            public TempLoader(ScannedDocumentsPage page)
            {
                this.page = page;
                page.IsLoading = true;
            }

            public void Dispose()
            {
                page.IsLoading = false;
            }
        }

        public ScannedDocumentsPage(ScannedDocument document)
        {
            this.document = document;
            Title = "Document";
            BackgroundColor = Colors.White;
            resultList = new ListView
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.White,
                RowHeight = 120,
                HeightRequest = Application.Current.MainPage.Height,
                ItemTemplate = new DataTemplate(typeof(ScannedDocumentPageItemTemplate)),
                ItemsSource = document.Pages
            };

            bottomBar = new BottomActionBar(isDetailPage: false);
            bottomBar.VerticalOptions = LayoutOptions.End;

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

            loader = new SBLoader
            {
                IsVisible = false
            };

            Content = new Grid
            {
                Children = { pageGridView, loader },
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            bottomBar.AddTappedEvent(bottomBar.AddButton, OnAddButtonTapped);
            bottomBar.AddTappedEvent(bottomBar.SaveButton, OnSaveButtonTapped);
            bottomBar.AddTappedEvent(bottomBar.DeleteButton, OnDeleteButtonTapped);
            bottomBar.AddTappedEvent(bottomBar.DeleteAllButton, OnDeleteAllButtonTapped);

            resultList.ItemTapped += OnItemClick;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void OnItemClick(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is ScannedDocument.Page selectedPage && selectedPage != null)
            {
                Navigation.PushAsync(new ScannedDocumentDetailPage(document, selectedPage));
            }
        }

        async void OnAddButtonTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            try
            {
                using var loader = new TempLoader(this);

                document = await RTU.DocumentScanner.LaunchAsync(new DocumentScanningFlow
                {
                    DocumentUuid = document.Uuid.ToString()
                });

                resultList.ItemsSource = document.Pages;
                pageGridView.PlatformSizeChanged();
            }
            // if the cancel button is clicked
            catch (TaskCanceledException)
            {

            }
        }

        async void OnSaveButtonTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var parameters = new string[] { PDF, OCR, SandwichPDF, TIFF };
            string action = await DisplayActionSheet("Save Image as", "Cancel", null, parameters);

            if (action == null || action.Equals("Cancel"))
            {
                return;
            }

            using var loader = new TempLoader(this);
            try
            {
                switch (action)
                {
                    case PDF:
                        await GeneratePdfAsync();
                        break;
                    case OCR:
                        await PerformOcrAsync();
                        break;
                    case SandwichPDF:
                        await GenerateSandwichPdfAsync();
                        break;
                    case TIFF:
                        await GenerateTiffAsync();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ViewUtils.Alert(this, "Error: ", $"An error occurred while saving the document: {ex.Message}");
            }
        }

        private async Task GeneratePdfAsync()
        {
            var fileUri = await document.CreatePdfAsync(new PDFConfiguration
            {
                PageOrientation = PDFPageOrientation.Auto,
                PageSize = PDFPageSize.A4,
                PdfAttributes = new PDFAttributes
                {
                    Author = "Scanbot User",
                    Creator = "ScanbotSDK",
                    Title = "ScanbotSDK PDF",
                    Subject = "Generating a normal PDF",
                    Keywords = new[] { "x-platform", "ios", "android" },
                },
                Dpi = 72,
                JpegQuality = 80,
                PageFitMode = PDFPageFitMode.FitIn,
                Resample = false
            });
            ViewUtils.Alert(this, "Success: ", "Wrote pdf to: " + fileUri.AbsolutePath);
        }

        private async Task PerformOcrAsync()
        {
            // NOTE:
            // The default OCR engine is 'OcrConfig.ScanbotOCR' which is ML based. This mode doesn't expect the Langauges array.
            // If you wish to use the previous engine please use 'OcrConfig.Tesseract(...)'. The Languages array is mandatory in this mode.
            // Uncomment the below code to use the past legacy 'OcrConfig.Tesseract(...)' engine mode.
            // var ocrConfig = OcrConfig.Tesseract(withLanguageString: new List<string>{ "en", "de" });

            // Using the default OCR option
            var ocrConfig = OcrConfig.ScanbotOCR;

            var pages = document.Pages.Select(p => new FileImageSource { File = p.OriginalImageUri.LocalPath });
            var result = await CommonOperations.PerformOcrAsync(pages, configuration: ocrConfig);

            // You can access the results with: result.Pages
            ViewUtils.Alert(this, "OCR", result.Text);
        }

        private async Task GenerateSandwichPdfAsync()
        {
            // NOTE:
            // The default OCR engine is 'OcrConfig.ScanbotOCR' which is ML based. This mode doesn't expect the Langauges array.
            // If you wish to use the previous engine please use 'OcrConfig.Tesseract(...)'. The Languages array is mandatory in this mode.
            // Uncomment the below code to use the past legacy 'OcrConfig.Tesseract(...)' engine mode.
            // var ocrConfig = OcrConfig.Tesseract(withLanguageString: new List<string>{ "en", "de" });

            // Using the default OCR option
            var ocrConfig = OcrConfig.ScanbotOCR;

            var result = await CommonOperations.CreateSandwichPdfAsync(
                document.Pages.Select(p => new FileImageSource { File = p.OriginalImageUri.LocalPath }),
                new PDFConfiguration
                {
                    PageOrientation = PDFPageOrientation.Auto,
                    PageSize = PDFPageSize.A4,
                    PdfAttributes = new PDFAttributes
                    {
                        Author = "Scanbot User",
                        Creator = "ScanbotSDK",
                        Title = "ScanbotSDK PDF",
                        Subject = "Generating a sandwiched PDF",
                        Keywords = new[] { "x-platform", "ios", "android" },
                    },
                    Dpi = 72,
                    JpegQuality = 80,
                    PageFitMode = PDFPageFitMode.FitIn,
                    Resample = false
                }, ocrConfig);

            ViewUtils.Alert(this, "PDF with OCR layer stored: ", result.AbsolutePath);
        }

        private async Task GenerateTiffAsync()
        {
            var fileUri = await document.CreateTiffAsync(
                                 new TiffOptions(ParametricFilter.ScanbotBinarization(OutputMode.Binary))
                                 {
                                     Compression = TiffCompressionOptions.CompressionCcittT6,
                                     Dpi = 300,
                                     OneBitEncoded = true
                                 }
                             );
            ViewUtils.Alert(this, "Success: ", "Wrote tiff to: " + fileUri.AbsolutePath);
        }

        private async void OnDeleteAllButtonTapped(object sender, EventArgs e)
        {
            using var loader = new TempLoader(this);
            var documentCount = ScannedDocument.StoredDocumentUuids.Length;
            var message = $"Do you really want to delete all {documentCount} documents?";
            var result = await this.DisplayAlert("Attention!", message, "Yes", "No");

            if (result)
            {
                await ScannedDocument.DeleteAllDocumentsAsync();
                await DisplayAlert("Alert", $"Deleted {documentCount} documents", "OK");
                await Navigation.PushAsync(new HomePage());
            }
        }

        private async void OnDeleteButtonTapped(object sender, EventArgs e)
        {
            using var loader = new TempLoader(this);
            var message = "Do you really want to delete this document?";
            var result = await this.DisplayAlert("Attention!", message, "Yes", "No");
            if (result)
            {
                await document.DeleteAsync();
                await Navigation.PushAsync(new HomePage());
            }
        }
    }
}