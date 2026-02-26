using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Core.DocumentQualityAnalyzer;
using ScanbotSDK.MAUI.Core.ImageProcessing;
using ScanbotSDK.MAUI.Document;
using ScanbotSdkExample.Maui.Controls;
using ScanbotSdkExample.Maui.Controls.ActionBar;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Results;

public class ScannedDocumentDetailPage : ContentPage
{
    private readonly Image _documentImage;
    private readonly SBLoader _loader;

    private readonly IScannedDocument _selectedDocument;
    private IScannedDocument.IPage _selectedPage;

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            _loader.IsBusy = _isLoading;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public ScannedDocumentDetailPage(IScannedDocument selectedDocument, IScannedDocument.IPage selectedPage)
    {
        _selectedDocument = selectedDocument;
        _selectedPage = selectedPage;
        Title = "Page";
        _documentImage = new Image
        {
            HorizontalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.LightGray,
            Aspect = Aspect.AspectFit,
        };
        _documentImage.SizeChanged += delegate
        {
            // Don't allow images larger than 2/3 of the screen
            _documentImage.HeightRequest = Content.Height / 3 * 2;
        };

        var bottomBar = new BottomActionBar(isDetailPage: true);

        var gridView = new Grid
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { _documentImage, bottomBar },
            RowDefinitions =
            [
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            ]
        };

        gridView.SetRow(_documentImage, 0);
        gridView.SetRow(bottomBar, 1);

        _loader = new SBLoader
        {
            IsVisible = false
        };

        Content = new Grid
        {
            Children = { gridView, _loader },
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill
        };

        bottomBar.AddTappedEvent(bottomBar.CropButton, OnCropButtonTapped);
        bottomBar.AddTappedEvent(bottomBar.FilterButton, OnFilterButtonTapped);
        bottomBar.AddTappedEvent(bottomBar.AnalyzeQualityButton, OnAnalyzeQualityTapped);
        bottomBar.AddTappedEvent(bottomBar.DeleteButton, OnDeleteButtonTapped);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _documentImage.Source = _selectedPage.DocumentImagePreview.ToImageSource();
    }

    private async void OnCropButtonTapped(object sender, EventArgs e)
    {
        if (!App.IsLicenseValid)
        {
            return;
        }

        // @Tag("Cropping UI")
        var result = await ScanbotSDKMain.Document.StartCroppingScreenAsync(new CroppingConfiguration
        {
            DocumentUuid = _selectedDocument.Uuid.ToString(),
            PageUuid = _selectedPage.Uuid.ToString()
        });

        // error
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        // success
        _documentImage.Source = _selectedPage.DocumentImagePreview.ToImageSource();
        // @EndTag("Cropping UI")
    }

    private async void OnFilterButtonTapped(object sender, EventArgs e)
    {
        if (!App.IsLicenseValid) return;

        IsLoading = true;
        var filterPage = new FiltersPage();
        filterPage.NavigateData(ApplyFilterHandler);

        await Navigation.PushAsync(filterPage);
        IsLoading = false;
    }

    private async void ApplyFilterHandler(ParametricFilter[] filters)
    {
        _documentImage.Source = null;
        var result = await _selectedPage.ModifyPageAsync(new ModifyPageOptions
        {
            Filters = filters
        });

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        _selectedPage = result.Value;
        _documentImage.Source = _selectedPage.DocumentImagePreview.ToImageSource();
    }

    private async void OnAnalyzeQualityTapped(object sender, EventArgs e)
    {
        if (!App.IsLicenseValid) return;
        IsLoading = true;
        var result = await ScanbotSDKMain.Document.AnalyzeQualityOnImageAsync(_selectedPage.DocumentImage, new DocumentQualityAnalyzerConfiguration
        {
            MaxImageSize = 2500,
            MinEstimatedNumberOfSymbolsForDocument = 20
        });

        IsLoading = false;
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        await Alert.ShowAsync("Document Quality", $"Detected quality is: {result.Value.Quality}");
    }

    private async void OnDeleteButtonTapped(object sender, EventArgs e)
    {
        var message = "Do you really want to delete this page?";
        var result = await Alert.ShowAsync("Attention!", message, "Yes", "No");
        if (result)
        {
            var removeResult = await _selectedDocument.RemovePagesAsync([_selectedPage.Uuid]);
            if (!removeResult.IsSuccess)
            {
                await Alert.ShowAsync(removeResult.Error);
                return;
            }
            await Navigation.PopAsync(true);
        }
    }
}