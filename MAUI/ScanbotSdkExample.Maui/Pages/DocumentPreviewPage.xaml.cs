namespace ScanbotSdkExample.Maui.Pages;

public partial class DocumentPreviewPage : ContentPage
{
    public ImageSource DocPreviewSource {
        get;
        set;
    }
    
    public DocumentPreviewPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DocPreviewImage.Source = DocPreviewSource;
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync(true);
    }
}