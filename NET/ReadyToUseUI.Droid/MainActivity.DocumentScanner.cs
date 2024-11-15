using Android.Content;
using Android.Views;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;
using DocumentScannerActivity = IO.Scanbot.Sdk.Ui_v2.Document.DocumentScannerActivity;
using ReadyToUseUI.Droid.Activities;
using ReadyToUseUI.Droid.Utils;
using DocumentSDK.NET.Model;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> documentScannerActions => new Dictionary<int, Action<Intent>>
    {
        { SCAN_DOCUMENT_REQUEST_CODE, HandleDocumentScannerResult },
        { IMPORT_IMAGE_REQUEST, HandleImageImport },
    };

    private void SingleDocumentScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var configuration = new DocumentScanningFlow();
        configuration.OutputSettings.PagesScanLimit = 1;
        configuration.Screens.Camera.CameraConfiguration.RequiredAspectRatios = new[]
        {
                new AspectRatio(21.0, 29.7) // allow only A4 format documents to be scanned
            };

        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
    }

    private void SingleFinderDocumentScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        // allow only A4 format documents to be scanned
        var aspectRatio = new AspectRatio(21.0, 29.7);

        var configuration = new DocumentScanningFlow();
        configuration.Screens.Camera.CameraConfiguration.AcceptedSizeScore = 0.75;
        configuration.Screens.Camera.CameraConfiguration.RequiredAspectRatios = new[] { aspectRatio };
        configuration.Screens.Camera.ViewFinder.Visible = true;
        configuration.Screens.Camera.ViewFinder.AspectRatio = aspectRatio;

        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
    }

    private void MultipleDocumentScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var configuration = new DocumentScanningFlow();
        configuration.OutputSettings.PagesScanLimit = 1;
        configuration.Screens.Camera.BottomBar.ShutterButton.InnerColor = new ScanbotColor(Android.Graphics.Color.Red);

        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
    }

    private void HandleDocumentScannerResult(Intent data)
    {
        var documentId = data?.GetStringExtra(IO.Scanbot.Sdk.Ui_v2.Common.Activity.ActivityConstants.ExtraKeyRtuResult) as string;
        var intent = PagePreviewActivity.CreateIntent(this, documentId);
        StartActivity(intent);
    }

    private void ImportImage()
    {
        var intent = new Intent();
        intent.SetType("image/*");
        intent.SetAction(Intent.ActionGetContent);
        intent.PutExtra(Intent.ExtraLocalOnly, false);
        intent.PutExtra(Intent.ExtraAllowMultiple, false);

        var chooser = Intent.CreateChooser(intent, Texts.share_title);
        StartActivityForResult(chooser, IMPORT_IMAGE_REQUEST);
    }

    private void HandleImageImport(Intent data)
    {
        progress.Visibility = ViewStates.Visible;

        Alert.Toast(this, Texts.importing_and_processing);

        var bitmap = ImageUtils.ProcessGalleryResult(this, data);

        var detector = scanbotSDK.CreateContourDetector();
        var detectionResult = detector.Detect(bitmap);

        var defaultDocumentSizeLimit = 0;
        var document = scanbotSDK.DocumentApi.CreateDocument(defaultDocumentSizeLimit);
        document.AddPage(bitmap);

        if (detectionResult != null)
        {
            document.PageAtIndex(0).Polygon = detectionResult.PolygonF;
        }

        progress.Visibility = ViewStates.Gone;

        var intent = PagePreviewActivity.CreateIntent(this, document.Uuid);
        StartActivity(intent);
    }
}