
#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Android.Media;
using Android.Graphics;
#endif

namespace ReadyToUseUI.Maui.Utils
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Rotates an image wrapped inside of an image source by parsing its EXIF header (for Android only).
        /// On iOS, rotation metadata is already handled internally by our SDK.
        /// </summary>
        public async static Task<ImageSource> RotateByExif(this ImageSource source)
        {
            if (source == null)
            {
                return source;
            }

#if ANDROID
            var orientation = Orientation.Normal;
            Bitmap bitmap = null;

            if (source is StreamImageSource streamSource)
            {
                var rawStream = await streamSource.Stream(CancellationToken.None);
                var exif = new ExifInterface(rawStream);
                orientation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)orientation);
                rawStream.Seek(0, SeekOrigin.Begin);

                if (!(orientation == Orientation.Rotate90 ||
                    orientation == Orientation.Rotate180 ||
                    orientation == Orientation.Rotate270))
                {
                    return StreamImageSource.FromStream(() => rawStream);
                }

                bitmap = await BitmapFactory.DecodeStreamAsync(rawStream);
            }
            else if (source is FileImageSource fileSource)
            {
                var exif = new ExifInterface(fileSource.File);
                orientation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)orientation);

                if (!(orientation == Orientation.Rotate90 ||
                    orientation == Orientation.Rotate180 ||
                    orientation == Orientation.Rotate270))
                {
                    return source;
                }

                bitmap = await BitmapFactory.DecodeFileAsync(fileSource.File);
            }

            if (bitmap == null)
            {
                return source;
            }

            try
            {
                var matrix = new Matrix();
                 
                switch (orientation)
                {
                    case Orientation.Rotate90:
                    {
                        matrix.SetRotate(90);
                        break;
                    }
                    case Orientation.Rotate180:
                    {
                        matrix.SetRotate(180);
                        break;
                    }
                    case Orientation.Rotate270:
                    {
                        matrix.SetRotate(270);
                        break;
                    }
                    default:
                        break;
                }          

                using (var rotatedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true))
                {
                    var tempMemoryStream = new MemoryStream();
                    await rotatedBitmap.CompressAsync(Bitmap.CompressFormat.WebpLossless, 0, tempMemoryStream);
                    source = StreamImageSource.FromStream(() => new MemoryStream(tempMemoryStream.ToArray(), false));
                }
            }
            finally
            {
                bitmap.Dispose();
            }
#endif

            return source;
	    }
	}
}

