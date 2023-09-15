using Android.Content;
using DocumentSDK.NET.Model;

namespace ReadyToUseUI.Droid.Views
{
    public class FragmentButton : Button
    {
        public ListItem Data { get; set; }

        public FragmentButton(Context context) : base(context)
        {
        }
    }
}
