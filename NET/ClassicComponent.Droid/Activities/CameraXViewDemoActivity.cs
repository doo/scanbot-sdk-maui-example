using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Util;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using IO.Scanbot.Sdk;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Document;
using IO.Scanbot.Sdk.Document.UI;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.UI.Camera;

namespace ClassicComponent.Droid
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class CameraXViewDemoActivity : AppCompatActivity,  IDocumentScannerViewCallback
    {
        public static string EXTRAS_ARG_DOC_IMAGE_FILE_URI = "documentImageFileUri";
        public static string EXTRAS_ARG_ORIGINAL_IMAGE_FILE_URI = "originalImageFileUri";
        private static string LOG_TAG = nameof(CameraXViewDemoActivity);
        
        private bool flashEnabled = false;
        private bool autoSnappingEnabled = true;
        private readonly bool ignoreBadAspectRatio = true;
        private long lastUserGuidanceHintTs = 0L;

        private DocumentScannerView _documentScannerView;
        private TextView userGuidanceTextView;
        private ProgressBar imageProcessingProgress;
        private ShutterButton shutterButton;
        private Button autoSnappingToggleButton;

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SupportRequestWindowFeature(WindowCompat.FeatureActionBarOverlay);
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.CameraXViewDemo);
            
            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            _documentScannerView = FindViewById<DocumentScannerView>(Resource.Id.scanbotCameraView);
            
            var contourDetector = scanbotSDK.CreateDocumentScanner();
            
            DocumentScannerViewWrapper.InitCamera(_documentScannerView);
            DocumentScannerViewWrapper.InitScanningBehavior(_documentScannerView,
                                documentScanner: contourDetector,
                                new DocumentScannerResultImplementation(new DocumentScannerResultImplementation.DocumentScannerHandleResult(ShowUserGuidance)), this);

            SupportActionBar.Hide();

            // Uncomment to disable AutoFocus by manually touching the camera view:
            _documentScannerView.CameraConfiguration.SetAutoFocusOnTouch(false);

            // Preview Mode: See https://github.com/doo/Scanbot-SDK-Examples/wiki/Using-ScanbotCameraView#preview-mode
            _documentScannerView.CameraConfiguration.SetCameraPreviewMode(CameraPreviewMode.FitIn);
          
            // custom color to the polygon view
            _documentScannerView.PolygonConfiguration.SetPolygonStrokeColor(Color.Red);
            _documentScannerView.PolygonConfiguration.SetPolygonStrokeColorOK(Color.Green);
            _documentScannerView.PolygonConfiguration.SetPolygonAutoSnappingProgressStrokeColor(Color.Black);

            // set automatic snapping enabled.
            _documentScannerView.ViewController.AutoSnappingEnabled = autoSnappingEnabled;
            
            SetUpUIElements();
        }

        private void SetUpUIElements()
        {
            userGuidanceTextView = FindViewById<TextView>(Resource.Id.userGuidanceTextView);
            imageProcessingProgress = FindViewById<ProgressBar>(Resource.Id.imageProcessingProgress);
            
            shutterButton = FindViewById<ShutterButton>(Resource.Id.shutterButton);
            shutterButton.Click += delegate
            {
               _documentScannerView.ViewController.TakePicture(false);
            };
            
            shutterButton.Visibility = ViewStates.Visible;

            FindViewById(Resource.Id.scanbotFlashButton).Click += delegate 
            {
              _documentScannerView.ViewController.UseFlash(!flashEnabled);
              flashEnabled = !flashEnabled;
            };

            autoSnappingToggleButton = FindViewById<Button>(Resource.Id.autoSnappingToggleButton);
            autoSnappingToggleButton.Click += delegate 
            {
              autoSnappingEnabled = !autoSnappingEnabled;
              SetAutoSnapEnabled(autoSnappingEnabled);
            };

            shutterButton.Post(() =>
            {
               SetAutoSnapEnabled(autoSnappingEnabled);
            });
        }

        private bool ShowUserGuidance(DocumentScannerFrameHandler.DetectedFrame frame, SdkLicenseError error)
        {
            if (!autoSnappingEnabled || frame == null) { return false; }

            if (Java.Lang.JavaSystem.CurrentTimeMillis() - lastUserGuidanceHintTs < 400)
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
                if (ignoreBadAspectRatio)
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
            userGuidanceTextView.Post(() =>
            {
                userGuidanceTextView.Text = guideText;
                userGuidanceTextView.SetTextColor(Color.White);
                userGuidanceTextView.SetBackgroundColor(color);
            });

            lastUserGuidanceHintTs = Java.Lang.JavaSystem.CurrentTimeMillis();

            return false;
        }

        public void OnCameraOpen()
        {
            // Uncomment to disable shutter sound (supported since Android 4.2+):
            // Please note that some devices may not allow disabling the camera shutter sound. 
            // If the shutter sound state cannot be set to the desired value, this method will be ignored.
            _documentScannerView.CameraConfiguration.SetShutterSound(false);
            _documentScannerView.ViewController.UseFlash(flashEnabled);

            // Enable ContinuousFocus mode:
            _documentScannerView.ViewController.ContinuousFocus();
        }

        public void OnPictureTaken(byte[] image, CaptureInfo captureInfo)
        {
            // Here we get the full image from the camera and apply document detection on it.
            // Implement a suitable async(!) detection and image handling here.
            // This is just a demo showing detected image as downscaled preview image.

            Log.Debug(LOG_TAG, "OnPictureTaken: imageOrientation = " + captureInfo.ImageOrientation);

            // Show progress spinner:
            RunOnUiThread(() =>
            {
                imageProcessingProgress.Visibility = ViewStates.Visible;
                userGuidanceTextView.Visibility = ViewStates.Gone;
            });

            // decode bytes as Bitmap
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 1;
            var originalBitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length, options);

            // rotate original image if required:
            if (captureInfo.ImageOrientation > 0)
            {
                Matrix matrix = new Matrix();
                matrix.SetRotate(captureInfo.ImageOrientation, originalBitmap.Width / 2f, originalBitmap.Height / 2f);
                originalBitmap = Bitmap.CreateBitmap(originalBitmap, 0, 0, originalBitmap.Width, originalBitmap.Height, matrix, false);
            }

            // Store the original image as file:
            var originalImgUri = TempImageStorage.Instance.AddImage(originalBitmap);

            Android.Net.Uri documentImgUri = null;
            // Run document detection on original image:
            var detector = scanbotSDK.CreateDocumentScanner();
            var sdkDetectionResult = detector.ScanFromBitmap(originalBitmap);
            //  var detectionResult = ScanbotSDK.MAUI.Native.Droid.ScanbotSDK.DetectDocument(originalBitmap);
            if (sdkDetectionResult.Status == DocumentDetectionStatus.Ok)
            {
                if (sdkDetectionResult.PointsNormalized != null)
                {
                    var resultImage = new ImageProcessor(originalBitmap).Crop(sdkDetectionResult.PointsNormalized).ProcessedBitmap();
                    documentImgUri = TempImageStorage.Instance.AddImage(resultImage);
                }
                else
                {
                    documentImgUri = TempImageStorage.Instance.AddImage(originalBitmap);
                }
            }
            else
            {
                // No document detected! Use original image as document image, so user can try to apply manual cropping.
                documentImgUri = originalImgUri;
            }

            Bundle extras = new Bundle();
            extras.PutString(EXTRAS_ARG_DOC_IMAGE_FILE_URI, documentImgUri.ToString());
            extras.PutString(EXTRAS_ARG_ORIGINAL_IMAGE_FILE_URI, originalImgUri.ToString());
            Intent intent = new Intent();
            intent.PutExtras(extras);
            SetResult(Result.Ok, intent);

            Finish();

            /* If you want to continue scanning:
            RunOnUiThread(() => {
                // continue camera preview
                cameraView.StartPreview();
                cameraView.ContinuousFocus();
            });
            */
        }

        void SetAutoSnapEnabled(bool enabled)
        {
            _documentScannerView.ViewController.AutoSnappingEnabled = enabled;
            _documentScannerView.ViewController.FrameProcessingEnabled = enabled;
            
            _documentScannerView.PolygonConfiguration.SetPolygonViewVisible(enabled);
            _documentScannerView.PolygonConfiguration.SetPolygonAutoSnapProgressEnabled(enabled);
            
            autoSnappingToggleButton.Text = ("Automatic " + (enabled ? "ON" : "OFF"));
            if (enabled)
            {
                shutterButton.ShowAutoButton();
                userGuidanceTextView.Visibility = ViewStates.Visible;
            }
            else
            {
                shutterButton.ShowManualButton();
                userGuidanceTextView.Visibility = ViewStates.Gone;
            }
        }
    }
}

internal class DocumentScannerResultImplementation(DocumentScannerResultImplementation.DocumentScannerHandleResult handleResult) : DocumentScannerResultHandlerWrapper
{
   internal delegate bool DocumentScannerHandleResult(DocumentScannerFrameHandler.DetectedFrame frame, SdkLicenseError error);

   public override bool HandleResult(DocumentScannerFrameHandler.DetectedFrame result, SdkLicenseError error) =>
                       handleResult?.Invoke(result, error) ?? false;
}