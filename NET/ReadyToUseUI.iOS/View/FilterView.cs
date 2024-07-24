using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.View
{
    public class FilterView : UIView
    {
        public UIImageView ImageView { get; private set; }

        public UIPickerView Filters { get; private set; }

        public FilterPickerModel Model { get; private set; }

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

        public void SetPickerModel(List<Filter> filters)
        {
            Model = new FilterPickerModel();
            Model.Items = filters;
            Filters.Model = Model;
        }
    }

    public class FilterEventArgs : EventArgs
    {
        public Filter Type { get; set; }
    }

    public class FilterPickerModel : UIPickerViewModel
    {
        private const string iOSPrefix = "SBSDK";
        public EventHandler<FilterEventArgs> SelectionChanged;

        public List<Filter> Items = new List<Filter>();

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
            
            if (currentItem.FilterType == FilterType.NewFilter)
            {
                text = text.Replace(iOSPrefix, string.Empty);
            }
            
            var attributed = new NSAttributedString(text, null, Models.Colors.AppleBlue);
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
                SelectionChanged?.Invoke(this, new FilterEventArgs { Type = filter });
            }
        }
    }
}
