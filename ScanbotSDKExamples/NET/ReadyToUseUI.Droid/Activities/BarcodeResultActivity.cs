using AndroidX.AppCompat.App;
using IO.Scanbot.Sdk.Barcode.Entity;
using ReadyToUseUI.Droid.Model;

namespace ReadyToUseUI.Droid.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class BarcodeResultActivity : AppCompatActivity
    {
        ImageView snapImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_barcode_result);

            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeFile(
                 pathName: BarcodeResultBundle.Instance.PreviewPath ?? BarcodeResultBundle.Instance.ImagePath
            );

            snapImage = FindViewById<ImageView>(Resource.Id.snapImage);
            snapImage.SetImageBitmap(bitmap);

            ShowBarcodeResult(BarcodeResultBundle.Instance.ScanningResult);
        }

        void ShowBarcodeResult(BarcodeScanningResult result)
        {
            var parent = FindViewById<LinearLayout>(Resource.Id.recognisedItems);

            if (result == null)
            {
                return;
            }

            foreach (var item in result.BarcodeItems)
            {
                var child = LayoutInflater.Inflate(Resource.Layout.barcode_item, parent, false);

                var image = child.FindViewById<ImageView>(Resource.Id.image);
                var barFormat = child.FindViewById<TextView>(Resource.Id.barcodeFormat);
                var docFormat = child.FindViewById<TextView>(Resource.Id.docFormat);
                var docText = child.FindViewById<TextView>(Resource.Id.docText);

                if (item.Image != null)
                {
                    image.SetImageBitmap(item.Image);
                }

                barFormat.Text = "Format: " + item.BarcodeFormat.Name();

                if (item.FormattedData != null)
                {
                    docFormat.Text = item.FormattedData.ToString();
                }
                else
                {
                    docFormat.Text = "Document: –";
                }

                docText.Text = "Content: " + item.Text;

                parent.AddView(child);
            }
        }

    }
}