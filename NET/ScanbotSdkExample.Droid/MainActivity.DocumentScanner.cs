using Android.Content;
using Android.Views;
using IO.Scanbot.Sdk.Ui_v2.Common;
using IO.Scanbot.Sdk.Ui_v2.Document.Configuration;
using DocumentScannerActivity = IO.Scanbot.Sdk.Ui_v2.Document.DocumentScannerActivity;
using ScanbotSdkExample.Droid.Activities;
using ScanbotSdkExample.Droid.Utils;
using ScanbotSdkExample.Droid.Model;
using IO.Scanbot.Sdk.Common;
using IO.Scanbot.Sdk.Document;
using IO.Scanbot.Sdk.Documentscanner;
using IO.Scanbot.Sdk.Geometry;
using IO.Scanbot.Sdk.Image;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid;

public partial class MainActivity
{
    private Dictionary<int, Action<Intent>> DocumentScannerActions => new Dictionary<int, Action<Intent>>
    {
        { ScanDocumentRequestCode, HandleDocumentScannerResult },
        { ImportImageRequestCode, HandleImageImport },
    };

    private void SingleDocumentScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var configuration = new DocumentScanningFlow();
        
        configuration.OutputSettings.PagesScanLimit = 1;
        configuration.Screens.Camera.ScannerParameters.AspectRatios =
        [
            new AspectRatio(21.0, 29.7) // allow only A4 format documents to be scanned
        ];

        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanDocumentRequestCode);
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
        configuration.OutputSettings.PagesScanLimit = 1;
        configuration.Screens.Camera.ScannerParameters.AcceptedSizeScore = 75;
        configuration.Screens.Camera.ScannerParameters.AspectRatios = [ aspectRatio ];
        configuration.Screens.Camera.ViewFinder.Visible = true;
        configuration.Screens.Camera.ViewFinder.AspectRatio = aspectRatio;

        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanDocumentRequestCode);
    }

    private void MultipleDocumentScanning()
    {
        if (!CheckLicense())
        {
            return;
        }

        var configuration = new DocumentScanningFlow();
        configuration.OutputSettings.PagesScanLimit = 0; // allow multiple document scanning
        configuration.Screens.Camera.BottomBar.ShutterButton.InnerColor = new ScanbotColor(Android.Graphics.Color.Red);

        var intent = DocumentScannerActivity.NewIntent(this, configuration);
        StartActivityForResult(intent, ScanDocumentRequestCode);
    }

    private void HandleDocumentScannerResult(Intent data)
    {
        var documentId = data?.GetStringExtra(ActivityConstants.ExtraKeyRtuResult);

        if (documentId == null) return;
        
        var intent = PagePreviewActivity.CreateIntent(this, documentId);
        StartActivity(intent);
    }

    private void CreateDocFromImage()
    {
        var intent = new Intent();
        intent.SetType("image/*");
        intent.SetAction(Intent.ActionGetContent);
        intent.PutExtra(Intent.ExtraLocalOnly, false);
        intent.PutExtra(Intent.ExtraAllowMultiple, false);

        var chooser = Intent.CreateChooser(intent, Texts.ShareTitle);
        StartActivityForResult(chooser, ImportImageRequestCode);
    }

    private void HandleImageImport(Intent data)
    {
        _progress.Visibility = ViewStates.Visible;

        Alert.Toast(this, Texts.ImportingAndProcessing);

        var bitmap = ImageUtils.ProcessGalleryResult(this, data);

        var scanner = _scanbotSdk.CreateDocumentScanner(new DocumentScannerConfiguration()).Get<IDocumentScanner>();
        var result = scanner.Scan(ImageRef.FromBitmap(bitmap, new BasicImageLoadOptions())).Get<DocumentScanningResult>();

        var defaultDocumentSizeLimit = 0;
        var document = _scanbotSdk.DocumentApi.CreateDocument(defaultDocumentSizeLimit);
        document.AddPage(bitmap);

        if (result?.DetectionResult != null && result.DetectionResult.Status != DocumentDetectionStatus.ErrorNothingDetected)
        {
            document.PageAtIndex(0).Polygon = result.DetectionResult.PointsNormalized;
        }

        _progress.Visibility = ViewStates.Gone;

        var intent = PagePreviewActivity.CreateIntent(this, document.Uuid);
        StartActivity(intent);
    }

    private void ClassicDocumentScannerView()
    {
        Intent intent = new Intent(this, typeof(ClassicDocumentScannerViewActivity));
        StartActivityForResult(intent, ScanDocumentRequestCode);
    }
}