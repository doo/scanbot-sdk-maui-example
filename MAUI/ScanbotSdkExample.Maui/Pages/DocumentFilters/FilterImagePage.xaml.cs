namespace ScanbotSdkExample.Maui.Pages;

public partial class FilteredImagePage : ContentPage
{
    public ImageSource FilteredImage {
        get;
        set;
    }
    
    public FilteredImagePage()
    {
        InitializeComponent();
        BindingContext = this;
    }
    
    internal void NavigateData(ImageSource source)
    {
        FilteredImage = source;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        filteredImage.Source = FilteredImage;
    }

    private void OpenFilters_OnClicked(object sender, EventArgs e)
    {
        // todo: show picker with filter options for user selection. 
    }
}