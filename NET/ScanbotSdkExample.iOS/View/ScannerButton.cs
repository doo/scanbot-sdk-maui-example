using ScanbotSdkExample.iOS.Models;

namespace ScanbotSdkExample.iOS.View
{
    public sealed class ScannerButton : UIView
    {
        public event EventHandler<EventArgs> Click;
        public ListItem Data { get; private set; }

        private readonly UILabel _title;

        public ScannerButton(ListItem data)
        {
            Data = data;

            _title = new UILabel();
            _title.Text = data.Title;
            _title.Font = UIFont.FromName("HelveticaNeue-Bold", 15f);
            _title.TextColor = Colors.NearWhite;
            
            AddSubview(_title);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 0;
            _title.Frame = new CGRect(padding, 0, (float)Frame.Width - 2 * padding, Frame.Height);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            Layer.Opacity = 0.5f;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            Layer.Opacity = 1.0f;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            Layer.Opacity = 1.0f;
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
