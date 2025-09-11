using ScanbotSdkExample.iOS.Models;

namespace ScanbotSdkExample.iOS.View
{
    public sealed class FilterView : UIView
    {
        public UIImageView ImageView { get; set; }

        private UIPickerView Filters { get; set; }

        private FilterPickerModel Model { get; set; }

        public FilterView()
        {
            BackgroundColor = UIColor.White;

            ImageView = new UIImageView();
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageView.Layer.BorderColor = Colors.LightGray.CGColor;
            ImageView.Layer.BorderWidth = 1;
            ImageView.BackgroundColor = Colors.NearWhite;
            AddSubview(ImageView);

            Filters = new UIPickerView();
            AddSubview(Filters);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 5;
            float pickerHeight = (float)Frame.Width / 3 * 2;

            float x = padding;
            float y = padding;

            float w = (float)Frame.Width - 2 * padding;
            float h = (float)Frame.Height - (pickerHeight + 2 * padding);

            ImageView.Frame = new CGRect(x, y, w, h);

            x = 0;
            y += h + padding;
            w = (float)Frame.Width;
            h = pickerHeight;

            Filters.Frame = new CGRect(x, y, w, h);
        }

        public void SetPickerModel(List<FilterItem> filters)
        {
            Model = new FilterPickerModel();
            Model.Items = filters;
            Filters.Model = Model;
        }
    }

    public class FilterEventArgs : EventArgs
    {
        public FilterItem Type { get; set; }
    }

    public class FilterPickerModel : UIPickerViewModel
    {
        private const string ScanbotSdkPrefix = "SBSDK";

        public List<FilterItem> Items = new List<FilterItem>();

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return Items.Count;
        }

        [Export("pickerView:attributedTitleForRow:forComponent:")]
        public override NSAttributedString GetAttributedTitle(UIPickerView pickerView, nint row, nint component)
        {
            var currentItem = Items[(int)row];
            var text = currentItem.Title;
            
            if (currentItem.IsSection)
            {
                return new NSAttributedString(text, UIFont.BoldSystemFontOfSize(16), foregroundColor:Colors.ScanbotRed);
            }
            
            if (text.Contains(ScanbotSdkPrefix))
            {
                text = text.Replace(ScanbotSdkPrefix, string.Empty);
            }
            
            var attributed = new NSAttributedString(text, null, Colors.AppleBlue);
            return attributed;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            var filter = Items[(int)row];
            if (!filter.IsSection)
            {
                filter.FilterSelected?.Invoke();
            }
        }
    }
}
