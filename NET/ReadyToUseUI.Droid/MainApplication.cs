using Android.Runtime;
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
        public const string LicenseKey =   "KGO0GonuTDTEkO8dfW8mFww63sBu6q" +
  "/nHu5SkZsIe+dG4Uy68FFFUgi72PCz" +
  "W24kq7BR5ENcyPhrSedhiJYtibUEfh" +
  "9jKMB82xyIyIsz5WT6fpwFRWg1Y/OT" +
  "SnOf2eznVycM8t3c5P+Kw/MTERRWvh" +
  "zFnZ4AwkmIfDUGkxfG1D219Lfu4iCs" +
  "ZHyX5CvldzKJActqd2jMFoq5exW2ST" +
  "6VkCo8ssAEJqJ9eO/F9zJJipxqWtJg" +
  "MwXcLAD1HHfQldRN4uW97WgawQDGd+" +
  "MEFHSa2Ylt2kzqtle1GzZ66jyHOZow" +
  "y2bCuu0X7pDLJ2eUKQNFEfgopXhwFI" +
  "U04SCWSJ03Og==\nU2NhbmJvdFNESw" +
  "pkb28uc2NhbmJvdC5jYXBhY2l0b3Iu" +
  "ZXhhbXBsZXxpby5zY2FuYm90LmV4YW" +
  "1wbGUuZG9jdW1lbnQudXNlY2FzZXMu" +
  "YW5kcm9pZHxpby5zY2FuYm90LmV4YW" +
  "1wbGUuZG9jdW1lbnRzZGsudXNlY2Fz" +
  "ZXMuaW9zfGlvLnNjYW5ib3QuZXhhbX" +
  "BsZS5mbHV0dGVyfGlvLnNjYW5ib3Qu" +
  "ZXhhbXBsZS5zZGsuYW5kcm9pZHxpby" +
  "5zY2FuYm90LmV4YW1wbGUuc2RrLmJh" +
  "cmNvZGUuYW5kcm9pZHxpby5zY2FuYm" +
  "90LmV4YW1wbGUuc2RrLmJhcmNvZGUu" +
  "Y2FwYWNpdG9yfGlvLnNjYW5ib3QuZX" +
  "hhbXBsZS5zZGsuYmFyY29kZS5mbHV0" +
  "dGVyfGlvLnNjYW5ib3QuZXhhbXBsZS" +
  "5zZGsuYmFyY29kZS5pb25pY3xpby5z" +
  "Y2FuYm90LmV4YW1wbGUuc2RrLmJhcm" +
  "NvZGUubWF1aXxpby5zY2FuYm90LmV4" +
  "YW1wbGUuc2RrLmJhcmNvZGUubmV0fG" +
  "lvLnNjYW5ib3QuZXhhbXBsZS5zZGsu" +
  "YmFyY29kZS5yZWFjdG5hdGl2ZXxpby" +
  "5zY2FuYm90LmV4YW1wbGUuc2RrLmJh" +
  "cmNvZGUud2luZG93c3xpby5zY2FuYm" +
  "90LmV4YW1wbGUuc2RrLmJhcmNvZGUu" +
  "eGFtYXJpbnxpby5zY2FuYm90LmV4YW" +
  "1wbGUuc2RrLmJhcmNvZGUueGFtYXJp" +
  "bi5mb3Jtc3xpby5zY2FuYm90LmV4YW" +
  "1wbGUuc2RrLmNhcGFjaXRvcnxpby5z" +
  "Y2FuYm90LmV4YW1wbGUuc2RrLmNhcG" +
  "FjaXRvci5hbmd1bGFyfGlvLnNjYW5i" +
  "b3QuZXhhbXBsZS5zZGsuY2FwYWNpdG" +
  "9yLmlvbmljfGlvLnNjYW5ib3QuZXhh" +
  "bXBsZS5zZGsuY2FwYWNpdG9yLmlvbm" +
  "ljLnJlYWN0fGlvLnNjYW5ib3QuZXhh" +
  "bXBsZS5zZGsuY2FwYWNpdG9yLmlvbm" +
  "ljLnZ1ZWpzfGlvLnNjYW5ib3QuZXhh" +
  "bXBsZS5zZGsuY29yZG92YS5pb25pY3" +
  "xpby5zY2FuYm90LmV4YW1wbGUuc2Rr" +
  "LmZsdXR0ZXJ8aW8uc2NhbmJvdC5leG" +
  "FtcGxlLnNkay5pb3MuYmFyY29kZXxp" +
  "by5zY2FuYm90LmV4YW1wbGUuc2RrLm" +
  "lvcy5jbGFzc2ljfGlvLnNjYW5ib3Qu" +
  "ZXhhbXBsZS5zZGsuaW9zLnJ0dXVpfG" +
  "lvLnNjYW5ib3QuZXhhbXBsZS5zZGsu" +
  "bWF1aXxpby5zY2FuYm90LmV4YW1wbG" +
  "Uuc2RrLm1hdWkucnR1fGlvLnNjYW5i" +
  "b3QuZXhhbXBsZS5zZGsubmV0fGlvLn" +
  "NjYW5ib3QuZXhhbXBsZS5zZGsucmVh" +
  "Y3RuYXRpdmV8aW8uc2NhbmJvdC5leG" +
  "FtcGxlLnNkay5yZWFjdC5uYXRpdmV8" +
  "aW8uc2NhbmJvdC5leGFtcGxlLnNkay" +
  "5ydHUuYW5kcm9pZHxpby5zY2FuYm90" +
  "LmV4YW1wbGUuc2RrLnhhbWFyaW58aW" +
  "8uc2NhbmJvdC5leGFtcGxlLnNkay54" +
  "YW1hcmluLmZvcm1zfGlvLnNjYW5ib3" +
  "QuZXhhbXBsZS5zZGsueGFtYXJpbi5y" +
  "dHV8aW8uc2NhbmJvdC5mb3Jtcy5uYX" +
  "RpdmVyZW5kZXJlcnMuZXhhbXBsZXxp" +
  "by5zY2FuYm90Lm5hdGl2ZWJhcmNvZG" +
  "VzZGtyZW5kZXJlcnxpby5zY2FuYm90" +
  "LlNjYW5ib3RTREtTd2lmdFVJRGVtb3" +
  "xpby5zY2FuYm90LnNka193cmFwcGVy" +
  "LmRlbW8uYmFyY29kZXxpby5zY2FuYm" +
  "90LnNkay13cmFwcGVyLmRlbW8uYmFy" +
  "Y29kZXxpby5zY2FuYm90LnNka193cm" +
  "FwcGVyLmRlbW8uZG9jdW1lbnR8aW8u" +
  "c2NhbmJvdC5zZGstd3JhcHBlci5kZW" +
  "1vLmRvY3VtZW50fGlvLnNjYW5ib3Qu" +
  "c2RrLmludGVybmFsZGVtb3xsb2NhbG" +
  "hvc3R8T3BlcmF0aW5nU3lzdGVtU3Rh" +
  "bmRhbG9uZXxzY2FuYm90c2RrLXFhLT" +
  "EuczMtZXUtd2VzdC0xLmFtYXpvbmF3" +
  "cy5jb218c2NhbmJvdHNkay1xYS0yLn" +
  "MzLWV1LXdlc3QtMS5hbWF6b25hd3Mu" +
  "Y29tfHNjYW5ib3RzZGstcWEtMy5zMy" +
  "1ldS13ZXN0LTEuYW1hem9uYXdzLmNv" +
  "bXxzY2FuYm90c2RrLXFhLTQuczMtZX" +
  "Utd2VzdC0xLmFtYXpvbmF3cy5jb218" +
  "c2NhbmJvdHNkay1xYS01LnMzLWV1LX" +
  "dlc3QtMS5hbWF6b25hd3MuY29tfHNj" +
  "YW5ib3RzZGstd2FzbS1kZWJ1Z2hvc3" +
  "QuczMtZXUtd2VzdC0xLmFtYXpvbmF3" +
  "cy5jb218d2Vic2RrLWRlbW8taW50ZX" +
  "JuYWwuc2NhbmJvdC5pb3wqLnFhLnNj" +
  "YW5ib3QuaW8KMTczMzA5NzU5OQo4Mz" +
  "g4NjA3CjMx\n";

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

          

            // You can enable encryption by uncommenting the following lines:
            //initializer.UseFileEncryption(enableFileEncryption: USE_ENCRYPTION, new AESEncryptedFileIOProcessor(
            //        "S0m3W3irDL0ngPa$$w0rdino!!!!",
            //        AESEncryptedFileIOProcessor.AESEncrypterMode.Aes256
            //    ));
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
