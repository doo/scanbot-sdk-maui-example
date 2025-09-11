namespace ScanbotSdkExample.Droid.Utils
{
    public static class LayoutExtensions
    {
        public static LinearLayout AddChildren(this LinearLayout container, List<ListItemButton> allButtons, params ListItemButton[] items)
        {
            allButtons.AddRange(items);

            return items.Aggregate(container, (container, item) =>
            {
                container.AddView(item);

                return container;
            });
        }
    }
}

