using Android.Content;
using Android.Graphics;

namespace ScanbotSdkExample.Droid.Utils;

public static class ImageUtils
{
    public static Bitmap ProcessGalleryResult(Context context, Intent data)
    {
        try
        {
            var uri = data?.Data;
            if (uri == null) return null;

            using var stream = context?.ContentResolver?.OpenInputStream(uri);
            if (stream == null) return null;
            return BitmapFactory.DecodeStream(stream);
        }
        catch (Exception)
        {
            return null;
        }
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