using Android.Content;
using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.UI;
using IO.Scanbot.Sdk.UI.Camera;

namespace ClassicComponent.Droid.Activities
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class CheckRecognizerDemoActivity : AppCompatActivity
    {
        private ScanbotCameraXView cameraView;
        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private FrameHandler checkFrameHandler;
        private bool isFlashEnabled = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityCheckScanner);

            cameraView = FindViewById<ScanbotCameraXView>(Resource.Id.camera);
            cameraView.SetPreviewMode(CameraPreviewMode.FitIn);
            cameraView.SetCameraOpenCallback(new CameraCallBack(() =>
            {
                cameraView.PostDelayed(() =>
                {
                    cameraView.UseFlash(isFlashEnabled);
                    cameraView.ContinuousFocus();
                }, 700);
            }));

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);

            var checkScanner = scanbotSDK.CreateCheckScanner();

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            var checkFrameHandlerWrapper = new CheckScannerFrameHandlerWrapper(checkScanner);
            checkFrameHandler = checkFrameHandlerWrapper.FrameHandler;
            checkFrameHandlerWrapper.AddResultHandler(new CheckScannResultHandler(this, checkFrameHandler));
            ScanbotCameraXViewWrapper.Attach(cameraView, checkFrameHandlerWrapper);

            FindViewById<Button>(Resource.Id.flash).Click += (_, _) =>
            {
                this.isFlashEnabled = !this.isFlashEnabled;
                this.cameraView.UseFlash(this.isFlashEnabled);
            };

            Toast.MakeText(
                this,
                scanbotSDK.LicenseInfo.IsValid ? "License is valid" : "License Expired",
                ToastLength.Long
            )?.Show();
        }


        protected override void OnResume()
        {
            base.OnResume();
            checkFrameHandler.Enabled = true;
        }

        public static Intent NewIntent(Context context)
        {
            return new Intent(context, typeof(CheckRecognizerDemoActivity));
        }
    }

    public class CameraCallBack : Java.Lang.Object, ICameraOpenCallback
    {
        private Action _cameraOpened;

        public CameraCallBack(Action cameraOpened)
        {
            _cameraOpened = cameraOpened;
        }

        public void OnCameraOpened()
        {
            _cameraOpened?.Invoke();
        }
    }

    public class CheckScannResultHandler : CheckScannerResultHandlerWrapper
    {
        private Context _context;
        private FrameHandler _frameHandler;

        public CheckScannResultHandler(Context context, FrameHandler frameHandler) 
        {
            _context = context;
            _frameHandler = frameHandler;
        }

        public override bool HandleResult(CheckScanningResult result, SdkLicenseError error)
        {
            if (result?.Status == IO.Scanbot.Sdk.Check.CheckMagneticInkStripScanningStatus.Success)
            {
                _frameHandler.Enabled = false;
                _context.StartActivity(CheckRecognizerResultActivity.NewIntent(_context, result));
            }

            return false;
        }
    }
}