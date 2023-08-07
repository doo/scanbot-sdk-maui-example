using BarcodeSDK.MAUI.Constants;
using ReadyToUseUI.Maui.Models;

namespace ReadyToUseUI.Maui.SubViews.Cells
{
    public class BarcodeFormatCell : ViewCell
    {
        public KeyValuePair<BarcodeFormat, bool> Source { get; private set; }

        public Label Label { get; set; }

        public Switch Switch { get; set; }

        public BarcodeFormatCell()
        {
            Label = new Microsoft.Maui.Controls.Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                TextColor = Colors.Black
            };

            Switch = new Switch
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End
            };

            View = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(0, 0, 10, 0),
                Children = { Label, Switch }
            };

            Switch.Toggled += delegate
            {
                BarcodeTypes.Instance.Update(Source.Key, Switch.IsToggled);
            };
        }

        protected override void OnBindingContextChanged()
        {
            Source = (KeyValuePair<BarcodeFormat, bool>)BindingContext;
            Label.Text = Source.Key.ToString();
            Switch.IsToggled = Source.Value;

            base.OnBindingContextChanged();
        }
    }
}