using Android.Content;
using Android.Graphics;
using Android.Views;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration;
using Java.Lang;

namespace ReadyToUseUI.Droid.Activities.V2
{
    [Activity(Theme = "@style/AppTheme")]
    public class BarcodeResultActivity : BaseResultActivity<BarcodeScannerResult>
    {
        protected override BaseBarcodeResult<BarcodeScannerResult> DisplayBarcodeResult()
        {
            var barcodeResult = base.DisplayBarcodeResult();
            ShowBarcodeResult(barcodeResult.ScanningResult);

            return barcodeResult;
        }
        
        private void ShowBarcodeResult(BarcodeScannerResult result)
        {
            var parent = FindViewById<LinearLayout>(Resource.Id.recognizedItems);

            if (result == null)
                return;

            foreach (var item in result.Items)
            {
                View child = LayoutInflater.Inflate(Resource.Layout.barcode_item, parent, false);
                InitItemData(child, item);
                
                parent.AddView(child);
                parent.AddView(GetSeparatorView());
            }
        }

        private void InitItemData(View child, BarcodeItem item)
        {
            var image = child.FindViewById<ImageView>(Resource.Id.image);
            var barFormat = child.FindViewById<TextView>(Resource.Id.barcodeFormat);
            var docFormat = child.FindViewById<TextView>(Resource.Id.docFormat);
            var docText = child.FindViewById<TextView>(Resource.Id.docText);

            if (item.RawBytes != null)
            {   
                byte[] byteArray = item.RawBytes; // Utils.ImageUtils.ConvertToByteArray(
                Bitmap bitmap = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);
            
                image.SetImageBitmap(bitmap);
            }

            barFormat.Text = "Format: " + item.Type?.Name();
            docFormat.Text = ParseData(item.ParsedDocument);
            docText.Text = "Content: " + item.Text;

            child.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(DetailedItemDataActivity));
                intent.PutExtra("SelectedBarcodeItem", item);
                StartActivity(intent);
            };
        } 
        
        private string ParseData(IO.Scanbot.Genericdocument.Entity.GenericDocument result)
        {
            if (result == null)
                return string.Empty;
            
            var builder = new StringBuilder();
            var description = string.Join(";\n", result.Fields?
                .Where(field => field != null)
                .Select((field) =>
                {
                    string outStr = "";
                    if (field.GetType() != null && field.GetType().Name != null)
                    {
                        outStr += field.GetType().Name + " = ";
                    }
                    if (field.Value != null && field.Value.Text != null)
                    {
                        outStr += field.Value.Text;
                    }
                    return outStr;
                })
                .ToList()
            );
            return description;
        }
    }
}
