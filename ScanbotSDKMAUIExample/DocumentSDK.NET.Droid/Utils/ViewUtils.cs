using Android.Content;
using Android.Views;

namespace DocumentSDK.NET.Droid.Utils
{
    public class ViewUtils
    {
        public static ViewGroup.LayoutParams GetParameters(Context context)
        {
            var parameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent
            );

            var margin = (int)(3 * context.Resources.DisplayMetrics.Density);
            parameters.SetMargins(0, margin, 0, margin);

            return parameters;
        }
    }
}
