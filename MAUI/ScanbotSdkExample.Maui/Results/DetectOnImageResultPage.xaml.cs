namespace ScanbotSdkExample.Maui.Results;

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
        ResultImageView.Source = _resultImageSource;
    }

    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(true);
    }
}