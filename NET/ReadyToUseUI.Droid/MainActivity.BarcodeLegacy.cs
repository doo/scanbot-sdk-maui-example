using Android.Content;
using Android.Graphics;
using DocumentSDK.NET.Model;
using IO.Scanbot.Sdk.UI.View.Barcode;
using IO.Scanbot.Sdk.UI.View.Barcode.Batch;
using IO.Scanbot.Sdk.UI.View.Barcode.Batch.Configuration;
using IO.Scanbot.Sdk.UI.View.Barcode.Configuration;
using IO.Scanbot.Sdk.UI.View.Base.Configuration;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private void ScanBarcode()
    {
        var configuration = new BarcodeScannerConfiguration();
        configuration.SetSelectionOverlayConfiguration(
            new SelectionOverlayConfiguration(
                overlayEnabled: true,
                automaticSelectionEnabled: true,
                textFormat: IO.Scanbot.Sdk.Barcode.UI.BarcodeOverlayTextFormat.CodeAndType,
                polygonColor: Color.Yellow,
                textColor: Color.Yellow,
                textContainerColor: Color.Black));

        configuration.SetFinderTextHint("Please align the QR-/Barcode in the frame above to scan it.");
        configuration.SetTopBarButtonsColor(Color.White);
        configuration.SetTopBarBackgroundColor(Color.Black);

        var intent = BarcodeScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, QR_BARCODE_DEFAULT_REQUEST);
    }

    private void ScanBarcodesInBatch()
    {
        var configuration = new BatchBarcodeScannerConfiguration();
        configuration.SetSelectionOverlayConfiguration(
            new SelectionOverlayConfiguration(
                overlayEnabled: true,
                automaticSelectionEnabled: true,
                textFormat: IO.Scanbot.Sdk.Barcode.UI.BarcodeOverlayTextFormat.CodeAndType,
                polygonColor: Color.Yellow,
                textColor: Color.Yellow,
                textContainerColor: Color.Black));

        configuration.SetOrientationLockMode(CameraOrientationMode.Portrait);

        configuration.SetFinderTextHint("Please align the QR-/Barcode in the frame above to scan it.");
        var intent = BatchBarcodeScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, QR_BARCODE_DEFAULT_REQUEST);
    }

    private void ImportAndDetectBarcode()
    {
        var intent = new Intent();
        intent.SetType("image/*");
        intent.SetAction(Intent.ActionGetContent);
        intent.PutExtra(Intent.ExtraLocalOnly, false);
        intent.PutExtra(Intent.ExtraAllowMultiple, false);

        var chooser = Intent.CreateChooser(intent, Texts.share_title);
        StartActivityForResult(chooser, IMPORT_BARCODE_REQUEST);
    }
}