using ClassicComponent.Maui.Models;

namespace ClassicComponent.Maui;

public partial class ClassicDocumentScannerPage
{
	private const string AutoSnap = "Autosnap",
						Finder = "Finder",
						Flash = "Flash",
						Polygons = "Polygon",
						Start = "Start",
						Stop = "Stop",
						Visibility = "Visibility";

	private bool _isAutoSnappingEnabled;

	public bool IsAutoSnappingEnabled
	{
		get => _isAutoSnappingEnabled;
		set
		{
			_isAutoSnappingEnabled = value;
			OnPropertyChanged(nameof(IsAutoSnappingEnabled));
		}
	}

	private bool _flashEnabled;

	public bool IsFlashEnabled
	{
		get => _flashEnabled;
		set
		{
			_flashEnabled = value;
			OnPropertyChanged(nameof(IsFlashEnabled));
		}
	}

	private bool _polygonEnabled;

	public bool IsPolygonEnabled
	{
		get => _polygonEnabled;
		set
		{
			_polygonEnabled = value;
			OnPropertyChanged(nameof(IsPolygonEnabled));
		}
	}

	private bool _isFinderEnabled;

	public bool IsFinderEnabled
	{
		get => _isFinderEnabled;
		set
		{
			_isFinderEnabled = value;
			OnPropertyChanged(nameof(IsFinderEnabled));
		}
	}

	private bool _isCameraVisible = true;

	public bool IsCameraVisible
	{
		get => _isCameraVisible;
		set
		{
			_isCameraVisible = value;
			OnPropertyChanged(nameof(IsCameraVisible));
		}
	}

	private List<ClassicCollectionItem> _scannerButtons = new List<ClassicCollectionItem>();

	public List<ClassicCollectionItem> ScannerButtons
	{
		get => _scannerButtons;
		set
		{
			_scannerButtons = value;
			OnPropertyChanged(nameof(ScannerButtons));
		}
	}
}