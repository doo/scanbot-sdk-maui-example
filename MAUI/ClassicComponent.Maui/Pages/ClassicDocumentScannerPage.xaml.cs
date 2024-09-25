using ClassicComponent.Maui.Pages;

namespace ClassicComponent.Maui;

public partial class ClassicDocumentScannerPage : ContentPage
{
	public ClassicDocumentScannerPage()
	{
		InitializeComponent();
	}
	
	private void DocumentScannerView_OnDocumentImageSnapped(ImageSource documentImage, ImageSource originalImage)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.NavigateData(documentImage);
		Navigation.PushAsync(resultsPage);
	}
	
	private void Button_StartCameraClicked(object? sender, EventArgs e)
	{
		documentScannerView.UnFreezeCamera();
	}
	
	private void Button_StopCameraClicked(object? sender, EventArgs e)
	{
		documentScannerView.FreezeCamera();
	}
}