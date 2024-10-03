using ScanbotSDK.MAUI;

namespace ClassicComponent.Maui.Pages;

public partial class DocumentScannerResultPage : ContentPage
{
	private ImageSource _imageSource;
	
	internal void NavigateData(ImageSource source)
	{
		_imageSource = source;
	}
	
	public DocumentScannerResultPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		imagePreview.Source = _imageSource;
	}
}