using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Constants;
using ScanbotSDK.MAUI.Models;

namespace ReadyToUseUI.Maui
{
    public static partial class MauiProgram
    {
        private static partial string StorageBaseDirectoryForExampleApp()
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
            //       var directory = GetExternalFilesDir(null).AbsolutePath;
            //       var externalPublicPath = Path.Combine(directory, "my-custom-storage");
            //      Directory.CreateDirectory(externalPublicPath);
            //     return externalPublicPath;

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = Path.Combine(documents, "maui-dev-app-storage");
            Directory.CreateDirectory(folder);

            return folder;
        }
    }
}

