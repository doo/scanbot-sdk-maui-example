using Android.Graphics;
using ScanbotSdkExample.Droid.Utils;

namespace ScanbotSdkExample.Droid.Activities;

[Activity]
public class ImageResultActivity : AndroidX.AppCompat.App.AppCompatActivity
{
      private ImageView _imageView;
      protected async override void OnCreate(Bundle savedInstanceState)
      {
            base.OnCreate(savedInstanceState);

            _imageView = new ImageView(this);
            _imageView.SetScaleType(ImageView.ScaleType.FitCenter);

            var imageBytes = Intent?.GetByteArrayExtra("image_bytes");

            if (imageBytes != null)
            {
                  var bitmap = await imageBytes.ByteArrayToBitmap();
                  _imageView.SetImageBitmap(bitmap);
            }

            SetContentView(_imageView);
            Window?.DecorView?.SetBackgroundColor(Android.Graphics.Color.White);
      }
}