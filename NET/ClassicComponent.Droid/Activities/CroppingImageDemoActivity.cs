using Android.Graphics;
using Android.Views;
using Android.Content;

using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.UI;
using IO.Scanbot.Sdk.Process;
using System.Diagnostics;
using IO.Scanbot.Sdk.Document;

namespace ClassicComponent.Droid
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class CroppingImageDemoActivity : AppCompatActivity
    {
        public static string EXTRAS_ARG_IMAGE_FILE_URI = "EXTRAS_ARG_IMAGE_FILE_URI";

        private static IList<PointF> defaultPolygon = [
            new PointF(0, 0),
            new PointF(1, 0),
            new PointF(1f, 1f),
            new PointF(0, 1)
        ];

        private IO.Scanbot.Sdk.ScanbotSDK SDK;
        private Android.Net.Uri imageUri;
        private Bitmap originalBitmap;
        private EditPolygonImageView editPolygonImageView;
        private MagnifierView scanbotMagnifierView;
        private ProgressBar processImageProgressBar;
        private View cancelBtn, doneBtn, rotateCWButton;

        private int rotationDegrees = 0;
        private long lastRotationEventTs = 0L;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SDK = new IO.Scanbot.Sdk.ScanbotSDK(this);

            SetContentView(Resource.Layout.CroppingImageDemo);

            SupportActionBar.SetDisplayShowHomeEnabled(false);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayShowCustomEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetCustomView(Resource.Layout.CroppingActionBarView);

            editPolygonImageView = FindViewById<EditPolygonImageView>(Resource.Id.scanbotEditImageView);
            processImageProgressBar = FindViewById<ProgressBar>(Resource.Id.processImageProgressBar);
            scanbotMagnifierView = FindViewById<MagnifierView>(Resource.Id.scanbotMagnifierView);

            cancelBtn = FindViewById<View>(Resource.Id.cancelButton);
            cancelBtn.Click += delegate
            {
                Finish();
            };

            doneBtn = FindViewById<View>(Resource.Id.doneButton);
            doneBtn.Click += delegate
            {
                cancelBtn.Enabled = false;
                doneBtn.Enabled = false;
                rotateCWButton.Enabled = false;
                CropAndSaveImage();
            };

            rotateCWButton = FindViewById<View>(Resource.Id.rotateCWButton);
            rotateCWButton.Click += delegate
            {
                if ((Java.Lang.JavaSystem.CurrentTimeMillis() - lastRotationEventTs) < 350)
                {
                    return;
                }
                rotationDegrees += 90;
                editPolygonImageView.RotateClockwise();
                lastRotationEventTs = Java.Lang.JavaSystem.CurrentTimeMillis();
            };

            var resizedBitmap = await Task.Run(() =>
            {
                string imageFileUri = Intent.Extras.GetString(EXTRAS_ARG_IMAGE_FILE_URI);
                imageUri = Android.Net.Uri.Parse(imageFileUri);

                originalBitmap = new ImageLoader(this).Load(imageUri);
                return ImageUtils.ResizeImage(originalBitmap, 1000, 1000);
            });

            var polygon = defaultPolygon;
            var detector = SDK.CreateDocumentScanner();
            // Since we just need detected polygon and lines here, we use ContourDetector class from the native SDK namespace.
            var detectionResult = detector.ScanFromBitmap(resizedBitmap);
            var detectionStatus = detectionResult.Status;
            if (detectionStatus == DocumentDetectionStatus.Ok || detectionStatus == DocumentDetectionStatus.OkButBadAngles ||
                detectionStatus == DocumentDetectionStatus.OkButTooSmall || detectionStatus == DocumentDetectionStatus.OkButBadAspectRatio)
            {
                polygon = detectionResult.PointsNormalized;
                Debug.WriteLine("Detected polygon: " + polygon);
            }

            editPolygonImageView.Polygon = polygon;
            // todo: check LineSegmentInt to LineSegmentFloat
            // editPolygonImageView.SetLines(detectionResult.HorizontalLines, detectionResult.VerticalLines);
            // important! first set the image and then the detected polygon and lines!
            editPolygonImageView.SetImageBitmap(resizedBitmap);
            // set up the MagnifierView every time when editPolygonView is set with a new image.
            scanbotMagnifierView.SetupMagnifier(editPolygonImageView);
        }

        private async void CropAndSaveImage()
        {
            processImageProgressBar.Visibility = ViewStates.Visible;
            cancelBtn.Visibility = ViewStates.Gone;
            doneBtn.Visibility = ViewStates.Gone;
            rotateCWButton.Visibility = ViewStates.Gone;

            var documentImgUri = await Task.Run(() =>
            {
                var detector = SDK.CreateDocumentScanner();
                var documentImage = new ImageProcessor(originalBitmap).Crop(editPolygonImageView.Polygon).ProcessedBitmap();
                var matrix = new Matrix();
                matrix.PostRotate(rotationDegrees);
                documentImage = Bitmap.CreateBitmap(documentImage, 0, 0, documentImage.Width, documentImage.Height, matrix, true);

                return TempImageStorage.Instance.AddImage(documentImage);
            });

            var extras = new Bundle();
            extras.PutString(EXTRAS_ARG_IMAGE_FILE_URI, documentImgUri.ToString());
            var intent = new Intent();
            intent.PutExtras(extras);
            SetResult(Result.Ok, intent);
            Finish();   
        }
    }
}
