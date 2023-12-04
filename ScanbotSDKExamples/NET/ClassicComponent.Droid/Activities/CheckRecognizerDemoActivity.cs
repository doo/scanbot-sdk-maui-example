using Android.Content;
using AndroidX.AppCompat.App;

using IO.Scanbot.Sdk;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Check;
using IO.Scanbot.Sdk.Check.Entity;
using IO.Scanbot.Sdk.UI.Camera;

namespace ClassicComponent.Droid.Activities
{
    [Activity(Theme = "@style/Theme.AppCompat")]
    public class CheckRecognizerDemoActivity : AppCompatActivity
    {
        private ScanbotCameraXView cameraView;
        private TextView resultView;
        private CheckRecognizerFrameHandlerWrapper checkFrameHandlerWrapper;
        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private bool isFlashEnabled = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ActivityCheckRecognizer);

            cameraView = FindViewById<ScanbotCameraXView>(Resource.Id.camera);
            cameraView.SetPreviewMode(CameraPreviewMode.FitIn);
            cameraView.SetCameraOpenCallback(() =>
            {
                cameraView.PostDelayed(() =>
                {
                    cameraView.UseFlash(isFlashEnabled);
                    cameraView.ContinuousFocus();
                }, 700);
            });

            resultView = FindViewById<TextView>(Resource.Id.result);
            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            checkFrameHandlerWrapper = new CheckRecognizerFrameHandlerWrapper(scanbotSDK.CreateCheckRecognizer());
            checkFrameHandlerWrapper.AddResultHandler(HandleCheckResult);

            cameraView.Attach(checkFrameHandlerWrapper);

            FindViewById<Button>(Resource.Id.flash).Click += (_, _) =>
            {
                this.isFlashEnabled = !this.isFlashEnabled;
                this.cameraView.UseFlash(this.isFlashEnabled);
            };

            Toast.MakeText(
                this,
                scanbotSDK.LicenseInfo.IsValid ? "License is valid" : "License Expired",
                ToastLength.Long
            ).Show();
        }


        private bool HandleCheckResult(CheckRecognizerResult result, SdkLicenseError error)
        {
            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                checkFrameHandlerWrapper.FrameHandler.Enabled = false;
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "License is expired", ToastLength.Long).Show();
                    Finish();
                });
                return false;
            }

            if (result.Status == IO.Scanbot.Check.Model.CheckRecognizerStatus.Success)
            {
                checkFrameHandlerWrapper.FrameHandler.Enabled = false;
                StartActivity(CheckRecognizerResultActivity.NewIntent(this, result));
            }

            return false;
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (checkFrameHandlerWrapper?.FrameHandler != null)
            {
                checkFrameHandlerWrapper.FrameHandler.Enabled = true;
            }
        }

        public static Intent NewIntent(Context context)
        {
            return new Intent(context, typeof(CheckRecognizerDemoActivity));
        }
    }
}