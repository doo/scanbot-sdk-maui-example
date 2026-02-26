using System.Diagnostics;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Results;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Document.ClassicComponent;

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

		// @Tag("Classic UI EventHandlers")
		DocumentScannerView.OnSnappedDocumentResult += OnSnappedDocumentImageResult;
		DocumentScannerView.OnFrameDetectionResult += UpdateDetectionHintFromStatus;
		// @EndTag("Classic UI EventHandlers")
		
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
			new(Snap, () => DocumentScannerView.SnapDocument())
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
			if (!value)
			{
				ScanningHintLabel.IsVisible = false;
			}
			OnPropertyChanged();
		}
	}

	private bool _isFlashEnabled;

	public bool IsFlashEnabled
	{
		get => _isFlashEnabled;
		set
		{
			_isFlashEnabled = value;
			OnPropertyChanged();
		}
	}

	private bool _isPolygonEnabled;

	public bool IsPolygonEnabled
	{
		get => _isPolygonEnabled;
		set
		{
			_isPolygonEnabled = value;
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

	private void ToggleVisibility()
	{
		IsCameraVisible = !IsCameraVisible;

		if (!IsCameraVisible)
		{
			ScanningHintLabel.IsVisible = false;
		}
	}

	// @Tag("Classic UI EventHandlers Implementation")
	// Receives the result of Document Scanning. 
	private async void OnSnappedDocumentImageResult(object sender, SnappedDocumentResultEventArgs eventArgs)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.SetData(eventArgs.DocumentImage?.ToImageSource(quality: 50));
		await Navigation.PushAsync(resultsPage);
	}

	private void UpdateDetectionHintFromStatus(object sender, DetectionStatusEventArgs args)
	{
		var status = args.Result.Status;
		Debug.WriteLine("Document Detection Status: " + status);
		switch (status)
		{
			case DocumentDetectionStatus.Ok:
				ScanningHintLabel.Text  = "The document is Ok";
				ScanningHintLabel.BackgroundColor = Colors.Green.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.OkButTooSmall:
				ScanningHintLabel.Text  = "Please move the camera closer to the document.";
				ScanningHintLabel.BackgroundColor = Colors.Yellow.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.OkButBadAngles:
				ScanningHintLabel.Text  = "Please hold the camera in parallel over the document.";
				ScanningHintLabel.BackgroundColor = Colors.Yellow.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.OkButBadAspectRatio:
				ScanningHintLabel.Text  = "The document size is too long.";
				ScanningHintLabel.BackgroundColor = Colors.Yellow.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.ErrorNothingDetected:
				ScanningHintLabel.Text  = "Unable to detect the document.";
				ScanningHintLabel.BackgroundColor = Colors.Red.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.OkButTooDark:
				ScanningHintLabel.Text  = "Unable to detect due to dark lighting conditions.";
				ScanningHintLabel.BackgroundColor = Colors.Yellow.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.ErrorTooNoisy:
				ScanningHintLabel.Text  = "Unable to detect document due to too much noise in the preview.";
				ScanningHintLabel.BackgroundColor = Colors.Red.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.NotAcquired:
				ScanningHintLabel.Text  = "Unable to acquire the document.";
				ScanningHintLabel.BackgroundColor = Colors.Red.WithAlpha(0.5f);;
				break;
			case DocumentDetectionStatus.OkButOrientationMismatch:
				ScanningHintLabel.Text  = "Unable to acquire the document.";
				ScanningHintLabel.BackgroundColor = Colors.Yellow.WithAlpha(0.5f);;
				break;
			default:
				ScanningHintLabel.IsVisible = false;
				return;
		}
		ScanningHintLabel.IsVisible = IsAutoSnappingEnabled;
	}
	// @EndTag("Classic UI EventHandlers Implementation")

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
			DocumentScannerView.IsCameraFrozen = false;
		}
		else
		{
			selectedItem.Title = Start;
			DocumentScannerView.IsCameraFrozen = true;
		}
	}
}