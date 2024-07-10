using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScanbotSDK.MAUI.DocumentFormats.Barcode;

namespace ReadyToUseUI.Maui.Pages.DocumentFilters;

public enum FilterParameterType
{
    SingleTitle,
    List,
    Slider
}

public class FilterItem : List<SubFilter>, INotifyPropertyChanged
{
    // Group Item - Parametric Filter
    private string filterTitle;
    public string FilterTitle 
    {
        get => filterTitle;
        set
        {
            filterTitle = value;
            OnPropertyChanged(nameof(FilterTitle));
        } 
    } 

    // Group Item selection
    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        } 
    } 
    
    // Section Expanded
    private bool isExpanded;
    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        } 
    }

    public FilterItem(string title, List<SubFilter> filters, bool selected = false) : base(filters)
    {
        FilterTitle = title;
        IsSelected = selected;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SubFilter : List<string>, INotifyPropertyChanged
{
    public SubFilter()
    {
    }

    public SubFilter(List<string> list) : base(list)
    {
    }

    // Template/Parameter Type
    public FilterParameterType ParameterType { get; set; }

    // Legacy Filters
    private string subFilterTitle;
    public string SubFilterTitle 
    {
        get => subFilterTitle;
        set
        {
            subFilterTitle = value;
            OnPropertyChanged(nameof(SubFilterTitle));
        } 
    } 

    // Sliders Caption
    private string sliderCaption;
    public string SliderCaption 
    {
        get => sliderCaption;
        set
        {
            sliderCaption = value;
            OnPropertyChanged(nameof(SliderCaption));
        } 
    } 

    // Slider Value
    
    private double sliderValue;
    public double SliderValue 
    {
        get => sliderValue;
        set
        {
            sliderValue = value;
            OnPropertyChanged(nameof(SliderValue));
        } 
    } 

    internal static SubFilter InitSlider(string caption, double sliderValue)
    {
        return new SubFilter
        {
            SliderCaption = caption,
            SliderValue = sliderValue,
            ParameterType = FilterParameterType.Slider
        };
    }

    internal static SubFilter InitLegacyFilter(string subFilter)
    {
        return new SubFilter
        {
            SubFilterTitle = subFilter,
            ParameterType = FilterParameterType.SingleTitle
        };
    }

    internal static SubFilter InitParameters(string parameter, List<string> pickerInput)
    {
        return new SubFilter(pickerInput)
        {
            SubFilterTitle = parameter,
            ParameterType = FilterParameterType.List
        };
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


//
// public class FilterItem : List<SubFilters>
// {
//     public string FilterTitle { get; set; }  // Group Item - Parametric Filter
//     
//     public bool IsSelected { get; set; } // Group Item selection
//
//     // Template/Parameter Type
//     public FilterParameterType ParameterType { get; set; }
//
//     // Inner List Parameters
//     public List<string> FilterParameters { get; set; }
//
//     // Legacy Filters
//     public string SubFilterTitle { get; set; }
//     
//     // Sliders
//     public string SliderCaption { get; set; }
//
//     public double SliderValue { get; set; }
//
//     internal static FilterItem InitSlider(string primeFilter, string caption, double sliderValue)
//     {
//        return new FilterItem
//        {
//            FilterTitle = primeFilter, 
//            SliderCaption = caption, 
//            SliderValue = sliderValue, 
//            ParameterType = FilterParameterType.Slider
//        };
//     }
//
//     internal static FilterItem InitLegacyFilter(string primeFilter, string subFilter)
//     {
//         return new FilterItem
//         {
//             FilterTitle = primeFilter, 
//             SubFilterTitle = subFilter, 
//             ParameterType = FilterParameterType.SingleTitle
//         };
//     }
//
//     internal static FilterItem InitParameters(string primeFilter, string parameter, List<string> pickerInput)
//     {
//         return new FilterItem
//         {
//             FilterTitle = primeFilter, 
//             SubFilterTitle = parameter, 
//             FilterParameters = pickerInput, 
//             ParameterType = FilterParameterType.List
//         };
//     }
//
// }