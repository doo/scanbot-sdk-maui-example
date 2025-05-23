using Android.Content;
using Android.Graphics;
using Android.Provider;

namespace ScanbotSdkExample.Droid.Utils
{
    public class ImageUtils
    {
        public static Bitmap ProcessGalleryResult(Context context, Intent data)
        {
            // TODO: What in the world is the correct way to import images these days?
            return MediaStore.Images.Media.GetBitmap(context.ContentResolver, data.Data);
        }

        public static byte[] ConvertToByteArray(IList<Java.Lang.Byte> rawBytes)
        {
            byte[] byteArray = new byte[rawBytes.Count];
            for (int i = 0; i < rawBytes.Count; i++)
            {
                byteArray[i] = (byte)rawBytes[i].ByteValue();
            }
            return byteArray;
        }
    }
}
