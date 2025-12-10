using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSdkExample.Maui.Controls;
using ScanbotSdkExample.Maui.Controls.ActionBar;
using ScanbotSdkExample.Maui.Utils;
using static ScanbotSDK.MAUI.ScanbotSDKMain;
using DocumentQualityAnalyzerConfiguration = ScanbotSDK.MAUI.DocumentQualityAnalyzerConfiguration;

namespace ScanbotSdkExample.Maui.Results;

public class ScannedDocumentDetailPage : ContentPage
{
    private readonly Image _documentImage;
    private readonly SBLoader _loader;

    private readonly ScannedDocument _selectedDocument;
    private ScannedDocument.Page _selectedPage;
        
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
        
    public ScannedDocumentDetailPage(ScannedDocument selectedDocument, ScannedDocument.Page selectedPage)
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

        _documentImage.Source = ImageSource.FromStream(() => _selectedPage.DocumentImagePreview.AsStream(ImageFormat.Jpeg, 0.7f)); 
    }

    private async void OnCropButtonTapped(object sender, EventArgs e)
    {
        if (!SdkUtils.CheckLicense(this)) { return; }

        var croppingOutput = await Rtu.CroppingScreen.LaunchAsync(new CroppingConfiguration
        {
            DocumentUuid = _selectedDocument.Uuid.ToString(),
            PageUuid = _selectedPage.Uuid.ToString()
        });
        
        if (croppingOutput.Status == OperationResult.Ok)
        {
            _documentImage.Source = ImageSource.FromStream(() => _selectedPage.DocumentImagePreview.AsStream(ImageFormat.Jpeg, 0.7f));
        }
    }

    private async void OnFilterButtonTapped(object sender, EventArgs e)
    {
        if (!SdkUtils.CheckLicense(this))
        {
            return;
        }

        IsLoading = true;
        var filterPage = new FiltersPage();
        filterPage.NavigateData(async filters =>
        {
            _documentImage.Source = null;
            _selectedPage = await _selectedPage.ModifyPageAsync(filters: filters);
            _documentImage.Source =  ImageSource.FromStream(() => _selectedPage.DocumentImagePreview.AsStream(ImageFormat.Jpeg, 0.7f));
        });            
        await Navigation.PushAsync(filterPage);
        IsLoading = false;
    }

    private async void OnAnalyzeQualityTapped(object sender, EventArgs e)
    {
        if (!SdkUtils.CheckLicense(this)) { return; }
        IsLoading = true;
        var quality = await CommonOperations.DetectDocumentQualityAsync(_selectedPage.DocumentImage, new DocumentQualityAnalyzerConfiguration
        {
            MaxImageSize = 2500,
            MinEstimatedNumberOfSymbolsForDocument = 20
        });
            
        IsLoading = false;
        Alert.Show("Document Quality", $"Detected quality is: {quality.Quality}");
    }

    private async void OnDeleteButtonTapped(object sender, EventArgs e)
    {
        var message = "Do you really want to delete this page?";
        var result = await DisplayAlertAsync("Attention!", message, "Yes", "No");
        if (result)
        {
            await _selectedDocument.RemovePageAsync(_selectedPage);
            await Navigation.PopAsync(true);
        }
    }
}