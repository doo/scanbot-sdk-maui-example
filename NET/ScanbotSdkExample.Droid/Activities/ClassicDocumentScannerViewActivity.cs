using _Microsoft.Android.Resource.Designer;
using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using IO.Scanbot.Sdk;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Document;
using IO.Scanbot.Sdk.Document.UI;
using IO.Scanbot.Sdk.Documentscanner;
using IO.Scanbot.Sdk.Image;
using IO.Scanbot.Sdk.Ui_v2.Common.Activity;
using IO.Scanbot.Sdk.UI.Camera;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Activities
{
    [Activity]
    public class ClassicDocumentScannerViewActivity : AppCompatActivity,  IDocumentScannerViewCallback
    {   
        private bool _flashEnabled;
        private bool _autoSnappingEnabled = true;
        private const bool IgnoreBadAspectRatio = true;
        private long _lastUserGuidanceHintTs;

        private DocumentScannerView _documentScannerView;
        private IDocumentScanner _documentScanner;
        private TextView _userGuidanceTextView;
        private ProgressBar _imageProcessingProgress;
        private ShutterButton _shutterButton;
        private Button _autoSnappingToggleButton;

        private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SupportRequestWindowFeature(WindowCompat.FeatureActionBarOverlay);
            base.OnCreate(savedInstanceState);
            
            SetContentView(ResourceConstant.Layout.document_scanner_view_activity);
            
            _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
            _documentScannerView = FindViewById<DocumentScannerView>(ResourceConstant.Id.document_scanner_view)!;
            _documentScanner = _scanbotSdk.CreateDocumentScanner(new DocumentScannerConfiguration()).Get<IDocumentScanner>();
            
            DocumentScannerViewWrapper.InitCamera(_documentScannerView);
            DocumentScannerViewWrapper.InitScanningBehavior(_documentScannerView,
                                documentScanner: _documentScanner,
                                new DocumentScannerResultImplementation(ShowUserGuidance), this);

            SupportActionBar?.Hide();

            // Uncomment to disable AutoFocus by manually touching the camera view:
            // _documentScannerView.CameraConfiguration.SetAutoFocusOnTouch(false);
            
            _documentScannerView.CameraConfiguration.SetCameraPreviewMode(CameraPreviewMode.FitIn);
          
            // custom color to the polygon view
            _documentScannerView.PolygonConfiguration.SetPolygonStrokeColor(Color.Red);
            _documentScannerView.PolygonConfiguration.SetPolygonStrokeColorOK(Color.Green);
            _documentScannerView.PolygonConfiguration.SetPolygonAutoSnappingProgressStrokeColor(Color.Black);

            // set automatic snapping enabled.
            _documentScannerView.ViewController.AutoSnappingEnabled = _autoSnappingEnabled;
            
            SetUpUiElements();
        }

        private void SetUpUiElements()
        {
            _userGuidanceTextView = FindViewById<TextView>(ResourceConstant.Id.user_guidance_text_view)!;
            _imageProcessingProgress = FindViewById<ProgressBar>(ResourceConstant.Id.image_processing_progress)!;
            _shutterButton = FindViewById<ShutterButton>(ResourceConstant.Id.shutter_button)!;
            
            _shutterButton.Click += delegate
            {
               _documentScannerView.ViewController.TakePicture(false);
            };
            
            _shutterButton.Visibility = ViewStates.Visible;

            FindViewById(ResourceConstant.Id.flash_button)!.Click += delegate 
            {
              _documentScannerView.ViewController.UseFlash(!_flashEnabled);
              _flashEnabled = !_flashEnabled;
            };

            _autoSnappingToggleButton = FindViewById<Button>(ResourceConstant.Id.auto_snapping_toggle_button)!;
            _autoSnappingToggleButton.Click += delegate 
            {
              _autoSnappingEnabled = !_autoSnappingEnabled;
              SetAutoSnapEnabled(_autoSnappingEnabled);
            };

            _shutterButton.Post(() =>
            {
               SetAutoSnapEnabled(_autoSnappingEnabled);
            });
        }

        private bool ShowUserGuidance(DocumentScannerFrameHandler.DetectedFrame frame, SdkLicenseError error)
        {
            if (!_autoSnappingEnabled || frame == null) { return false; }

            if (Java.Lang.JavaSystem.CurrentTimeMillis() - _lastUserGuidanceHintTs < 400)
            {
                return false;
            }

            var color = Color.Red;
            var guideText = "";

            var result = frame.DetectionStatus;
            if (result == DocumentDetectionStatus.Ok)
            {
                guideText = "Don't move.\nCapturing...";
                color = Color.Green;
            }
            else if (result == DocumentDetectionStatus.OkButTooSmall)
            {
                guideText = "Move closer";
            }
            else if (result == DocumentDetectionStatus.OkButBadAngles)
            {
                guideText = "Perspective";
            }
            else if (result == DocumentDetectionStatus.OkButBadAspectRatio)
            {
                guideText = "Wrong aspect ratio.\n Rotate your device";
                if (IgnoreBadAspectRatio)
                {
                    guideText = "Don't move.\nCapturing...";
                    color = Color.Green;
                }
            }
            else if (result == DocumentDetectionStatus.ErrorNothingDetected)
            {
                guideText = "No Document";
            }
            else if (result == DocumentDetectionStatus.ErrorTooNoisy)
            {
                guideText = "Background too noisy";
            }
            else if (result == DocumentDetectionStatus.ErrorTooDark)
            {
                guideText = "Poor light";
            }

            // The HandleResult callback is coming from a worker thread. Use main UI thread to update UI:
            _userGuidanceTextView.Post(() =>
            {
                _userGuidanceTextView.Text = guideText;
                _userGuidanceTextView.SetTextColor(Color.White);
                _userGuidanceTextView.SetBackgroundColor(color);
            });

            _lastUserGuidanceHintTs = Java.Lang.JavaSystem.CurrentTimeMillis();

            return false;
        }

        public void OnCameraOpen()
        {
            // Uncomment to disable shutter sound (supported since Android 4.2+):
            // Please note that some devices may not allow disabling the camera shutter sound. 
            // If the shutter sound state cannot be set to the desired value, this method will be ignored.
            _documentScannerView.CameraConfiguration.SetShutterSound(false);
            _documentScannerView.ViewController.UseFlash(_flashEnabled);

            // Enable ContinuousFocus mode:
            _documentScannerView.ViewController.ContinuousFocus();
        }

        public void OnPictureTaken(ImageRef image, CaptureInfo captureInfo)
        {
            // Here we get the full image from the camera and further apply document detection on it.
            // This is just a demo showing detected image as downscaled preview image.

            // Show progress spinner:
            RunOnUiThread(() =>
            {
                _imageProcessingProgress.Visibility = ViewStates.Visible;
                _userGuidanceTextView.Visibility = ViewStates.Gone;
            });
                
            DetectDocumentOnImage(image);
            Finish();
        }

        /// <summary>
        /// Detects the document on the bitmap image and adds it to the document storage.
        /// The document can be further access with the Document Uuid.
        /// </summary>
        /// <param name="imageRef">Full image captured by the Camera</param>
        private void DetectDocumentOnImage(ImageRef imageRef)
        {
            var detectionResult = _documentScanner?.Scan(imageRef).Get<DocumentScanningResult>();

            var defaultDocumentSizeLimit = 0;
            var document = _scanbotSdk.DocumentApi.CreateDocument(defaultDocumentSizeLimit);
            document.AddPage(imageRef);

            if (detectionResult?.DetectionResult != null)
            {
                document.PageAtIndex(0).Polygon = detectionResult.DetectionResult.PointsNormalized;
            }
            
            Bundle extras = new Bundle();
            extras.PutString(ActivityConstants.ExtraKeyRtuResult, document.Uuid);
            Intent intent = new Intent();
            intent.PutExtras(extras);
            SetResult(Result.Ok, intent);
        }

        void SetAutoSnapEnabled(bool enabled)
        {
            _documentScannerView.ViewController.AutoSnappingEnabled = enabled;
            _documentScannerView.ViewController.FrameProcessingEnabled = enabled;
            
            _documentScannerView.PolygonConfiguration.SetPolygonViewVisible(enabled);
            _documentScannerView.PolygonConfiguration.SetPolygonAutoSnapProgressEnabled(enabled);
            
            _autoSnappingToggleButton.Text = "Automatic " + (enabled ? "ON" : "OFF");
            if (enabled)
            {
                _shutterButton.ShowAutoButton();
                _userGuidanceTextView.Visibility = ViewStates.Visible;
            }
            else
            {
                _shutterButton.ShowManualButton();
                _userGuidanceTextView.Visibility = ViewStates.Gone;
            }
        }
    }
}

internal class DocumentScannerResultImplementation(DocumentScannerResultImplementation.DocumentScannerHandleResult handleResult) : DocumentScannerResultHandlerWrapper
{
   internal delegate bool DocumentScannerHandleResult(DocumentScannerFrameHandler.DetectedFrame frame, SdkLicenseError error);

   public override bool HandleResult(DocumentScannerFrameHandler.DetectedFrame result, SdkLicenseError error) => handleResult?.Invoke(result, error) ?? false;
}