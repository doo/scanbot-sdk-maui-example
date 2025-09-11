using R = _Microsoft.Android.Resource.Designer.ResourceConstant;
using Android.Views;
using ScanbotSdkExample.Droid.Utils;
using GenericDocument = IO.Scanbot.Sdk.Genericdocument.Entity.GenericDocument;
using ScanbotSdkExample.Droid.Views;

namespace ScanbotSdkExample.Droid.Fragments
{
    internal class MRZDialogFragment : BaseDialogFragment
    {
        private const string MrzData = "MRZ_DATA";
        public const string Name = "MRZDialogFragment";

        private GenericDocument _result;

        public static MRZDialogFragment CreateInstance(GenericDocument mrzDocument)
        {
            var fragment = new MRZDialogFragment();
            var args = new Bundle();
            args.PutParcelable(MrzData, mrzDocument);
            fragment.Arguments = args;

            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            _result = (GenericDocument)Arguments.GetParcelable(MrzData);
            var view = inflater.Inflate(R.Layout.fragment_mrz_dialog, container)!;

            CopyText = _result.ToFormattedString();
            ((TextView)view.FindViewById(R.Id.tv_data)).Text = CopyText;
            return view;
        }
    }
}
