using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Controls;

public class ScannedDocumentPageView : ContentView
{
    private IScannedDocument.IPage _currentPage;

    private readonly Image _pagePreview;
    internal Action<IScannedDocument.IPage> PageItemTapped;

    public ScannedDocumentPageView()
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
        _currentPage = (IScannedDocument.IPage)BindingContext;

        _pagePreview.Source = _currentPage.DocumentImagePreview.ToImageSource();
    }
}