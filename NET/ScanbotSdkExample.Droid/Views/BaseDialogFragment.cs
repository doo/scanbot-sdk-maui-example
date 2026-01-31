using Android.Views;
using DialogFragment = Android.App.DialogFragment;
using R = _Microsoft.Android.Resource.Designer.ResourceConstant;

namespace ScanbotSdkExample.Droid.Views
{
    public class BaseDialogFragment : DialogFragment
    {
        public string CopyText { get; set; }

        public virtual View AddContentView(LayoutInflater inflater, ViewGroup container)
        {
            throw new Exception("AddContentView should be overridden");
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
            var inflater = LayoutInflater.From(Activity);
            var container = (ViewGroup)inflater.Inflate(R.Layout.holo_dialog_frame, null, false);
            AddContentView(inflater, container);

            builder.SetView(container);

            builder.SetPositiveButton("Close", (_, _) =>
            {
                Dismiss();
            });

            var dialog = builder.Create();
            dialog.SetCanceledOnTouchOutside(true);

            return dialog;
        }

    }
}
