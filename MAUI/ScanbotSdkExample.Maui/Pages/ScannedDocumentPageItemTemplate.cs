using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.SubViews.Cells;

public class ScannedDocumentPageItemTemplate : ContentView
{
    private ScannedDocument.Page _currentPage;

    private readonly Image _pagePreview;
    internal Action<ScannedDocument.Page> PageItemTapped;

    public ScannedDocumentPageItemTemplate()
    {
        _pagePreview = new Image
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(5),
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.White,
        };

        var parentView = new Grid
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
      
        parentView.Children.Add(_pagePreview);
        Content = parentView;
        var gestureRecognizer = new TapGestureRecognizer();
        gestureRecognizer.Tapped += ItemTapped;
        Content.GestureRecognizers.Add(gestureRecognizer);
    }

    private void ItemTapped(object sender, TappedEventArgs e)
    {
        PageItemTapped?.Invoke(_currentPage);
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