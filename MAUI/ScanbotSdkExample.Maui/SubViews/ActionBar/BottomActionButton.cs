namespace ScanbotSdkExample.Maui.SubViews.ActionBar;
public class BottomActionButton : StackLayout
{
    public Label Label { get; private set; }

    public BottomActionButton(string text)
    {
        Orientation = StackOrientation.Horizontal;

        Label = new Label
        {
            Text = text,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(horizontalSize: 12, verticalSize: 3),
            FontSize = 14
        };
        Children.Add(Label);
    }
}