using BarcodeSDK.MAUI.Constants;

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
        public BottomActionButton DeleteButton { get; private set; }

        public BottomActionBar(bool isDetailPage)
        {
            BackgroundColor = SBColors.ScanbotRed;
            Orientation = StackOrientation.Horizontal;
            HeightRequest = HEIGHT;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.End;

            if (isDetailPage)
            {
                CropButton = new BottomActionButton("crop.png", "CROP");
                CreateButton(CropButton);
                FilterButton = new BottomActionButton("filter.png", "FILTER");
                CreateButton(FilterButton);
                DeleteButton = new BottomActionButton("delete.png", "DELETE");
                CreateButton(DeleteButton, true);
            }
            else
            {
                AddButton = new BottomActionButton("add.png", "ADD");
                CreateButton(AddButton);
                SaveButton = new BottomActionButton("save.png", "SAVE");
                CreateButton(SaveButton);
                DeleteAllButton = new BottomActionButton("delete.png", "DELETE ALL");
                CreateButton(DeleteAllButton, true);
            }
        }

        void CreateButton(BottomActionButton button, bool alignRight = false)
        {
            button.HeightRequest = HEIGHT;
            if (alignRight)
            {
                button.HorizontalOptions = LayoutOptions.End;
            }

            Children.Add(button);
        }

        public void AddClickEvent(BottomActionButton button, EventHandler<TappedEventArgs> action)
        {
            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += action;

            button.GestureRecognizers.Add(recognizer);
        }
    }
}