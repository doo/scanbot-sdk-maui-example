using ScanbotSDK.MAUI.Services;

namespace ReadyToUseUI.Maui.SubViews.Cells
{
    public class ImageResultCell : ViewCell
    {
        public IScannedPage Source { get; private set; }

        Image Document { get; set; }

        public ImageResultCell()
        {
            Document = new Image
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(10, 10, 10, 10),
                Aspect = Aspect.AspectFit,
                WidthRequest = 100
            };

            var parentView = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
            };
            parentView.Children.Add(Document);
            View = parentView;
        }

        protected override async void OnBindingContextChanged()
        {
            if (BindingContext == null)
            {
                return;
            }
            Source = (IScannedPage)BindingContext;
            // If encryption is enabled, load the decrypted document.
            // Else accessible via page.Document
            Document.Source = await Source.DecryptedDocumentPreview();
            //Document.Source = Source.DocumentPreview;
            base.OnBindingContextChanged();
        }
    }
}

