using System.Text;
using Android.OS;
using Android.Views;
using IO.Scanbot.Sdk.Ehicscanner;
using ReadyToUseUI.Droid.Views;

namespace ReadyToUseUI.Droid.Fragments
{
    public class HealthInsuranceCardFragment : BaseDialogFragment
    {
        public const string Name = "HealthInsuranceCardFragment";

        public static HealthInsuranceCardFragment CreateInstance(EuropeanHealthInsuranceCardRecognitionResult result)
        {
            var fragment = new HealthInsuranceCardFragment();

            var args = new Bundle();
            args.PutParcelableArray(Name, result.Fields.ToArray<IParcelable>());
            fragment.Arguments = args;

            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            var list = Arguments.GetParcelableArray(Name).Cast<EuropeanHealthInsuranceCardRecognitionResult.Field>().ToList();
            var view = inflater.Inflate(Resource.Layout.fragment_barcode_dialog, container)!;

            var format = view.FindViewById<TextView>(Resource.Id.title)!;
            var content = view.FindViewById<TextView>(Resource.Id.barcode_result_values)!;

            if (list.Count == 0)
            {
                content.Text = "No fields found";
                return view;
            }

            format.Text = "Scanned EHIC Card";

            var text = ParseData(list);

            CopyText = text;
            content.Text = CopyText;

            return view;
        }

        private string ParseData(List<EuropeanHealthInsuranceCardRecognitionResult.Field> fields)
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