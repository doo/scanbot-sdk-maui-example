﻿using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.SubViews.ActionBar;
using ReadyToUseUI.Maui.SubViews.Cells;
using ReadyToUseUI.Maui.Utils;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Document;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReadyToUseUI.Maui.SubViews;

namespace ReadyToUseUI.Maui.Pages
{
    public class ImageResultsPage : ContentPage, INotifyPropertyChanged
    {
        private const string PDF = "PDF", OCR = "Perform OCR", SandwichPDF = "Sandwiched PDF", TIFF = "TIFF (1-bit, B&W)";
        private Grid pageGridView;
        private ListView resultList;
        private BottomActionBar bottomBar;
        private SBLoader loader;
        private ObservableCollection<IScannedPage> scannedPages = new ObservableCollection<IScannedPage>();

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
        
        public ImageResultsPage()
        {
            this.BindingContext = this;
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
            bottomBar.AddTappedEvent(bottomBar.DeleteAllButton, OnDeleteButtonTapped);

            resultList.ItemTapped += OnItemClick;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                IsLoading = true;
                var savedPages = await PageStorage.Instance.LoadAsync();

                scannedPages.Clear();

                resultList.HeightRequest = Height;
                savedPages.ForEach(scannedPages.Add);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnItemClick(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is IScannedPage selectedPage && selectedPage != null)
            {
                Navigation.PushAsync(new ImageDetailPage(selectedPage, SBSDK.SDKService));
            }
        }

        async void OnAddButtonTapped(object sender, EventArgs e)
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

        async void OnSaveButtonTapped(object sender, EventArgs e)
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

            var parameters = new string[] { PDF, OCR, SandwichPDF, TIFF};
            string action = await DisplayActionSheet("Save Image as", "Cancel", null, parameters);

            if (action == null || action.Equals("Cancel"))
            {
                return;
            }

            try
            {
                IsLoading = true;
                switch (action)
                {
                    case PDF:
                        await GeneratePdfAsync(documentSources);
                        break;
                    case OCR:
                        await PerformOcrAsync(documentSources);
                        break;
                    case SandwichPDF:
                        await GenerateSandwichPdfAsync(documentSources);
                        break;
                    case TIFF:
                        await GenerateTiffAsync(documentSources);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ViewUtils.Alert(this, "Error: ", $"An error occurred while saving the document: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }            
        }

        private async Task GeneratePdfAsync(List<ImageSource> documentSources)
        {
            var fileUri = await SBSDK.SDKService.CreatePdfAsync(documentSources.OfType<FileImageSource>(),
                        configuration: new PDFConfiguration
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
            ViewUtils.Alert(this, "Success: ", "Wrote documents to: " + fileUri.AbsolutePath);
        }

        private async Task PerformOcrAsync(List<ImageSource> documentSources)
        {
            // NOTE:
            // The default OCR engine is 'OcrConfig.ScanbotOCR' which is ML based. This mode doesn't expect the Langauges array.
            // If you wish to use the previous engine please use 'OcrConfig.Tesseract(...)'. The Languages array is mandatory in this mode.
            // Uncomment the below code to use the past legacy 'OcrConfig.Tesseract(...)' engine mode.
            // var ocrConfig = OcrConfig.Tesseract(withLanguageString: new List<string>{ "en", "de" });

            // Using the default OCR option
            var ocrConfig = OcrConfig.ScanbotOCR;

            var result = await SBSDK.SDKService.PerformOcrAsync(documentSources.OfType<FileImageSource>(), configuration: ocrConfig);

            // You can access the results with: result.Pages
            ViewUtils.Alert(this, "OCR", result.Text);
        }

        private async Task GenerateSandwichPdfAsync(List<ImageSource> documentSources)
        {
            // NOTE:
            // The default OCR engine is 'OcrConfig.ScanbotOCR' which is ML based. This mode doesn't expect the Langauges array.
            // If you wish to use the previous engine please use 'OcrConfig.Tesseract(...)'. The Languages array is mandatory in this mode.
            // Uncomment the below code to use the past legacy 'OcrConfig.Tesseract(...)' engine mode.
            // var ocrConfig = OcrConfig.Tesseract(withLanguageString: new List<string>{ "en", "de" });

            // Using the default OCR option
            var ocrConfig = OcrConfig.ScanbotOCR;

            var result = await SBSDK.SDKService.CreateSandwichPdfAsync(
                documentSources.OfType<FileImageSource>(),
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

        private async Task GenerateTiffAsync(List<ImageSource> documentSources)
        {
            var fileUri = await SBSDK.SDKService.WriteTiffAsync(
                                 documentSources.OfType<FileImageSource>(),
                                 new TiffOptions (ParametricFilter.ScanbotBinarization(OutputMode.Binary))
                                 {
                                     Compression = TiffCompressionOptions.CompressionCcittT6,
                                     Dpi = 300,
                                     OneBitEncoded = true
                                 }
                             );
            ViewUtils.Alert(this, "Success: ", "Wrote documents to: " + fileUri.AbsolutePath);
        }

        private async void OnDeleteButtonTapped(object sender, EventArgs e)
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