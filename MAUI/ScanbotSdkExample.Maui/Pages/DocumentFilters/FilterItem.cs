using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScanbotSdkExample.Maui.Pages;

public class FilterItemConstants
{
    internal const string None = "None";
    internal const string Presets = "Presets";
    internal const string OutputMode = "Output Mode";
    internal const string Denoise = "Denoise";
    internal const string Radius = "Radius";
    internal const string Brightness = "Brightness";
    internal const string Contrast = "Contrast";
    internal const string BorderWidthFraction = "Border Width Fraction";
    internal const string BlackOutlierFraction = "Black Outlier Fraction";
    internal const string WhiteOutlierFraction = "White Outlier Fraction";
    internal const string LegacyFilters = "Legacy Filters";
    internal const string BlackPoint = "Black Point";
    internal const string WhitePoint = "White Point";
}

public enum FilterParameterType
{
    PrimaryFilter,
    SubFilter,
    Slider,
    Picker
}

public class FilterItem : INotifyPropertyChanged
{
    private string _filterTitle;
    private bool _isSelected;
    private bool _isExpanded;
    private string _caption;
    private double _sliderValue;
    private List<string> _pickerItems;
    private string _pickerSelectedValue;
    private double _minValue;
    private double _maxValue;
    
    public FilterParameterType ParameterType { get; set; }
    
    // Group Item - Parametric Filter
    public string FilterTitle
    {
        get => _filterTitle;
        set
        {
            _filterTitle = value;
            OnPropertyChanged(nameof(FilterTitle));
        } 
    } 

    // Group Item selection
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        } 
    } 
    
    // Section Expanded
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            _isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        } 
    }
    
    // Sliders Caption
    public string Caption 
    {
        get => _caption;
        set
        {
            _caption = value;
            OnPropertyChanged(nameof(Caption));
        } 
    } 

    // Slider Value
    public double SliderValue 
    {
        get => _sliderValue;
        set
        {
            _sliderValue = value;
            OnPropertyChanged(nameof(SliderValue));
        } 
    }
    
    // Picker Values
    public List<string> PickerItems 
    {
        get => _pickerItems;
        set
        {
            _pickerItems = value;
            OnPropertyChanged(nameof(PickerItems));
        } 
    }

    
    /// <summary>
    /// Selected Picker Value
    /// </summary>
    public string PickerSelectedValue {
    get => _pickerSelectedValue;
    set
    {
        _pickerSelectedValue = value;
        OnPropertyChanged(nameof(PickerSelectedValue));
    } 
} 


    internal static FilterItem InitPrimaryFilter(string title)
    {
        return new FilterItem
        {
            FilterTitle = title,
            ParameterType = FilterParameterType.PrimaryFilter
        };
    }
    
    internal static FilterItem InitSubFilter(string primaryFilter, string subFilter)
    {
        return new FilterItem
        {
            FilterTitle = primaryFilter,
            Caption = subFilter,
            ParameterType = FilterParameterType.SubFilter
        };
    }

    internal static FilterItem InitSlider(string primaryFilter, string caption, double sliderValue, double minValue, double maxValue)
    {
        return new FilterItem
        {
            FilterTitle = primaryFilter,
            Caption = caption,
            SliderValue = sliderValue,
            ParameterType = FilterParameterType.Slider,
            MinValue = minValue,
            MaxValue = maxValue
        };
    }

    public double MaxValue
    {
        get => _maxValue;
        set
        {
            if (value.Equals(_maxValue)) return;
            _maxValue = value;
            OnPropertyChanged();
        }
    }

    public double MinValue
    {
        get => _minValue;
        set
        {
            if (value.Equals(_minValue)) return;
            _minValue = value;
            OnPropertyChanged();
        }
    }

    internal static FilterItem InitPicker(string primaryFilter, string caption, List<string> pickerValues)
    {
        return new FilterItem
        {
            FilterTitle = primaryFilter,
            Caption = caption,
            ParameterType = FilterParameterType.Picker,
            PickerItems = pickerValues
        };
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}