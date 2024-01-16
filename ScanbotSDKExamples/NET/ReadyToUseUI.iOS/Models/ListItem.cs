namespace ReadyToUseUI.iOS.Models
{
    public struct ListItem
    {
        public ListItem(string title, Action listAction)
        {
            Title = title;
            DoAction = listAction;
        }

        public string Title { get; private set; }

        public Action DoAction { get; private set; }
    }
}
