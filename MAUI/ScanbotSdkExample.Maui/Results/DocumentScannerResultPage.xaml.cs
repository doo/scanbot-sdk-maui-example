namespace ScanbotSdkExample.Maui.Results;

public partial class DocumentScannerResultPage : ContentPage
{
	private ImageSource _imageSource;
	
	internal void SetData(ImageSource source)
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
		ImagePreview.Source = _imageSource;
	}
}