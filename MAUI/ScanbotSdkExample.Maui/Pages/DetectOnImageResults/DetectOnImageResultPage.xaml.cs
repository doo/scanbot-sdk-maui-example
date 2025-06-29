namespace ScanbotSdkExample.Maui.Pages.DetectOnImageResults;

public partial class DetectOnImageResultPage : ContentPage
{
    private ImageSource _resultImageSource;
    public DetectOnImageResultPage()
    {
        InitializeComponent();
    }

    internal void NavigateData(ImageSource source)
    {
        _resultImageSource = source;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        resultImageView.Source = _resultImageSource;
    }

    private async void BtnGoback_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(true);
    }
}