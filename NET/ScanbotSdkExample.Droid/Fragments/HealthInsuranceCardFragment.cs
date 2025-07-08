using System.Text;
using Android.OS;
using Android.Views;
using IO.Scanbot.Sdk.Ehicscanner;
using ScanbotSdkExample.Droid.Views;
using R = _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace ScanbotSdkExample.Droid.Fragments
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
            var view = inflater.Inflate(R.Layout.fragment_ehic_dialog, container)!;

            var format = view.FindViewById<TextView>(R.Id.title_text_view)!;
            var content = view.FindViewById<TextView>(R.Id.ehic_info_text_view)!;

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