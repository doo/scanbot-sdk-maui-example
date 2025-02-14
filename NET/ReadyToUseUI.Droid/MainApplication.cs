﻿using Android.Runtime;
using Android.Util;

namespace ReadyToUseUI.Droid
{
    // It is strongly recommended to add the LargeHeap = true flag in your Application class.
    // Working with images, creating PDFs, etc. are memory intensive tasks. So to prevent OutOfMemoryError, consider adding this flag!
    // For more details see: http://developer.android.com/guide/topics/manifest/application-element.html#largeHeap
    [Application(LargeHeap = true)]
    public class MainApplication : Application
    {
        // Set the below to true to test our encryption functionality.
        public const bool USE_ENCRYPTION = false;
        // Note: all the images and files exported through the SDK will
        // not be openable from external applications, since they will be
        // encrypted.

        static readonly string LOG_TAG = nameof(MainApplication);

        // TODO Add the Scanbot SDK license key here.
        // Please note: The Scanbot SDK will run without a license key for one minute per session!
        // After the trial period is over all Scanbot SDK functions as well as the UI components will stop working.
        // You can get an unrestricted "no-strings-attached" 30 day trial license key for free.
        // Please submit the trial license form (https://scanbot.io/sdk/trial.html) on our website by using
        // the app identifier "io.scanbot.example.sdk.maui.rtu" of this example app.
        public const string LicenseKey = "";

        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        { }

        public override void OnCreate()
        {
            base.OnCreate();
            InitializeScanbotSdk(this);
        }

        private static void InitializeScanbotSdk(Application app)
        {
            Log.Debug(LOG_TAG, "Initializing Scanbot SDK...");
            
            var initializer = new IO.Scanbot.Sdk.ScanbotSDKInitializer();
            initializer.WithLogging(useLog: true, enableNativeLogging: false);
            initializer.SdkFilesDirectory(app, PageStoragePathForExample(app));
            initializer.License(app, LicenseKey);
            initializer.UsePageStorageSettings(new IO.Scanbot.Sdk.Persistence.Page.PageStorageSettings.Builder()
                                .ImageQuality(80)
                                .ImageFormat(IO.Scanbot.Sdk.Persistence.CameraImageFormat.Jpg)
                                .PreviewTargetMax(1500) // max size for the preview images
                                .Build());
            initializer.OcrBlobsPath(app, "SBSDKLanguageData");
            initializer.PrepareOCRLanguagesBlobs(true);
            
            // You can enable encryption by uncommenting the following lines:
            //initializer.UseFileEncryption(enableFileEncryption: USE_ENCRYPTION, new AESEncryptedFileIOProcessor(
            //       "S0m3W3irDL0ngPa$$w0rdino!!!!",
            //       AESEncryptedFileIOProcessor.AESEncrypterMode.Aes256
            //   ));
            // Note: all the images and files exported through the SDK will
            // not be openable from external applications, since they will be
            // encrypted.

            initializer.Initialize(app);
        }

        private static Java.IO.File PageStoragePathForExample(Application app)
        {
            // !! Please note !!
            // In this demo app we use the "ExternalStorageDirectory" which is a public(!) storage directory.
            // All image files as well as export files (PDF, TIFF, etc) created by this demo app will be stored
            // in a sub-folder of this storage directory and will be accessible for every(!) app having external storage permissions!
            // We use the "ExternalStorageDirectory" here only for demo purposes, to be able to share generated PDF and TIFF files.
            // (also see the example code for PDF and TIFF creation).
            // If you need a secure storage for all images and export files (which is strongly recommended) use a suitable internal(!) storage directory.
            //
            // For more detais about the Android file system see:
            // - https://developer.android.com/guide/topics/data/data-storage
            // - https://docs.microsoft.com/en-us/xamarin/android/platform/files/
            var external = app.GetExternalFilesDir(null).AbsolutePath;
            var path = Path.Combine(external, "scanbot-sdk-example-net-rtu_demo-storage");
            Directory.CreateDirectory(path);

            return new Java.IO.File(path);
        }
    }
}
