using Android.Content;
using Android.Views;

namespace ScanbotSdkExample.Droid.Utils
{
    public class ListItemButton : Button
    {
        public ListItemButton(Context context, string title, Action doAction) : base(context)
        {
            Text = title;
            DoAction = doAction;
            this.SetAllCaps(false);

            var parameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent
            );

            var margin = (int)(3 * context.Resources.DisplayMetrics.Density);
            parameters.SetMargins(0, margin, 0, margin);

            Background = null;
            LayoutParameters = parameters;
        }

        public Action DoAction { get; private set; }
    }
}

