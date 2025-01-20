using ScanbotSDK.MAUI.Configurations;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Barcode.RTU.v1;
using ScanbotSDK.MAUI.Common;

namespace ClassicComponent.Maui;

public partial class ClassicBarcodeScannerPage : ContentPage

{
	public ClassicBarcodeScannerPage()
	{
		InitializeComponent();

		cameraView.OverlayConfiguration = new SelectionOverlayConfiguration(true, BarcodeTextFormat.CodeAndType,
							textColor: Colors.Yellow,
							textContainerColor: Colors.Black,
							strokeColor: Colors.Yellow,
							highlightedTextColor: Colors.Red,
							highlightedTextContainerColor: Colors.Black,
							highlightedStrokeColor: Colors.Red);

		cameraView.FinderConfiguration = new FinderConfiguration
		{
							IsFinderEnabled = true,
							FinderLineColor = Colors.WhiteSmoke,
							FinderOverlayColor = Colors.Black.WithAlpha(0.5f),
							FinderLineCornerRadius = 20,
							FinderLineWidth = 5,
							// FinderAspectRatio = new AspectRatio(1,1)
		};
	}

	private void HandleScannerResults(BarcodeResultBundle result)
	{
		string text = string.Empty;

		if (result?.Barcodes != null)
		{
			foreach (var barcode in result.Barcodes)
			{
				text += $"{barcode.Text} ({barcode.Format.ToString().ToUpper()})\n";
				text += "--------------------------\n";
			}
		}

		System.Diagnostics.Debug.WriteLine(text);
		lblResult.Text = text;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Start barcode detection manually
		cameraView.StartDetection();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		// Stop barcode detection manually
		cameraView.StopDetection();

		cameraView.Handler?.DisconnectHandler();
	}

	private void CameraView_OnOnBarcodeScanResult(BarcodeResultBundle result)
	{
		HandleScannerResults(result);
	}
}