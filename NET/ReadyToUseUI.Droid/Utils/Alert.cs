using Android.Content;

namespace ReadyToUseUI.Droid.Utils
{
    public class Alert
    {
        public static void ShowLicenseDialog(Context context)
        {
            var text =
                "The demo app will terminate because of the missing license key. " +
                "Get your free 30-day license today!";
            Toast(context, text);
        }

        public static void Toast(Context context, string text)
        {
            Android.Widget.Toast.MakeText(context, text, Android.Widget.ToastLength.Long).Show();
        }

        public static void ShowAlert(Context context, string title, string message)
        {
            var dialog = new AndroidX.AppCompat.App.AlertDialog.Builder(context);
            AndroidX.AppCompat.App.AlertDialog alert = dialog.Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton((int)DialogButtonType.Neutral, "OK", (c, ev) =>
            {
                alert.Dismiss();
            });
            alert.Show();
        }
    }
}
