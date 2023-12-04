using Android.Content;
using Android.Graphics;
using Android.Provider;

namespace ClassicComponent.Droid
{
    public class ImageLoader
    {
        public static ImageLoader Instance;

        private Context context;
        private BitmapFactory.Options options;
        private IO.Scanbot.Sdk.ScanbotSDK SDK;

        public ImageLoader(Context context)
        {
            this.context = context;
            options = new BitmapFactory.Options();
            SDK = new IO.Scanbot.Sdk.ScanbotSDK(context);
        }

        public Bitmap Load(Android.Net.Uri uri)
        {
            return SDK.FileIOProcessor().ReadImage(uri, options);
        }

        public Bitmap LoadFromMedia(Android.Net.Uri uri)
        {
            return MediaStore.Images.Media.GetBitmap(context.ContentResolver, uri);
        }
    }
}
