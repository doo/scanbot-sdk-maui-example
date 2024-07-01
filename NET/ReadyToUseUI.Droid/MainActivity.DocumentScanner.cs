using Android.Content;
using DocumentSDK.NET.Model;
using IO.Scanbot.Sdk.UI.View.Base.Configuration;
using IO.Scanbot.Sdk.UI.View.Camera;
using IO.Scanbot.Sdk.UI.View.Camera.Configuration;
using ReadyToUseUI.Droid.Activities;

namespace ReadyToUseUI.Droid;

public partial class MainActivity
{
        private void ScanDocument()
        {
            var configuration = new DocumentScannerConfiguration();

            configuration.SetCameraPreviewMode(IO.Scanbot.Sdk.Camera.CameraPreviewMode.FitIn);
            configuration.SetIgnoreBadAspectRatio(true);
            configuration.SetMultiPageEnabled(true);
            configuration.SetPageCounterButtonTitle("%d Page(s)");
            configuration.SetTextHintOK("Don't move.\nScanning document...");

            // further configuration properties
            //configuration.SetBottomBarBackgroundColor(Color.Blue);
            //configuration.SetBottomBarButtonsColor(Color.White);
            //configuration.SetFlashButtonHidden(true);
            // and so on...

            var intent = DocumentScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
        }

        private void ScanDocumentWithFinder()
        {
            var configuration = new FinderDocumentScannerConfiguration();

            configuration.SetCameraPreviewMode(IO.Scanbot.Sdk.Camera.CameraPreviewMode.FitIn);
            configuration.SetIgnoreBadAspectRatio(true);
            configuration.SetTextHintOK("Don't move.\nScanning document...");
            configuration.SetOrientationLockMode(CameraOrientationMode.Portrait);
            configuration.SetFinderAspectRatio(new IO.Scanbot.Sdk.AspectRatio(21.0, 29.7)); // a4 portrait

            // further configuration properties
            //configuration.SetFinderLineColor(Color.Red);
            //configuration.SetTopBarBackgroundColor(Color.Blue);
            //configuration.SetFlashButtonHidden(true);
            // and so on...

            var intent = FinderDocumentScannerActivity.NewIntent(this, configuration);
            StartActivityForResult(intent, SCAN_DOCUMENT_REQUEST_CODE);
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

        private void ViewImages() => StartActivity(new Intent(this, typeof(PagePreviewActivity)));
}