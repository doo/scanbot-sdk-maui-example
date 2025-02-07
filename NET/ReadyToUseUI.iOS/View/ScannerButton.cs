﻿using ReadyToUseUI.iOS.Models;

namespace ReadyToUseUI.iOS.View
{
    public class ScannerButton : UIView
    {
        public event EventHandler<EventArgs> Click;
        public ListItem Data { get; private set; }

        private UILabel title;

        public ScannerButton(ListItem data)
        {
            this.Data = data;

            title = new UILabel();
            title.Text = data.Title;
            title.Font = UIFont.FromName("HelveticaNeue-Bold", 15f);
            title.TextColor = Models.Colors.NearWhite;
            
            AddSubview(title);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 0;
            title.Frame = new CGRect(padding, 0, (float)Frame.Width - 2 * padding, Frame.Height);
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
