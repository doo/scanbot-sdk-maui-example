using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;


namespace ScanbotSdkExample.Maui.SubViews.Cells
{
    public class ScannedDocumentPageItemTemplate : ViewCell
    {
        private ScannedDocument.Page currentPage;

        private Image pagePreview;

        public ScannedDocumentPageItemTemplate()
        {
            pagePreview = new Image
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
            parentView.Children.Add(pagePreview);
            View = parentView;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext == null)
            {
                return;
            }
            currentPage = (ScannedDocument.Page)BindingContext;
            // If encryption is enabled, load the decrypted document.
            // Else accessible via page.Document
            // Document.Source = await Source.DecryptedDocumentPreview();

            pagePreview.Source = currentPage.DocumentImagePreviewUri.ToImageSource();
        }
    }
}

