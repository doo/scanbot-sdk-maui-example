using System.Diagnostics;
using System.Windows.Input;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Document.ClassicComponent;
using ScanbotSdkExample.Maui.Results;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.ViewModels;

public class ClassicDocumentScannerViewModel : BaseViewModel
{
    public ClassicDocumentScannerViewModel()
    {
	    SnappedDocumentResultCommand = new Command<SnappedDocumentResultEventArgs>(OnSnappedDocumentResult);
	    UpdateDetectionStatusCommand = new Command<DetectionStatusEventArgs>(OnUpdateDetectionHintFromStatus);
        SwitchFlashEnabledCommand = new Command(() => IsFlashEnabled = !IsFlashEnabled);
        SwitchPolygonEnabledCommand = new Command(() => IsPolygonEnabled = !IsPolygonEnabled);
    }

    public ICommand SnappedDocumentResultCommand { get; private set; }
    public ICommand UpdateDetectionStatusCommand { get; private set; }
    public ICommand SwitchFlashEnabledCommand { get; private set; }
    public ICommand SwitchPolygonEnabledCommand { get; private set; }
    
    private ICommand _snapDocumentCommand;
    public ICommand SnapDocumentCommand
    {
	    get => _snapDocumentCommand;
	    set
	    {
		    _snapDocumentCommand = value;
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

	// Receives the result of Document Scanning.
	private async void OnSnappedDocumentResult(SnappedDocumentResultEventArgs eventArgs)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.SetData(eventArgs?.DocumentImage?.ToImageSource());
		await App.Navigation.PushAsync(resultsPage);
	}

	private void OnUpdateDetectionHintFromStatus(DetectionStatusEventArgs args)
	{
		var status = args.Result.Status;
		Debug.WriteLine("Document Detection Status: " + status);
		switch (status)
		{
			case DocumentDetectionStatus.Ok:
				ScanningHintText = "The document is Ok";
				ScanningHintBackgroundColor = Colors.Green.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.OkButTooSmall:
				ScanningHintText = "Please move the camera closer to the document.";
				ScanningHintBackgroundColor = Colors.Yellow.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.OkButBadAngles:
				ScanningHintText = "Please hold the camera in parallel over the document.";
				ScanningHintBackgroundColor = Colors.Yellow.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.OkButBadAspectRatio:
				ScanningHintText = "The document size is too long.";
				ScanningHintBackgroundColor = Colors.Yellow.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.ErrorNothingDetected:
				ScanningHintText = "Unable to detect the document.";
				ScanningHintBackgroundColor = Colors.Red.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.OkButTooDark:
				ScanningHintText = "Unable to detect due to dark lighting conditions.";
				ScanningHintBackgroundColor = Colors.Yellow.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.ErrorTooNoisy:
				ScanningHintText = "Unable to detect document due to too much noise in the preview.";
				ScanningHintBackgroundColor = Colors.Red.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.NotAcquired:
				ScanningHintText = "Unable to acquire the document.";
				ScanningHintBackgroundColor = Colors.Red.WithAlpha(0.5f);
				break;
			case DocumentDetectionStatus.OkButOrientationMismatch:
				ScanningHintText = "Unable to acquire the document.";
				ScanningHintBackgroundColor = Colors.Yellow.WithAlpha(0.5f);;
				break;
			default:
				IsScanningHintVisible = false;
				return;
		}
		IsScanningHintVisible = true;
	}
}