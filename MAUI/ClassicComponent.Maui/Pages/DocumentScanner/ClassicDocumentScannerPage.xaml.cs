// using System.Diagnostics;
// using ClassicComponent.Maui.Pages;
// using ScanbotSDK.MAUI;
// using ScanbotSDK.MAUI.ClassicComponents;
// using ScanbotSDK.MAUI.Document;
//

namespace ClassicComponent.Maui;

public partial class ClassicDocumentScannerPage : ContentPage //, IClassicDocumentScannerViewInteraction
{
// 	public const string Start = "Start";
// 	public const string Stop = "Stop";
// 	public const string AutoSnap = "Auto Snap";
// 	public const string Flash = "Flash";
// 	public const string Polygons = "Polygons";
// 	
// 	private bool _autoSnappingEnabled;
// 	public bool AutoSnappingEnabled
// 	{
// 		get => _autoSnappingEnabled;
// 		set
// 		{
// 			_autoSnappingEnabled = value;
// 			OnPropertyChanged(nameof(AutoSnappingEnabled));
// 		}
// 	}
// 	
// 	private bool _flashEnabled;
// 	public bool IsFlashEnabled
// 	{
// 		get => _flashEnabled;
// 		set
// 		{
// 			_flashEnabled = value;
// 			OnPropertyChanged(nameof(IsFlashEnabled));
// 		}
// 	}
// 	
// 	private bool _polygonViewEnabled;
// 	public bool PolygonViewEnabled
// 	{
// 		get => _polygonViewEnabled;
// 		set
// 		{
// 			_polygonViewEnabled = value;
// 			OnPropertyChanged(nameof(PolygonViewEnabled));
// 		}
// 	}
//
	public ClassicDocumentScannerPage()
	{
		InitializeComponent();
// 		this.BindingContext = this;
// 		documentScannerView.ViewInteraction = this;
// 		documentScannerView.PolygonColor = Colors.DeepPink;
// 		documentScannerView.PolygonColorOk  = Colors.DeepPink;
// 		
// 		documentScannerView.PolygonBackgroundColor = Colors.PaleVioletRed;
// 		documentScannerView.PolygonBackgroundColorOk = Colors.PaleVioletRed;
//
// 		documentScannerView.PolygonCornerRadius = 20;
// 		documentScannerView.PolygonLineWidth = 5;
// 		documentScannerView.PolygonAutoSnapProgressEnabled = true;
// 		documentScannerView.PolygonAutoSnapProgressColor = Colors.SaddleBrown;
	}
//
// 	public void DidDetectDocument(ImageSource documentImage, ImageSource originalImage)
// 	{
// 		var resultsPage = new DocumentScannerResultPage();
// 		resultsPage.NavigateData(documentImage);
// 		Navigation.PushAsync(resultsPage);
// 	}
//
// 	public void UpdateDetectionHintFromStatus(DocumentDetectionStatus status)
// 	{
// 		Debug.WriteLine("Document Detection Status: " + status);
// 	}
//
// 	private void DocumentScanner_ButtonClicked(object sender, EventArgs e)
// 	{
// 		var button = sender as Button;
// 		switch (button.Text)
// 		{
// 			case AutoSnap:
// 				AutoSnappingEnabled = !AutoSnappingEnabled;
// 				UpdateButton(button, AutoSnappingEnabled);
// 				break;
// 			
// 			case Flash:
// 				IsFlashEnabled = !IsFlashEnabled;
// 				UpdateButton(button, IsFlashEnabled);
// 				break;
// 			
// 			case Polygons:
// 				PolygonViewEnabled = !PolygonViewEnabled;
// 				UpdateButton(button, PolygonViewEnabled);
// 				break;
// 			
// 			case Start:
// 				documentScannerView.UnFreezeCamera();
// 				button.Text = Stop;
// 				break;
// 			
// 			case Stop:
// 				documentScannerView.FreezeCamera();
// 				button.Text = Start;
// 				break;
// 		}
// 	}
//
// 	private void UpdateButton(Button button, bool enabled)
// 	{
// 		if (enabled)
// 		{
// 			
// 		}
// 	}
}