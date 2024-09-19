namespace ReadyToUseUI.Maui.Pages.DetectOnImageResults;

public partial class DetectOnImageResultPage : ContentPage
{
    private ImageSource resultImageSource;
    public DetectOnImageResultPage()
    {
        InitializeComponent();
    }

    internal void NavigateData(ImageSource source)
    {
        resultImageSource = source;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        resultImageView.Source = resultImageSource;
    }

    private async void BtnGoback_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(true);
    }
}