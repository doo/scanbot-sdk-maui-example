using Android.Graphics;
using Android.Content;
using Java.Util;
using AndroidNetUri = Android.Net.Uri;
using Android.Util;
using IO.Scanbot.Sdk.Barcode.Entity;
using ClassicComponent.Droid.Activities;
using ClassicComponent.Droid.Utils;

using AndroidX.Core.Content;
using IO.Scanbot.Sdk.Documentdata;
using IO.Scanbot.Sdk.UI.Result;
using IO.Scanbot.Sdk.UI.View.Base;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Persistence.Fileio;
using IO.Scanbot.Sdk.Util.Thread;
using static IO.Scanbot.Sdk.Ocr.IOcrEngine;

namespace ClassicComponent.Droid
{
    [Activity(Label = "NET Classic Component", MainLauncher = true, Icon = "@mipmap/icon",
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        static readonly string LOG_TAG = nameof(MainActivity);

        const int REQUEST_SB_SCANNING_UI = 4711;
        const int REQUEST_SB_CROPPING_UI = 4712;
        const int BIG_THUMB_MAX_W = 800, BIG_THUMB_MAX_H = 800;

        AndroidNetUri documentImageUri, originalImageUri;

        ImageView imageView;
        Button performOcrButton;

        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);

            TempImageStorage.Init(MainApplication.USE_ENCRYPTION ? MainApplication.EncryptionFileIOProcessor : new DefaultFileIOProcessor(this));

            SetContentView(Resource.Layout.Main);

            imageView = FindViewById<ImageView>(Resource.Id.imageView);

            AssignCopyrightText();
            AssignStartCameraXButtonHandler();
            AssingCroppingUIButtonHandler();
            AssignCheckRecognizerUiButtonHandler();

            PermissionUtils.Request(this, FindViewById(Resource.Layout.Main));
        }

        void AssignCopyrightText()
        {
            var copyrightTextView = FindViewById<TextView>(Resource.Id.copyrightTextView);
            copyrightTextView.Text = "Copyright (c) " + DateTime.Now.Year.ToString() + " Scanbot SDK GmbH. All rights reserved.";
        }

        void AssignStartCameraXButtonHandler()
        {
            var scanningCameraXUIButton = FindViewById<Button>(Resource.Id.scanningCameraXUIButton);
            scanningCameraXUIButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }

                Intent intent = new Intent(this, typeof(CameraXViewDemoActivity));
                StartActivityForResult(intent, REQUEST_SB_SCANNING_UI);
            };
        }

        void AssingCroppingUIButtonHandler()
        {
            var croppingUIButton = FindViewById<Button>(Resource.Id.croppingUIButton);
            croppingUIButton.Click += delegate
            {
                if (!CheckScanbotSDKLicense()) { return; }
                if (!CheckOriginalImage()) { return; }

                Intent intent = new Intent(this, typeof(CroppingImageDemoActivity));
                intent.PutExtra(CroppingImageDemoActivity.EXTRAS_ARG_IMAGE_FILE_URI, documentImageUri?.ToString() ?? originalImageUri?.ToString());
                StartActivityForResult(intent, REQUEST_SB_CROPPING_UI);
            };
        }
        
        void AssignCheckRecognizerUiButtonHandler()
        {
            FindViewById<Button>(Resource.Id.checkUiButton).Click += delegate
            {
                StartActivity(CheckRecognizerDemoActivity.NewIntent(this));
            };
        }

        bool CheckDocumentImage()
        {
            if (documentImageUri == null)
            {
                Toast.MakeText(this, "Please snap a document image via Scanning UI or run Document Detection on an image file from the gallery", ToastLength.Long).Show();
                return false;
            }
            return true;
        }

        bool CheckOriginalImage()
        {
            if (originalImageUri == null)
            {
                Toast.MakeText(this, "Please snap a document image via Scanning UI or run Document Detection on an image file from the gallery", ToastLength.Long).Show();
                return false;
            }
            return true;
        }

        bool CheckScanbotSDKLicense()
        {
            if (scanbotSDK.LicenseInfo.IsValid)
            {
                // Trial period, valid trial license or valid production license.
                return true;
            }

            Toast.MakeText(this, "Scanbot SDK (trial) license has expired!", ToastLength.Long)?.Show();
            return false;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == REQUEST_SB_SCANNING_UI && resultCode == Result.Ok)
            {
                documentImageUri = AndroidNetUri.Parse(data.GetStringExtra(CameraXViewDemoActivity.EXTRAS_ARG_DOC_IMAGE_FILE_URI));
                originalImageUri = AndroidNetUri.Parse(data.GetStringExtra(CameraXViewDemoActivity.EXTRAS_ARG_ORIGINAL_IMAGE_FILE_URI));
                ShowImageView(new ImageLoader(this).Load(documentImageUri));
                return;
            }

            if (requestCode == REQUEST_SB_CROPPING_UI && resultCode == Result.Ok)
            {
                documentImageUri = AndroidNetUri.Parse(data.GetStringExtra(CroppingImageDemoActivity.EXTRAS_ARG_IMAGE_FILE_URI));
                ShowImageView(new ImageLoader(this).Load(documentImageUri));
                return;
            }
        }
        
        void ShowImageView(Bitmap bitmap)
        {
            imageView.Post(() =>
            {
                var thumb = ImageUtils.GetThumbnail(bitmap, BIG_THUMB_MAX_W, BIG_THUMB_MAX_H);
                imageView.SetImageBitmap(thumb);
            });
        }


        Java.IO.File GenerateRandomFileInDemoTempStorage(string fileExtension)
        {
            var targetFile = System.IO.Path.Combine(
                TempImageStorage.Instance.TempDir, UUID.RandomUUID() + fileExtension);
            return new Java.IO.File(targetFile);
        }

        AndroidNetUri GenerateRandomFileUrlInDemoTempStorage(string fileExtension)
        {
            return AndroidNetUri.FromFile(GenerateRandomFileInDemoTempStorage(fileExtension));
        }

        void ShowAlertDialog(string message, string title = "Info", Action onDismiss = null)
        {
            RunOnUiThread(() =>
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle(title);
                builder.SetMessage(message);
                var alert = builder.Create();
                alert.SetButton("OK", (c, ev) =>
                {
                    alert.Dismiss();
                    onDismiss?.Invoke();
                });
                alert.Show();
            });
        }
    }
}