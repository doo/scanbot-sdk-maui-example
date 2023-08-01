using Android.Content;

namespace DocumentSDK.NET.Droid.Utils
{
    public class Copier
    {
        const string SNAPPING_DOCUMENTS_DIR_NAME = "snapping_documents";

        public static Java.IO.File Copy(Context context, Android.Net.Uri uri)
        {
            var path = Path.Combine(context.GetExternalFilesDir(null).AbsolutePath, SNAPPING_DOCUMENTS_DIR_NAME);
            var file = Path.Combine(path, uri.LastPathSegment);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.Copy(uri.Path, file);

            return new Java.IO.File(file);
        }
    }
}
