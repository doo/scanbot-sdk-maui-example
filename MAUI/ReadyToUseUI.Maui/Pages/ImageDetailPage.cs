﻿using ScanbotSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Utils;
using ReadyToUseUI.Maui.SubViews.ActionBar;
using ScanbotSDK.MAUI.Services;
using ReadyToUseUI.Maui.Models;

namespace ReadyToUseUI.Maui.Pages
{
    public class ImageDetailPage : ContentPage
    {
        private Image documentImage;
        private BottomActionBar bottomBar;
        private IScannedPage selectedPage;
        private IScanbotSDKService scanbotDocumentService; // for document quality detection

        public ImageDetailPage(IScannedPage selectedPage, IScanbotSDKService service)
        {
            this.selectedPage = selectedPage;
            this.scanbotDocumentService = service;

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
            Content = gridView;

            bottomBar.AddTappedEvent(bottomBar.CropButton, OnCropButtonTapped);
            bottomBar.AddTappedEvent(bottomBar.FilterButton, OnFilterButtonTapped);
            bottomBar.AddTappedEvent(bottomBar.AnalyzeQualityButton, OnAnalyzeQualityTapped);
            bottomBar.AddTappedEvent(bottomBar.DeleteButton, OnDeleteButtonTapped);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            documentImage.Source = await PageDocument();
        }
        
        private async Task<ImageSource> PageDocument() =>  await selectedPage.DecryptedDocument();

        private async void OnCropButtonTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var result = await ScanbotSDK.MAUI.ScanbotSDK.ReadyToUseUIService.LaunchCroppingScreenAsync(selectedPage);

            if (result.Status == OperationResult.Ok)
            {
                documentImage.Source = null;
                await PageStorage.Instance.UpdateAsync(selectedPage);
                documentImage.Source = await PageDocument();
            }
        }


        private async void OnFilterButtonTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            var buttons = Enum.GetNames(typeof(ImageFilter));
            var action = await DisplayActionSheet("Filter", "Cancel", null, buttons);

            if (Enum.TryParse<ImageFilter>(action, out var filter))
            {
                documentImage.Source = null;
                await selectedPage.SetFilterAsync(filter);
                await PageStorage.Instance.UpdateAsync(selectedPage);

                documentImage.Source = await PageDocument();
            }
        }

        private async void OnAnalyzeQualityTapped(object sender, EventArgs e)
        {
            if (!SDKUtils.CheckLicense(this)) { return; }

            DocumentQuality quality = await scanbotDocumentService.DetectDocumentQualityAsync(documentImage.Source);

            ViewUtils.Alert(this, "Document Quality", $"Detected quality is: {quality}");
        }

        private async void OnDeleteButtonTapped(object sender, EventArgs e)
        {
            documentImage.Source = null;
            await Task.WhenAll(PageStorage.Instance.DeleteAsync(selectedPage), Navigation.PopAsync());
            selectedPage = null;
        }
    }
}