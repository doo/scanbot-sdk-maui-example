using Microsoft.Maui.Controls.Internals;

namespace ReadyToUseUI.Maui.Pages.DocumentFilters;

public class FilterItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate CollectionViewItemTemplate { get; set; }
    public DataTemplate ListItemTemplate { get; set; }
    public DataTemplate SliderItemTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
     
        var listItem = item as SubFilter;
        switch (listItem.ParameterType)
        {
            case FilterParameterType.SingleTitle:
                return ListItemTemplate;
            case FilterParameterType.Slider:
                return SliderItemTemplate;
            case FilterParameterType.List:
                return CollectionViewItemTemplate;
            
            default: return ListItemTemplate;
        }
    }
}