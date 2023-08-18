using Android.App;
using Android.Runtime;
using DocumentSDK.MAUI;
using DocumentSDK.MAUI.Droid;

namespace ReadyToUseUI.Maui;

[Application]
public class MainApplication : MauiApplication
{

    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => CreateMauiInstance();// MauiProgram.CreateMauiApp();

    private MauiApp CreateMauiInstance()
    {
        Task.Run(async () =>
        {
            try
            {
                //await Task.Delay(5000);
                var configuration = new SBSDKConfiguration
                {
                    EnableLogging = true,
                    // If no StorageBaseDirectory is specified, the default will be used
                    StorageBaseDirectory = GetDemoStorageBaseDirectory(),
                    AllowGpuAcceleration = false,
                    AllowXnnpackAcceleration = false,
                    EnableNativeLogging = true
                };

                SBSDKInitializer.Initialize(this, App.LICENSE_KEY, configuration);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
        });

        return MauiProgram.CreateMauiApp();
    }


    string GetDemoStorageBaseDirectory()
    {
        /** !!Please note!!

         * In this demo app we overwrite the "StorageBaseDirectory"
         * of the Scanbot SDK by a custom public (!) storage directory.
         * "GetExternalFilesDir" returns an external, public (!) storage directoy.
         * All image files as well export files (PDF, TIFF, etc) created
         * by the Scanbot SDK in this demo app will be stored in a sub-folder
         * of this storage directory and will be accessible
         * for every(!) app having external storage permissions!

         * We use the "ExternalStorageDirectory" here only for demo purposes,
         * to be able to share generated PDF and TIFF files.
         * (also see the example code for PDF and TIFF creation).

         * If you need a secure storage for all images
         * and export files (which is strongly recommended):
            - Use the default settings of the Scanbot SDK (don't overwrite
              the "StorageBaseDirectory" config parameter above)
            - Set a suitable custom internal (!) StorageBaseDirectory.

         * For more detais about the Android file system see:
            - https://developer.android.com/guide/topics/data/data-storage
            - https://docs.microsoft.com/en-us/xamarin/android/platform/files/
        */
        var directory = GetExternalFilesDir(null).AbsolutePath;
        var externalPublicPath = Path.Combine(directory, "my-custom-storage");
        Directory.CreateDirectory(externalPublicPath);
        return externalPublicPath;
    }
}