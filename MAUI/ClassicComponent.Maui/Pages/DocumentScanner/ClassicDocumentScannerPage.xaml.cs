using System.Diagnostics;
using ClassicComponent.Maui.Models;
using ClassicComponent.Maui.Pages;
using ScanbotSDK.MAUI.Document.ClassicComponent;
using ScanbotSDK.MAUI.Document;

namespace ClassicComponent.Maui;

public partial class ClassicDocumentScannerPage : ContentPage, IClassicDocumentScannerViewInteraction
{
    public ClassicDocumentScannerPage()
	{
		InitializeComponent();
		DocumentScannerView.ViewInteraction = this;
		
		// ==> Polygon Configuration: Uncomment below code for 
		// DocumentScannerView.PolygonColor = Colors.Red;
		// DocumentScannerView.PolygonColorOK = Colors.Yellow;
		// DocumentScannerView.PolygonBackgroundColor = Colors.PaleVioletRed;
		// DocumentScannerView.PolygonBackgroundColorOK = Colors.MediumPurple;
		// DocumentScannerView.PolygonAutoSnapProgressColor = Colors.Black;
		//
		// DocumentScannerView.PolygonAutoSnapProgressEnabled = true;
		// DocumentScannerView.PolygonCornerRadius = 20;
		// DocumentScannerView.PolygonLineWidth = 5;

		// ==> For Finder Configuration: Uncomment below code for 
		// DocumentScannerView.FinderLineColor = Colors.Yellow;
		// DocumentScannerView.FinderAspectRatio = AspectRatio.A4DocumentPortraitAspectRatio;
		// DocumentScannerView.FinderOverlayColor = Colors.Black.WithAlpha(0.6f);
		// DocumentScannerView.FinderLineWidth = 10;
		// DocumentScannerView.FinderMinimumPadding = 10;
		// // iOS only
		// DocumentScannerView.FinderCornerRadius = 20;
		
		// CollectionView Buttons
		ScannerButtons = new List<ClassicCollectionItem>
		{
			new(AutoSnap, () => IsAutoSnappingEnabled = !IsAutoSnappingEnabled),
			new(Finder, () => IsFinderEnabled = !IsFinderEnabled),
			new(Flash, () => IsFlashEnabled = !IsFlashEnabled),
			new(Polygons, () => IsPolygonEnabled = !IsPolygonEnabled),
			new(Visibility, () => IsCameraVisible = !IsCameraVisible),
			new(Stop, null, true),
		};
		
		this.BindingContext = this;
	}

	// Receives the result of Document Scanning. 
	public void DidDetectDocument(ImageSource documentImage, ImageSource originalImage)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.NavigateData(documentImage);
		Navigation.PushAsync(resultsPage);
	}

	public void UpdateDetectionHintFromStatus(DocumentDetectionStatus status)
	{
		Debug.WriteLine("Document Detection Status: " + status);
		var hint = string.Empty;
		var backgroundColor = Colors.Transparent;
		switch (status)
		{
			case DocumentDetectionStatus.Ok:
				hint = "The document is Ok";
				backgroundColor = Colors.Green;
				break;
			case DocumentDetectionStatus.OkBarcode:
				hint = "Barcode ?????";
				break;
			case DocumentDetectionStatus.OkButTooSmall:
				hint = "Please move the camera closer to the document.";
				backgroundColor = Colors.Yellow;
				break;
			case DocumentDetectionStatus.OkButBadAngles:
				hint = "Please hold the camera in parallel over the document.";
				backgroundColor = Colors.Yellow;
				break;
			case DocumentDetectionStatus.OkButBadAspectRatio:
				hint = "The document size is too long.";
				backgroundColor = Colors.Yellow;
				break;
			case DocumentDetectionStatus.OkOffCenter:
				hint = "Please align the document in the center of the camera preview.";
				backgroundColor = Colors.Yellow;
				break;
			case DocumentDetectionStatus.ErrorNothingDetected:
				hint = "Unable to detect the document.";
				backgroundColor = Colors.Red;
				break;
			case DocumentDetectionStatus.ErrorTooDark:
				hint = "Unable to detect due to dark lighting conditions.";
				backgroundColor = Colors.Red;
				break;
			case DocumentDetectionStatus.ErrorTooNoisy:
				hint = "Unable to detect document due to too much noise in the preview.";
				backgroundColor = Colors.Red;
				break;
			case DocumentDetectionStatus.NotAcquired:
				hint = "Unable to acquire the document.";
				backgroundColor = Colors.Red;
				break;
			case DocumentDetectionStatus.Unknown:
				break;
		}

		ScanningHintLabel.Text = hint;
		ScanningHintLabel.BackgroundColor = backgroundColor.WithAlpha(0.5f);
	}
	
	private void ScannerButton_OnClicked(object sender, EventArgs e)
	{
		var selectedItem = (sender as Button)?.BindingContext as ClassicCollectionItem;
		if (selectedItem == null) return;

		selectedItem.Selected = !selectedItem.Selected;
		selectedItem.ClickAction?.Invoke();

		if (selectedItem.Title != Start && selectedItem.Title != Stop)
			return;

		// Start and Stop Toggle.
		if (selectedItem.Selected)
		{
			selectedItem.Title = Stop;
			DocumentScannerView.UnFreezeCamera();
		}
		else
		{
			selectedItem.Title = Start;
			DocumentScannerView.FreezeCamera();
		}
	}
}