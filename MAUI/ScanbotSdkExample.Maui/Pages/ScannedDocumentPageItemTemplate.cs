using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.SubViews.Cells;

public class ScannedDocumentPageItemTemplate : ViewCell
{
    private ScannedDocument.Page _currentPage;

    private readonly Image _pagePreview;

    public ScannedDocumentPageItemTemplate()
    {
        _pagePreview = new Image
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
        parentView.Children.Add(_pagePreview);
        View = parentView;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext == null)
        {
            return;
        }
        _currentPage = (ScannedDocument.Page)BindingContext;
        // If encryption is enabled, load the decrypted document.
        // Else accessible via page.Document
        // Document.Source = await Source.DecryptedDocumentPreview();

        _pagePreview.Source = _currentPage.DocumentImagePreviewUri.ToImageSource();
    }
}