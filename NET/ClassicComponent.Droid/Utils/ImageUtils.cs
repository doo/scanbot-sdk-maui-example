using Android.Graphics;
using IO.Scanbot.Sdk.Persistence.Fileio;
using IO.Scanbot.Sdk.Util;

namespace ClassicComponent.Droid
{
    public class ImageUtils
    {
        public static string[] GetSelectedImagesAsFilePaths(List<Android.Net.Uri> selectedImages, Activity activity)
        {
            var result = new List<string>();
            foreach (var androidUri in selectedImages)
            {
                result.Add(FileChooserUtils.GetPath(activity, androidUri, new DefaultFileIOProcessor(activity)));
            }
            return result.ToArray();
        }

        public static Bitmap GetThumbnail(Bitmap originalImage, float width, float height)
        {
            byte[] imageData = ResizeImage(originalImage, width, height, 70);
            return BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
        }

        public static Bitmap ResizeImage(Bitmap originalImage, float width, float height)
        {
            byte[] imageData = ResizeImage(originalImage, width, height, 70);
            return BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
        }

        public static byte[] ResizeImage(Bitmap originalImage, float width, float height, int quality)
        {
            float oldWidth = originalImage.Width;
            float oldHeight = originalImage.Height;
            float scaleFactor = 0f;

            if (oldWidth > oldHeight)
            {
                scaleFactor = width / oldWidth;
            }
            else
            {
                scaleFactor = height / oldHeight;
            }

            float newHeight = oldHeight * scaleFactor;
            float newWidth = oldWidth * scaleFactor;

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                return ms.ToArray();
            }
        }
    }
}
