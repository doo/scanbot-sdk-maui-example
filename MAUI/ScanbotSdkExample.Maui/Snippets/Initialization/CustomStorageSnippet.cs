using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Sdk;

namespace ScanbotSdkExample.Maui.Snippets.Initialization;

public class CustomStorageSnippet
{
    static void Initialize(MauiAppBuilder builder)
    {
        // NOTE: This method is provided as an example of how to override
        // the default "StorageBaseDirectory" with a custom public (!) storage directory.
        // All images and export files (PDF, TIFF, etc.) created by the Scanbot SDK
        // will be stored in a sub-folder of this directory and will be accessible
        // to every (!) app with external storage permissions.
        //
        //  If you need a secure storage for all images
        //  and export files (which is strongly recommended):
        //    - Use the default settings of the Scanbot SDK (don't overwrite
        //      the "StorageBaseDirectory" config parameter above)
        //    - Set a suitable custom internal (!) StorageBaseDirectory.

        //  For more detais about the Android file system see:
        //    - https://developer.android.com/guide/topics/data/data-storage
        //    - https://docs.microsoft.com/en-us/xamarin/android/platform/files/

        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var folder = Path.Combine(documents, "maui-dev-app-storage");
        Directory.CreateDirectory(folder);

        ScanbotSDKMain.Initialize(builder, new SdkConfiguration
        {
            LicenseKey = "",
            StorageBaseDirectory = folder
        });
    }
}