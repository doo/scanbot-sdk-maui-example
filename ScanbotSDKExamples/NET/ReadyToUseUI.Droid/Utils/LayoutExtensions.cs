using System;
using Android.Widget;
using System.Linq;

namespace ReadyToUseUI.Droid.Utils
{
    public static class LayoutExtensions
    {
        public static LinearLayout AddChildren(this LinearLayout container, params ListItemButton[] items)
        {
            return items.Aggregate(container, (container, item) =>
            {
                container.AddView(item);

                return container;
            });
        }

        public static LinearLayout AddChildren(this LinearLayout container, List<ListItemButton> allButtons, params ListItemButton[] items)
        {
            allButtons.AddRange(items);
            return AddChildren(container, items);
        }
    }
}

