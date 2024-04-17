using Android.Content;
using Android.Graphics;
using Android.Provider;

namespace ReadyToUseUI.Droid.Utils
{
    public class ImageUtils
    {
        public static Bitmap ProcessGalleryResult(Context context, Intent data)
        {
            // TODO: What in the world is the correct way to import images these days?
            return MediaStore.Images.Media.GetBitmap(context.ContentResolver, data.Data);
        }
    }
}
