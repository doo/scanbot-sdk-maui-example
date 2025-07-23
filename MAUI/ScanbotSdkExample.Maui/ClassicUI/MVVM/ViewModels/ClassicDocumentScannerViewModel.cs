using System.Diagnostics;
using System.Windows.Input;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document.ClassicComponent;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Results;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.ViewModels;

public class ClassicDocumentScannerViewModel : BaseViewModel
{
	private const string AutoSnap = "Autosnap",
		Finder = "Finder",
		Flash = "Flash",
		Polygons = "Polygon",
		Snap = "Snap",
		Start = "Start",
		Stop = "Stop",
		Visibility = "Visibility";

    public ClassicDocumentScannerViewModel()
    {
        // CollectionView Buttons
        ScannerButtons = new List<ClassicCollectionItem>
        {
            new(AutoSnap, () => IsAutoSnappingEnabled = !IsAutoSnappingEnabled),
            new(Finder, () => IsFinderEnabled = !IsFinderEnabled),
            new(Flash, () => IsFlashEnabled = !IsFlashEnabled),
            new(Polygons, () => IsPolygonEnabled = !IsPolygonEnabled),
            new(Visibility, ToggleVisibility),
            new(Stop, null, true),
            new(Snap, () => SnapDocumentImageCommand.Execute(null)),
        };

        SnappedDocumentImageResultCommand = new Command<SnappedDocumentImageResultEventArgs>(OnSnappedDocumentImageResult);
        UpdateDetectionStatusCommand = new Command<DetectionStatusEventArgs>(OnUpdateDetectionHintFromStatus);
        SelectClassicCollectionItemCommand = new Command<ClassicCollectionItem>(OnSelectClassicCollectionItem);
    }

    public ICommand SnappedDocumentImageResultCommand { get; private set; }

    public ICommand UpdateDetectionStatusCommand { get; private set; }

    public ICommand SelectClassicCollectionItemCommand { get; private set; }

    public ICommand SnapDocumentImageCommand { get; set; }

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

	private List<ClassicCollectionItem> _scannerButtons = [];

	public List<ClassicCollectionItem> ScannerButtons
	{
		get => _scannerButtons;
		set
		{
			_scannerButtons = value;
			OnPropertyChanged();
		}
	}

	private string _scanningHintText;

	public string ScanningHintText
	{
		get => _scanningHintText;
		set
		{
			_scanningHintText = value;
			OnPropertyChanged();
		}
	}

	private bool _isScanningHintVisible;

	public bool IsScanningHintVisible
	{
		get => _isScanningHintVisible;
		set
		{
			_isScanningHintVisible = value;
			OnPropertyChanged();
		}
	}

	private Color _scanningHintBackgroundColor;

	public Color ScanningHintBackgroundColor
	{
		get => _scanningHintBackgroundColor;
		set
		{
			_scanningHintBackgroundColor = value;
			OnPropertyChanged();
		}
	}

	private bool _isFreezeCamera;

	public bool IsFreezeCamera
	{
		get => _isFreezeCamera;
		set
		{
			_isFreezeCamera = value;
			OnPropertyChanged();
		}
	}

	private void ToggleVisibility()
	{
		IsCameraVisible = !IsCameraVisible;

		if (!IsCameraVisible)
		{
			IsScanningHintVisible = false;
		}
	}

	// Receives the result of Document Scanning.
	private async void OnSnappedDocumentImageResult(SnappedDocumentImageResultEventArgs eventArgs)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.SetData(ImageSource.FromStream(() => eventArgs.DocumentImage.AsStream()));
		await Application.Current.MainPage.Navigation.PushAsync(resultsPage);
	}

	private void OnUpdateDetectionHintFromStatus(DetectionStatusEventArgs args)
	{
		var status = args.Status;
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
				IsScanningHintVisible = false;
				return;
		}

		ScanningHintText = hint;
		ScanningHintBackgroundColor = backgroundColor.WithAlpha(0.5f);
		IsScanningHintVisible = true;
	}

	private void OnSelectClassicCollectionItem(ClassicCollectionItem selectedItem)
	{
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
			IsFreezeCamera = false;
		}
		else
		{
			selectedItem.Title = Start;
			IsFreezeCamera = true;
		}
	}
}