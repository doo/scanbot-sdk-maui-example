using Microsoft.Maui.Controls.Internals;

namespace ScanbotSdkExample.Maui.Pages;

public class FilterItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate PrimaryFilterTemplate { get; set; }
    
    public DataTemplate SubFilterTemplate { get; set; }
    
    public DataTemplate PickerItemTemplate { get; set; }

    public DataTemplate SliderItemTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var listItem = item as FilterItem;
        switch (listItem.ParameterType)
        {
            case FilterParameterType.PrimaryFilter:
                return PrimaryFilterTemplate;
            case FilterParameterType.SubFilter:
                return SubFilterTemplate;
            case FilterParameterType.Slider:
                return SliderItemTemplate;
            case FilterParameterType.Picker:
                return PickerItemTemplate;
            
            default: return PrimaryFilterTemplate;
        }
    }
}