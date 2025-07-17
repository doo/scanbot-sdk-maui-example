using static ScanbotSDK.MAUI.ScanbotSDKMain;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSdkExample.Maui.SubViews;
using ScanbotSdkExample.Maui.SubViews.ActionBar;
using ScanbotSdkExample.Maui.SubViews.Cells;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Pages;
public class ScannedDocumentsPage : ContentPage
{
    private const string Pdf = "PDF", Ocr = "Perform OCR", SandwichPdf = "Sandwiched PDF", Tiff = "TIFF (1-bit, B&W)";
    private readonly Grid _pageGridView;
    private readonly CollectionView _resultList;
    private readonly SBLoader _loader;

    private ScannedDocument _document;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            _loader.IsBusy = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    private readonly struct TempLoader : IDisposable
    {
        private readonly ScannedDocumentsPage _page;
        public TempLoader(ScannedDocumentsPage page)
        {
            _page = page;
            page.IsLoading = true;
        }

        public void Dispose()
        {
            _page.IsLoading = false;
        }
    }

    public ScannedDocumentsPage(ScannedDocument document)
    {
        _document = document;
        Title = "Document";
        BackgroundColor = Colors.White;
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        var itemSize = (screenWidth - 30) / 2; 
        _resultList = new CollectionView
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            
            ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical)
            {
                HorizontalItemSpacing = 5,
                VerticalItemSpacing = 5
            },
            FlowDirection = FlowDirection.MatchParent,
            ItemTemplate = new DataTemplate(() => new ScannedDocumentPageItemTemplate
            {
                WidthRequest = itemSize,
                HeightRequest = itemSize,
                PageItemTapped = OnSelectDocumentPage
            }),
            ItemsSource = document.Pages
        };
        
        var bottomBar = new BottomActionBar(isDetailPage: false)
        {
            VerticalOptions = LayoutOptions.End
        };

        _pageGridView = new Grid
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { _resultList, bottomBar },
            RowDefinitions =
            [
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            ]
        };

        _pageGridView.SetRow(_resultList, 0);
        _pageGridView.SetRow(bottomBar, 1);

        _loader = new SBLoader
        {
            IsVisible = false
        };

        Content = new Grid
        {
            Children = { _pageGridView, _loader },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        };

        bottomBar.AddTappedEvent(bottomBar.AddButton, OnAddButtonTapped);
        bottomBar.AddTappedEvent(bottomBar.SaveButton, OnSaveButtonTapped);
        bottomBar.AddTappedEvent(bottomBar.DeleteButton, OnDeleteButtonTapped);
        bottomBar.AddTappedEvent(bottomBar.DeleteAllButton, OnDeleteAllButtonTapped);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _resultList.ItemsSource = _document.Pages;
    }

    private void OnSelectDocumentPage(ScannedDocument.Page selectedPage)
    {
        if (selectedPage == null) return;
        Navigation.PushAsync(new ScannedDocumentDetailPage(_document, selectedPage));
    }

    async void OnAddButtonTapped(object sender, EventArgs e)
    {
        if (!SdkUtils.CheckLicense(this)) { return; }

        try
        {
            using var loader = new TempLoader(this);
            var result = await Rtu.DocumentScanner.LaunchAsync(new DocumentScanningFlow
            {
                DocumentUuid = _document.Uuid.ToString()
            });
            
            _document = result.Result; 
            _resultList.ItemsSource = _document.Pages;
            _pageGridView.PlatformSizeChanged();
        }
        // if the cancel button is clicked
        catch (TaskCanceledException)
        {

        }
    }

    async void OnSaveButtonTapped(object sender, EventArgs e)
    {
        if (!SdkUtils.CheckLicense(this) || !_document.Pages.Any()) { return; }

        var parameters = new [] { Pdf, Ocr, SandwichPdf, Tiff };
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
                case Pdf:
                    await GeneratePdfAsync();
                    break;
                case Ocr:
                    await PerformOcrAsync();
                    break;
                case SandwichPdf:
                    await GenerateSandwichPdfAsync();
                    break;
                case Tiff:
                    await GenerateTiffAsync();
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
        var fileUri = await _document.CreatePdfAsync(new PdfConfiguration
        {
            PageDirection = PageDirection.Auto,
            PageSize = PageSize.A4,
            Attributes = new PdfAttributes
            {
                Author = "Scanbot User",
                Creator = "ScanbotSDK",
                Title = "ScanbotSDK PDF",
                Subject = "Generating a normal PDF",
                Keywords = "x-platform, ios, android"
            },
            Dpi = 72,
            JpegQuality = 80,
            PageFit = PageFit.FitIn,
            ResamplingMethod = ResamplingMethod.None
        });
        
        // Sharing the Pdf.
        await ShareFileAsync(fileUri.LocalPath, "application/pdf");
    }

    private async Task PerformOcrAsync()
    {
        // NOTE:
        // The default OCR engine is 'OcrConfig.ScanbotOCR' which is ML based. This mode doesn't expect the Langauges array.
        // If you wish to use the previous engine please use 'OcrConfig.Tesseract(...)'. The Languages array is mandatory in this mode.
        // Uncomment the below code to use the past legacy 'OcrConfig.Tesseract(...)' engine mode.
        // var ocrConfig = OcrConfig.Tesseract(withLanguageString: new List<string>{ "en", "de" });

        // Using the default OCR option
        var ocrConfig = ScanbotSDK.MAUI.Ocr.OcrConfig.ScanbotOcr;

        var pages = _document.Pages.Select(p => new FileImageSource { File = p.OriginalImageUri.LocalPath });
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
        var ocrConfig = ScanbotSDK.MAUI.Ocr.OcrConfig.ScanbotOcr;

        var result = await CommonOperations.CreateSandwichPdfAsync(
            _document.Pages.Select(p => new FileImageSource { File = p.OriginalImageUri.LocalPath }),
            new PdfConfiguration
            {
                PageDirection = PageDirection.Auto,
                PageSize = PageSize.A4,
                Attributes = new PdfAttributes
                {
                    Author = "Scanbot User",
                    Creator = "ScanbotSDK",
                    Title = "ScanbotSDK PDF",
                    Subject = "Generating a sandwiched PDF",
                    Keywords = "x-platform, ios, android"
                },
                Dpi = 72,
                JpegQuality = 80,
                PageFit = PageFit.FitIn,
                ResamplingMethod = ResamplingMethod.None
            }, ocrConfig);

        // Sharing the Pdf.
        await ShareFileAsync(result.LocalPath, "application/pdf");
    }

    private async Task GenerateTiffAsync()
    {
        var fileUri = await _document.CreateTiffAsync(new TiffGeneratorParameters
            {
                Compression = CompressionMode.CcittT6,
                Dpi = 72,
                JpegQuality = 80,
                BinarizationFilter = ParametricFilter.ScanbotBinarization(OutputMode.Binary),
                ZipCompressionLevel = 6,
            }
        );

        // Sharing the Tiff file.
        await ShareFileAsync(fileUri.LocalPath, "image/tiff");
    }

    private async void OnDeleteAllButtonTapped(object sender, EventArgs e)
    {
        var documentCount = ScannedDocument.StoredDocumentUuids.Length;
        var message = "This will delete the current document that contains all the pages visible on the screen along with any previous documents found on the local storage.";
        var result = await DisplayAlert("Attention!", message, "Confirm", "Cancel");
        if (!result) return;
        
        using var loader = new TempLoader(this);
        await ScannedDocument.DeleteAllDocumentsAsync();
        await DisplayAlert("Alert", $"Deleted {documentCount} documents", "OK");
        await Navigation.PopAsync(true);
    }

    private async void OnDeleteButtonTapped(object sender, EventArgs e)
    {
        var message = "This will delete the current document that contains all the pages visible on the screen.";
        var result = await DisplayAlert("Attention!", message, "Confirm", "Cancel");
        if (!result) return;
        using var loader = new TempLoader(this);
        await _document.DeleteAsync();
        await Navigation.PopAsync(true);
    }

    public async Task ShareFileAsync(string localFilePath, string contentType)
    {
        if (string.IsNullOrEmpty(localFilePath) || !File.Exists(localFilePath))
        {
            // Handle file-not-found scenario
            ViewUtils.Alert(this, "Error", "Unable to find the file:" + localFilePath);
            return;
        }

        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(new ShareFileRequest
        {
            Title = "Share PDF",
            File = new ShareFile(localFilePath, contentType ?? string.Empty)
        });
    }
}