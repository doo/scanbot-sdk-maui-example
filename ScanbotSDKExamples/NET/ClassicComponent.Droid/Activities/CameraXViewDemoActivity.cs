using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Util;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;

using IO.Scanbot.Sdk.UI.Camera;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Contourdetector;
using IO.Scanbot.Sdk.UI;
using IO.Scanbot.Sdk.Core.Contourdetector;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk;

namespace ClassicComponent.Droid
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class CameraXViewDemoActivity : AppCompatActivity
    {
        public static string EXTRAS_ARG_DOC_IMAGE_FILE_URI = "documentImageFileUri";
        public static string EXTRAS_ARG_ORIGINAL_IMAGE_FILE_URI = "originalImageFileUri";

        private static string LOG_TAG = nameof(CameraXViewDemoActivity);
        private ScanbotCameraXView cameraView;
        private DocumentAutoSnappingController autoSnappingController;
        private PolygonView polygonView;
        private bool flashEnabled = false;
        private bool autoSnappingEnabled = true;
        private readonly bool ignoreBadAspectRatio = true;
        private TextView userGuidanceTextView;
        private long lastUserGuidanceHintTs = 0L;
        private ProgressBar imageProcessingProgress;
        private ShutterButton shutterButton;
        private Button autoSnappingToggleButton;

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SupportRequestWindowFeature(WindowCompat.FeatureActionBarOverlay);
            base.OnCreate(savedInstanceState);

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);

            SetContentView(Resource.Layout.CameraXViewDemo);

            SupportActionBar.Hide();

            cameraView = FindViewById<ScanbotCameraXView>(Resource.Id.scanbotCameraView);

            // In this example we demonstrate how to lock the orientation of the UI (Activity)
            // as well as the orientation of the taken picture to portrait.
            cameraView.LockToPortrait(true);

            // Uncomment to disable AutoFocus by manually touching the camera view:
            cameraView.SetAutoFocusOnTouch(false);

            // Preview Mode: See https://github.com/doo/Scanbot-SDK-Examples/wiki/Using-ScanbotCameraView#preview-mode
            cameraView.SetPreviewMode(CameraPreviewMode.FitIn);

            userGuidanceTextView = FindViewById<TextView>(Resource.Id.userGuidanceTextView);

            imageProcessingProgress = FindViewById<ProgressBar>(Resource.Id.imageProcessingProgress);

            var contourDetector = scanbotSDK.CreateContourDetector();
            var frameHandlerWrapper = new ContourDetectorFrameHandlerWrapper(cameraView.Context, contourDetector);
            frameHandlerWrapper.AddResultHandler(ShowUserGuidance);
            cameraView.Attach(frameHandlerWrapper);
            ScanbotCameraXViewWrapper.Attach(cameraView, frameHandlerWrapper);

            // Add an additional custom contour detector to add user guidance text
            polygonView = FindViewById<PolygonView>(Resource.Id.scanbotPolygonView);
            polygonView.SetStrokeColor(Color.Red);
            polygonView.SetStrokeColorOK(Color.Green);

            // Attach the default polygon result handler, to draw the default polygon
            frameHandlerWrapper.FrameHandler.AddResultHandler(polygonView.ContourDetectorResultHandler);

            autoSnappingController = DocumentAutoSnappingController.Attach(cameraView, contourDetector);

            cameraView.AddPictureCallback(new PictureCallback(ProcessTakenPicture));
            cameraView.SetCameraOpenCallback(new CameraOpenCallback(() =>
            {
                cameraView.PostDelayed(() =>
                {
                    // Uncomment to disable shutter sound (supported since Android 4.2+):
                    // Please note that some devices may not allow disabling the camera shutter sound. 
                    // If the shutter sound state cannot be set to the desired value, this method will be ignored.
                    cameraView.SetShutterSound(false);
                    // Enable ContinuousFocus mode:
                    cameraView.ContinuousFocus();
                }, 500);
            }));

            shutterButton = FindViewById<ShutterButton>(Resource.Id.shutterButton);
            shutterButton.Click += delegate
            {
                cameraView.TakePicture(false);
            };
            shutterButton.Visibility = ViewStates.Visible;

            FindViewById(Resource.Id.scanbotFlashButton).Click += delegate
            {
                cameraView.UseFlash(!flashEnabled);
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

        private bool ShowUserGuidance(ContourDetectorFrameHandler.DetectedFrame frame, SdkLicenseError error)
        {
            if (!autoSnappingEnabled) { return false; }

            if (Java.Lang.JavaSystem.CurrentTimeMillis() - lastUserGuidanceHintTs < 400)
            {
                return false;
            }

            var color = Color.Red;
            var guideText = "";

            var result = frame.DetectionStatus;
            if (result == DetectionStatus.Ok)
            {
                guideText = "Don't move.\nCapturing...";
                color = Color.Green;
            }
            else if (result == DetectionStatus.OkButTooSmall)
            {
                guideText = "Move closer";
            }
            else if (result == DetectionStatus.OkButBadAngles)
            {
                guideText = "Perspective";
            }
            else if (result == DetectionStatus.OkButBadAspectRatio)
            {
                guideText = "Wrong aspect ratio.\n Rotate your device";
                if (ignoreBadAspectRatio)
                {
                    guideText = "Don't move.\nCapturing...";
                    color = Color.Green;
                }
            }
            else if (result == DetectionStatus.ErrorNothingDetected)
            {
                guideText = "No Document";
            }
            else if (result == DetectionStatus.ErrorTooNoisy)
            {
                guideText = "Background too noisy";
            }
            else if (result == DetectionStatus.ErrorTooDark)
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

        private void ProcessTakenPicture(byte[] image, int imageOrientation, IList<PointF> finderRect)
        {
            // Here we get the full image from the camera and apply document detection on it.
            // Implement a suitable async(!) detection and image handling here.
            // This is just a demo showing detected image as downscaled preview image.

            Log.Debug(LOG_TAG, "OnPictureTaken: imageOrientation = " + imageOrientation);

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
            if (imageOrientation > 0)
            {
                Matrix matrix = new Matrix();
                matrix.SetRotate(imageOrientation, originalBitmap.Width / 2f, originalBitmap.Height / 2f);
                originalBitmap = Bitmap.CreateBitmap(originalBitmap, 0, 0, originalBitmap.Width, originalBitmap.Height, matrix, false);
            }

            // Store the original image as file:
            var originalImgUri = TempImageStorage.Instance.AddImage(originalBitmap);

            Android.Net.Uri documentImgUri = null;
            // Run document detection on original image:
            var detector = scanbotSDK.CreateContourDetector();
            var sdkDetectionResult = detector.Detect(originalBitmap);
            //  var detectionResult = ScanbotSDK.MAUI.Native.Droid.ScanbotSDK.DetectDocument(originalBitmap);
            if (sdkDetectionResult.Status == DetectionStatus.Ok)
            {
                if (sdkDetectionResult.PolygonF != null)
                {
                    var resultImage = scanbotSDK.ImageProcessor().ProcessBitmap(originalBitmap, new CropOperation(sdkDetectionResult.PolygonF));
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
            return;

            /* If you want to continue scanning:
            RunOnUiThread(() => {
                // continue camera preview
                cameraView.StartPreview();
                cameraView.ContinuousFocus();
            });
            */
        }

        protected void SetAutoSnapEnabled(bool enabled)
        {
            autoSnappingController.Enabled = enabled;
            //contourDetectorFrameHandler.Enabled = enabled;
            polygonView.Visibility = (enabled ? ViewStates.Visible : ViewStates.Gone);
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

    public class PictureCallback : Java.Lang.Object, IBasePictureCallback
    {
        private Action<byte[], int, IList<PointF>> PictureTaken;

        public PictureCallback(Action<byte[], int, IList<PointF>> pictureTaken)
        {
            PictureTaken = pictureTaken;
        }

        public void OnPictureTakenInternal(byte[] image, int imageOrientation, IList<PointF> finderRect, bool isCapturedAutomatically)
        {
            PictureTaken?.Invoke(image, imageOrientation, finderRect);
        }
    }

    public class CameraOpenCallback : Java.Lang.Object, ICameraOpenCallback
    {
        private Action _cameraOpened;

        public CameraOpenCallback(Action cameraOpened)
        {
            _cameraOpened = cameraOpened;
        }

        public void OnCameraOpened()
        {
            _cameraOpened?.Invoke();
        }
    }
}