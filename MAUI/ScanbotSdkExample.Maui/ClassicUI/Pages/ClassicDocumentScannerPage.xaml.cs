using System.Diagnostics;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Results;
using Microsoft.Maui.Graphics.Platform;
using ScanbotSDK.MAUI;

namespace ScanbotSdkExample.Maui.ClassicUI.Pages;

public partial class ClassicDocumentScannerPage : ContentPage
{
	private const string AutoSnap = "Autosnap",
		Finder = "Finder",
		Flash = "Flash",
		Polygons = "Polygon",
		Snap = "Snap",
		Start = "Start",
		Stop = "Stop",
		Visibility = "Visibility";

    public ClassicDocumentScannerPage()
	{
		InitializeComponent();

		DocumentScannerView.OnSnappedDocumentImageResult += DidDetectDocument;
		DocumentScannerView.OnUpdateDetectionStatus += UpdateDetectionHintFromStatus;
		
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
			new(Visibility, ToggleVisibility),
			new(Stop, null, true),
			new(Snap, DocumentScannerView.SnapDocumentImage),
		};
		
		BindingContext = this;
	}

	private bool _isAutoSnappingEnabled;

	public bool IsAutoSnappingEnabled
	{
		get => _isAutoSnappingEnabled;
		set
		{
			_isAutoSnappingEnabled = value;
			OnPropertyChanged();
		}
	}

	private bool _flashEnabled;

	public bool IsFlashEnabled
	{
		get => _flashEnabled;
		set
		{
			_flashEnabled = value;
			OnPropertyChanged();
		}
	}

	private bool _polygonEnabled;

	public bool IsPolygonEnabled
	{
		get => _polygonEnabled;
		set
		{
			_polygonEnabled = value;
			OnPropertyChanged();
		}
	}

	private bool _isFinderEnabled;

	public bool IsFinderEnabled
	{
		get => _isFinderEnabled;
		set
		{
			_isFinderEnabled = value;
			OnPropertyChanged();
		}
	}

	private bool _isCameraVisible = true;

	public bool IsCameraVisible
	{
		get => _isCameraVisible;
		set
		{
			_isCameraVisible = value;
			OnPropertyChanged();
		}
	}

	private List<ClassicCollectionItem> _scannerButtons = new List<ClassicCollectionItem>();

	public List<ClassicCollectionItem> ScannerButtons
	{
		get => _scannerButtons;
		set
		{
			_scannerButtons = value;
			OnPropertyChanged();
		}
	}

	public void ToggleVisibility()
	{
		IsCameraVisible = !IsCameraVisible;

		if (!IsCameraVisible)
		{
			ScanningHintLabel.IsVisible = false;
		}
	}

	// Receives the result of Document Scanning. 
	public void DidDetectDocument(PlatformImage documentImage, PlatformImage originalImage)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.NavigateData(ImageSource.FromStream(() => documentImage.AsStream()));
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
			case DocumentDetectionStatus.OkButOrientationMismatch:
				hint = "Unable to acquire the document.";
				backgroundColor = Colors.Red;
				break;
			default:
				ScanningHintLabel.IsVisible = false;
				return;
		}

		ScanningHintLabel.Text = hint;
		ScanningHintLabel.BackgroundColor = backgroundColor.WithAlpha(0.5f);
		ScanningHintLabel.IsVisible = true;
	}
	
	private void ScannerButtonOnClicked(object sender, EventArgs e)
	{
		var selectedItem = (sender as Button)?.BindingContext as ClassicCollectionItem;
		if (selectedItem == null) return;

		if (selectedItem.Title != Snap)
		{
			selectedItem.Selected = !selectedItem.Selected;
		}

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