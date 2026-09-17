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

        var gridView = new Grid
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Children = { _documentImage},
            RowDefinitions =
            [
                new RowDefinition(GridLength.Star)
            ]
        };

        gridView.SetRow(_documentImage, 0);

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

        SetupBottomToolbar();
    }
    
    private void SetupBottomToolbar()
    {
        // 1. Prominent primary buttons (Visible)
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Crop", Order = ToolbarItemOrder.Secondary, Priority = 0
        });
        
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Clean Up", Order = ToolbarItemOrder.Secondary, Priority = 1
        });

        // 2. Secondary overflow buttons (Clubbed into the "..." menu)
        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Apply Filter", Order = ToolbarItemOrder.Secondary, Priority = 2
        });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Check Quality", Order = ToolbarItemOrder.Secondary, Priority = 3
        });

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Delete", Order = ToolbarItemOrder.Secondary, Priority = 4
        });

        // 3. Bind your existing click events securely
        ToolbarItems[0].Clicked += OnCropButtonTapped;
        ToolbarItems[1].Clicked += OnCleanUpDocumentTapped;
        ToolbarItems[2].Clicked += OnFilterButtonTapped;
        ToolbarItems[3].Clicked += OnAnalyzeQualityTapped;
        ToolbarItems[4].Clicked += OnDeleteButtonTapped;
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
        var result = await ScanbotSDKMain.Document.StartCroppingScreenAsync(new CroppingStandaloneConfiguration
        {
            DocumentUuid = _selectedDocument.Uuid,
            PageUuid = _selectedPage.Uuid
        });

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

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

    // @Tag("Document Quality Analyzer")
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
    // @EndTag("Document Quality Analyzer")

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

    private async void OnCleanUpDocumentTapped(object sender, EventArgs e)
    {
        var configuration = new DocumentCleanupStandaloneConfiguration
        {
            DocumentUuid = _selectedDocument.Uuid,
            PageUuid = _selectedPage.Uuid
        };

        // If true, the cleanup tool will not allow erasing text. But it takes some time to process OCR on the image initially,
        configuration.Cleanup.EngineConfiguration.KeepText = false;

        // The maximum number of undo/redo operations that can be performed. Make it less to save up memory
        configuration.Cleanup.EngineConfiguration.MaxUndoRedoStackSize = 4;

        // Downscales stroke area to this value in pixels (width x height) to speed up the cleanup process. The smaller the value the faster but quality will be lower too.
        configuration.Cleanup.EngineConfiguration.MaxCleanupResolution = 1_200_000;

        // Customize the top bar.
        configuration.Cleanup.TopBarBackButton.Text = "Cancel";
        configuration.Cleanup.TopBarConfirmButton.Text = "Done";
        configuration.Cleanup.TopBarTitle.Text = "Clean up the page";

        // background color
        configuration.Cleanup.BackgroundColor = new ColorValue("#222222");

        // Configure the toolbar buttons (undo / redo / reset).
        configuration.Cleanup.Toolbar.UndoButton.Title.Text = "Undo";
        configuration.Cleanup.Toolbar.RedoButton.Title.Text = "Redo";
        configuration.Cleanup.Toolbar.ResetButton.Title.Text = "Reset";

        configuration.Cleanup.Toolbar.StrokeSizeSlider.Visible = true;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.MinStrokeSize = 1;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.MaxStrokeSize = 50;
        configuration.Cleanup.Toolbar.StrokeSizeSlider.Title.Text = "Brush size";

        // Optional: show an introduction screen the first time the user opens cleanup.
        configuration.Cleanup.Introduction.ShowAutomatically = true;

        // Customize the alert dialogs shown for Reset and for cancelling with unsaved changes.
        configuration.Cleanup.ResetAllEditsAlertDialog.Title.Text = "Reset all edits?";
        configuration.Cleanup.ResetAllEditsAlertDialog.Subtitle.Text = "This will revert all cleanup operations on this page.";

        configuration.Cleanup.DiscardChangesAlertDialog.Title.Text = "Discard changes?";
        configuration.Cleanup.DiscardChangesAlertDialog.Subtitle.Text = "Your cleanup edits on this page will be lost.";

        var result = await ScanbotSDKMain.DocumentEnhancer.StartDocumentCleanupScreenAsync(configuration);

        if (result.IsCanceled) return;

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
        }
    }
}