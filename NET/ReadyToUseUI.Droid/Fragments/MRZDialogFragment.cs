using Android.App.AppSearch;
using Android.Views;
using ReadyToUseUI.Droid.Utils;
using GenericDocument = IO.Scanbot.Sdk.Genericdocument.Entity.GenericDocument;
using ReadyToUseUI.Droid.Views;

namespace ReadyToUseUI.Droid.Fragments
{
    internal class MRZDialogFragment : BaseDialogFragment
    {
        public const string MRZ_DATA = "MRZ_DATA";
        public const string NAME = "MRZDialogFragment";

        private GenericDocument result;

        public static MRZDialogFragment CreateInstance(GenericDocument mrzDocument)
        {
            var fragment = new MRZDialogFragment();
            var args = new Bundle();
            args.PutParcelable(MRZ_DATA, mrzDocument);
            fragment.Arguments = args;

            return fragment;
        }

        public override View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            result = (GenericDocument)Arguments.GetParcelable(MRZ_DATA);
            var view = inflater.Inflate(Resource.Layout.fragment_mrz_dialog, container);

            CopyText = result.ToFormattedString();
            view.FindViewById<TextView>(Resource.Id.tv_data).Text = CopyText;
            return view;
        }
    }
}
