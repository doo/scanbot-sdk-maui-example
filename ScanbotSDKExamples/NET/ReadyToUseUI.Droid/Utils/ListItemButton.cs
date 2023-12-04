using Android.Content;
using Android.Views;

namespace ReadyToUseUI.Droid.Utils
{
    public class ListItemButton : Android.Widget.Button
    {
        public ListItemButton(Android.Content.Context context, string title, Action doAction) : base(context)
        {
            Text = title;
            DoAction = doAction;

            var parameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );

            var margin = (int)(3 * context.Resources.DisplayMetrics.Density);
            parameters.SetMargins(0, margin, 0, margin);
            LayoutParameters = parameters;
        }

        public Action DoAction { get; private set; }
    }
}

