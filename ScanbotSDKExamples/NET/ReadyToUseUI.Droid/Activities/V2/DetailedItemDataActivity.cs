using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration;
using Java.Lang;

namespace ReadyToUseUI.Droid.Activities.V2
{
    [Activity(Theme = "@style/AppTheme")]
    public class DetailedItemDataActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.detailed_item_data);
            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var item = Intent.GetParcelableExtra("SelectedBarcodeItem") as BarcodeItem;

            if (item == null)
            {
                return;
            }

            var container = FindViewById<ConstraintLayout>(Resource.Id.container);

            container.FindViewById<TextView>(Resource.Id.barcodeFormat).Text = item.Type?.Name();
                container.FindViewById<TextView>(Resource.Id.docFormat).Text = ParseData(item.ParsedDocument);      
            container.FindViewById<TextView>(Resource.Id.description).Text = item.Text;
        }
        
        private string ParseData(IO.Scanbot.Genericdocument.Entity.GenericDocument result)
        {
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
