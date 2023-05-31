using System;
using Android.Content;
using Android.Widget;
using DocumentSDK.NET.Droid.Model;
using DocumentSDK.NET.Model;

namespace DocumentSDK.NET.Droid.Views
{
    public class FragmentButton : Button
    {
        public ListItem Data { get; set; }

        public FragmentButton(Context context) : base(context)
        {
        }
    }
}
