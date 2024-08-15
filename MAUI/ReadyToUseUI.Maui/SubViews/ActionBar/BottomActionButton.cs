namespace ReadyToUseUI.Maui.SubViews.ActionBar
{
    public class BottomActionButton : StackLayout
    {
        public Label Label { get; private set; }

        public BottomActionButton(string text)
        {
            Orientation = StackOrientation.Horizontal;

            Label = new Label();
            Label.Text = text;
            Label.TextColor = Colors.White;
            Label.VerticalOptions = LayoutOptions.Center;
            Label.Margin = new Thickness(horizontalSize: 12, verticalSize: 3);
            Label.FontSize = 14;
            Children.Add(Label);
        }
    }
}

