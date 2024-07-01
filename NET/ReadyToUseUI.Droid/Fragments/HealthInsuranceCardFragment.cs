using System.Text;
using Android.Views;
using IO.Scanbot.Ehicscanner.Model;
using ReadyToUseUI.Droid.Views;

namespace ReadyToUseUI.Droid.Fragments
{
    public class HealthInsuranceCardFragment : BaseDialogFragment
    {
        public const string NAME = "HealthInsuranceCardFragment";

        public static HealthInsuranceCardFragment CreateInstance(EhicRecognitionResult result)
        {
            var fragment = new HealthInsuranceCardFragment();

            var args = new Bundle();
            var fields = result.Fields.Cast<EhicField>();
            args.PutParcelableArray(NAME, fields.ToArray());

            fragment.Arguments = args;
            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            var data = Arguments.GetParcelableArray(NAME).Cast<EhicField>().ToList();
            var view = inflater.Inflate(Resource.Layout.fragment_barcode_dialog, container);

            var format = view.FindViewById<TextView>(Resource.Id.title);
            var content = view.FindViewById<TextView>(Resource.Id.barcode_result_values);

            if (data.Count == 0)
            {
                content.Text = "No fields found";
                return view;
            }
            format.Text = "Scanned EHIC Card";

            var text = ParseData(data);

            CopyText = text;
            content.Text = CopyText;

            return view;
        }

        public string ParseData(List<EhicField> fields)
        {
            var builder = new StringBuilder();

            foreach (var field in fields)
            {
                builder.Append($"{field.Type.Name()}: {field.Value}\n");
            }
            return builder.ToString();
        }
    }
}
