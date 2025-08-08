using System.Diagnostics;
using System.Windows.Input;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Document.ClassicComponent;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Results;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.ViewModels;

public class ClassicDocumentScannerViewModel : BaseViewModel
{
    public ClassicDocumentScannerViewModel()
    {
        SnappedDocumentImageResultCommand = new Command<SnappedDocumentImageResultEventArgs>(OnSnappedDocumentImageResult);
        UpdateDetectionStatusCommand = new Command<DetectionStatusEventArgs>(OnUpdateDetectionHintFromStatus);
        SwitchFlashEnabledCommand = new Command(() => IsFlashEnabled = !IsFlashEnabled);
        SwitchPolygonEnabledCommand = new Command(() => IsPolygonEnabled = !IsPolygonEnabled);
    }

    public ICommand SnappedDocumentImageResultCommand { get; private set; }

    public ICommand UpdateDetectionStatusCommand { get; private set; }

    public ICommand SwitchFlashEnabledCommand { get; private set; }

    public ICommand SwitchPolygonEnabledCommand { get; private set; }

    public ICommand SnapDocumentImageCommand { get; set; }

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
	private async void OnSnappedDocumentImageResult(SnappedDocumentImageResultEventArgs eventArgs)
	{
		var resultsPage = new DocumentScannerResultPage();
		resultsPage.SetData(ImageSource.FromStream(() => eventArgs.DocumentImage.AsStream()));
		await App.Navigation.PushAsync(resultsPage);
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
}