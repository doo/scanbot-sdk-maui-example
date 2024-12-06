using System.ComponentModel;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Document.Legacy;
using ReadyToUseUI.Maui.Utils;
using ReadyToUseUI.Maui.SubViews.ActionBar;
using ReadyToUseUI.Maui.SubViews;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ReadyToUseUI.Maui.Pages
{
    public class ScannedDocumentDetailPage : ContentPage
    {
        private Image documentImage;
        private BottomActionBar bottomBar;
        private SBLoader loader;

        private ScannedDocument selectedDocument;
        private ScannedDocument.Page selectedPage;
        
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                loader.IsBusy = _isLoading;
                this.OnPropertyChanged(nameof(IsLoading));
            }
        }
        
        public ScannedDocumentDetailPage(ScannedDocument selectedDocument, ScannedDocument.Page selectedPage)
        {
            this.selectedDocument = selectedDocument;
            this.selectedPage = selectedPage;

            documentImage = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray,
                Aspect = Aspect.AspectFit,
            };
            documentImage.SizeChanged += delegate
            {
                // Don't allow images larger than 2/3 of the screen
                documentImage.HeightRequest = Content.Height / 3 * 2;
            };

            bottomBar = new BottomActionBar(isDetailPage: true);

            var gridView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Children = { documentImage, bottomBar },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                }
            };

            gridView.SetRow(documentImage, 0);
            gridView.SetRow(bottomBar, 1);

            loader = new SBLoader
            {
                IsVisible = false
            };
            
            Content = new Grid
            {
                Children = { gridView, loader },
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

            documentImage.Source = selectedPage.DocumentImagePreviewUri.ToImageSource();
        }

        private async void OnCropButtonTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            try
            {
                var result = await RTU.CroppingScreen.LaunchAsync(
                    new CroppingConfiguration() 
                    {
                        DocumentUuid = selectedDocument.Uuid.ToString(),
                        PageUuid = selectedPage.Uuid.ToString()
                    });
                documentImage.Source = selectedPage.DocumentImagePreviewUri.ToImageSource();
            }
            catch (TaskCanceledException)
            {

            }
        }


        private async void OnFilterButtonTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this))
            {
                return;
            }

            IsLoading = true;
            var filterPage = new FiltersPage();
            filterPage.NavigateData(async (filters) =>
            {
                documentImage.Source = null;
                selectedPage = await selectedPage.ModifyPageAsync(filters: filters);
                documentImage.Source = selectedPage.DocumentImagePreviewUri.ToImageSource();
            });            
            await Navigation.PushAsync(filterPage);
            IsLoading = false;
        }

        private async void OnAnalyzeQualityTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }
            IsLoading = true;
            DocumentQuality quality = await CommonOperations.DetectDocumentQualityAsync(selectedPage.DocumentImage, new DocumentQualityAnalyzerConfiguration
            {
                ImageSizeLimit = 2500,
                MinimumNumberOfSymbols = 20
            });
            
            IsLoading = false;
            ViewUtils.Alert(this, "Document Quality", $"Detected quality is: {quality}");
        }

        private async void OnDeleteButtonTapped(object sender, EventArgs e)
        {
            var message = "Do you really want to delete this page?";
            var result = await this.DisplayAlert("Attention!", message, "Yes", "No");
            if (result)
            {
                await selectedDocument.RemovePageAsync(selectedPage);
                await Navigation.PushAsync(new ScannedDocumentsPage(selectedDocument));
            }
        }
    }
}