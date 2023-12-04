using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Constants;
using ScanbotSDK.MAUI.Models;
using ClassicComponent.Maui.CustomViews;

namespace ClassicComponent.Maui;

public static class MauiProgram
{
    private const string LICENSE_KEY = null;

    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			}).ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler(typeof(BarcodeCameraView), typeof(BarcodeCameraViewHandler));
			});

        SBSDKInitializer.Initialize(LICENSE_KEY, new ScanbotSDK.MAUI.SBSDKConfiguration
        {
            EnableLogging = true,
            // If no StorageBaseDirectory is specified, the default will be used
            StorageBaseDirectory = StorageBaseDirectoryForExampleApp(),
            AllowGpuAcceleration = false,
            AllowXnnpackAcceleration = false,
            EnableNativeLogging = true
        }
                );

        return builder.Build();
	}

    private static string StorageBaseDirectoryForExampleApp()
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
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var folder = Path.Combine(documents, "forms-dev-app-storage");
        Directory.CreateDirectory(folder);

        return folder;
    }
}

