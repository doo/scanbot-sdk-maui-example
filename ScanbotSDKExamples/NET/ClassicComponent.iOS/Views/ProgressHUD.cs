using ClassicComponent.iOS.Utils;
using ObjCRuntime;

namespace ClassicComponent.iOS
{
    public partial class ProgressHUD : UIView
    {
        public ProgressHUD(IntPtr handle) : base(handle)
        {
        }

        public static ProgressHUD Load(CGRect frame)
        {
            var array = NSBundle.MainBundle.LoadNib("ProgressHUD", null, null);
            var view = Runtime.GetNSObject<ProgressHUD>(array.ValueAt(0));
            view.Frame = frame;
            view.BackgroundColor = UIColor.Black.ColorWithAlpha(0.5f);
            Utilities.CreateRoundedCardView(view.loadingIndicator);
            return view;
        }

        public void ToggleLoading(bool isBusy)
        {
            InvokeOnMainThread(() =>
            {
                if (isBusy)
                {
                    var window = (UIApplication.SharedApplication.Delegate as AppDelegate)?.Window;
                    window?.AddSubview(this);
                }
                else
                {
                    this.RemoveFromSuperview();
                }
            });
        }
    }
}