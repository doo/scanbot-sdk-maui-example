using ScanbotSDK.MAUI;

namespace ReadyToUseUI.Maui.SubViews.ActionBar
{
    public class BottomActionBar : StackLayout
    {
        public const int HEIGHT = 50;

        // Pseudo-universal bottom action bar for multiple pages:
        // These are initialized in Image Results Page
        public BottomActionButton AddButton { get; private set; }
        public BottomActionButton SaveButton { get; private set; }
        public BottomActionButton DeleteAllButton { get; private set; }

        // Whereas these are initialized in Image Details Page
        public BottomActionButton CropButton { get; private set; }
        public BottomActionButton FilterButton { get; private set; }
        public BottomActionButton AnalyzeQualityButton { get; private set; }
        public BottomActionButton DeleteButton { get; private set; }

        public BottomActionBar(bool isDetailPage)
        {
            BackgroundColor = Constants.Colors.ScanbotRed;
            Orientation = StackOrientation.Horizontal;
            HeightRequest = HEIGHT;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.End;

            if (isDetailPage)
            {
                Children.Add(CropButton = new BottomActionButton("CROP")
                {
                    HeightRequest = HEIGHT
                });

                Children.Add(FilterButton = new BottomActionButton("FILTER")
                {
                    HeightRequest = HEIGHT
                });

                Children.Add(AnalyzeQualityButton = new BottomActionButton("ANALYZE QUALITY")
                {
                    HeightRequest = HEIGHT
                });

                Children.Add(DeleteButton = new BottomActionButton("DELETE")
                {
                    HeightRequest = HEIGHT,
                    HorizontalOptions = LayoutOptions.End
                });
            }
            else
            {
                Children.Add(AddButton = new BottomActionButton("ADD")
                {
                    HeightRequest = HEIGHT
                });

                Children.Add(SaveButton = new BottomActionButton("SAVE")
                {
                    HeightRequest = HEIGHT
                });

                Children.Add(DeleteAllButton = new BottomActionButton("DELETE ALL")
                {
                    HeightRequest = HEIGHT,
                    HorizontalOptions = LayoutOptions.End
                });
            }
        }

        public void AddTappedEvent(BottomActionButton button, EventHandler<TappedEventArgs> action)
        {
            var recognizer = new TapGestureRecognizer
            {
            };
            recognizer.Tapped += action;

            button.GestureRecognizers.Add(recognizer);
        }
    }
}