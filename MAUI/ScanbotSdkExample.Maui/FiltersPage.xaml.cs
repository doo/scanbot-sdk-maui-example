using System.Collections.ObjectModel;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.ImageProcessing;
using ScanbotSdkExample.Maui.Models;

namespace ScanbotSdkExample.Maui;

public partial class FiltersPage
{
    private Action<ParametricFilter[]> _filterSelectionChanged;
    internal void NavigateData(Action<ParametricFilter[]> filterSelectionChanged)
    {
        _filterSelectionChanged = filterSelectionChanged;
    }
    private void PopulateData()
    {
        FilterItems =
        [
            FilterItem.InitPrimaryFilter(FilterItemConstants.None),

            FilterItem.InitPrimaryFilter(nameof(ScanbotBinarizationFilter)),
            FilterItem.InitPicker(nameof(ScanbotBinarizationFilter), FilterItemConstants.OutputMode, Enum.GetNames<OutputMode>().ToList()),
            
            FilterItem.InitPrimaryFilter(nameof(CustomBinarizationFilter)),
            FilterItem.InitPicker(nameof(CustomBinarizationFilter), FilterItemConstants.Presets, Enum.GetNames<BinarizationFilterPreset>().ToList()),
            FilterItem.InitPicker(nameof(CustomBinarizationFilter), FilterItemConstants.OutputMode, Enum.GetNames<OutputMode>().ToList()),
            FilterItem.InitSlider(nameof(CustomBinarizationFilter), FilterItemConstants.Denoise, 0.5, 0.0, 1.0),
            FilterItem.InitSlider(nameof(CustomBinarizationFilter), FilterItemConstants.Radius, 32, 0, 512),

            FilterItem.InitPrimaryFilter(nameof(ColorDocumentFilter)),

            FilterItem.InitPrimaryFilter(nameof(BrightnessFilter)),
            FilterItem.InitSlider(nameof(BrightnessFilter), FilterItemConstants.Brightness, 0.0, -1.0, 1.0),

            FilterItem.InitPrimaryFilter(nameof(ContrastFilter)),
            FilterItem.InitSlider(nameof(ContrastFilter), FilterItemConstants.Contrast, 0.0, -1, 254),

            FilterItem.InitPrimaryFilter(nameof(GrayscaleFilter)),
            FilterItem.InitSlider(nameof(GrayscaleFilter), FilterItemConstants.BorderWidthFraction, 0.06, 0.0, 0.15),
            FilterItem.InitSlider(nameof(GrayscaleFilter), FilterItemConstants.BlackOutlierFraction, 0.0, 0.0, 0.05),
            FilterItem.InitSlider(nameof(GrayscaleFilter), FilterItemConstants.WhiteOutlierFraction, 0.02, 0.0, 0.05),

            FilterItem.InitPrimaryFilter(nameof(WhiteBlackPointFilter)),
            FilterItem.InitSlider(nameof(WhiteBlackPointFilter), FilterItemConstants.BlackPoint, 0.0, 0.0, 1.0),
            FilterItem.InitSlider(nameof(WhiteBlackPointFilter), FilterItemConstants.WhitePoint, 0.0, 0.0, 1.0),
        ];
    }

    private ObservableCollection<FilterItem> _filterItems;
    public ObservableCollection<FilterItem> FilterItems
    {
        get => _filterItems;
        set
        {
            _filterItems = value;
            OnPropertyChanged(nameof(FilterItems));
        }
    }

    public FiltersPage()
    {
        InitializeComponent();
        PopulateData();
        Title = "Document Filters";

        ToolbarItems.Add(new ToolbarItem("Done", null, DoneButtonClicked, ToolbarItemOrder.Primary));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FilterssCollectionView.ItemsSource = FilterItems;
    }
    
    private bool _isBusy;

    private async void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        // Case: Selection 'None' Filter
        // Unselecting all items selected filters(CheckBoxes) triggers the CheckBoxValueChanged event every single time. So we skip the call until all checkboxes are updated.
        if (_isBusy)
        {
            return;
        }

        _isBusy = true;
        var checkBox = sender as CheckBox;
        var filterItem = checkBox?.BindingContext as FilterItem;

        if (checkBox == null || filterItem == null)
        {
            _isBusy = false;
            return;
        }

        // Clear filters
        if (filterItem.FilterTitle == FilterItemConstants.None && checkBox.IsChecked)
        {
            var result = await DisplayAlert("Alert",
                "Selecting None will clear all your previous selections, Please confirm.", "Continue",
                "Cancel");

            if (result)
            {
                foreach (var item in FilterItems)
                {
                    item.IsSelected = false;
                }

                // Checking the None filter.
                FilterItems.First().IsSelected = true;
            }
            else
            {
                checkBox.IsChecked = false;
            }
        }
        else
        {
            // Unchecking the None filter.
            FilterItems.First().IsSelected = false;
        }

        _isBusy = false;
    }

    private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        if (picker == null) return;

        UpdateList(picker, index =>
        {
            if (index >= 0 && index < FilterItems.Count)
            {
                FilterItems[index].PickerSelectedValue = picker.SelectedItem as string;
            }
        });
    }

    private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        var slider = sender as Slider;
        if (slider == null)
            return;

        UpdateList(slider, index =>
        {
            if (index >= 0 && index < FilterItems.Count)
            {
                FilterItems[index].SliderValue = slider.Value;
            }
        });
    }

    private void UpdateList(View sender, Action<int> action)
    {
        if (sender?.BindingContext == null) return;
        
        if (sender.BindingContext is FilterItem selectedItem)
        {
            var index = FilterItems.ToList().FindIndex(item => item.FilterTitle == selectedItem.FilterTitle);
            action?.Invoke(index);
        }
    }

    /// <summary>
    /// Extract the selected filters from the FilterItems and init the ParametricFiltersList
    /// </summary>
    private async void DoneButtonClicked()
    {
        var selectedFilters = FilterItems?.Where(item => item.IsSelected).ToList() ?? new List<FilterItem>(); // gets only primary filters with CheckBox.
        
        if (ValidateEmptyValues(selectedFilters))
        {
            // if filter is selected but the picker values are left empty.
            await DisplayAlert("Alert", "Please insert the required values for the selected filters and try again.", "Ok");
            return;
        }

        // Clear all the filters of null
        if (selectedFilters.Count == 1 && selectedFilters.First().FilterTitle == FilterItemConstants.None)
        {
            // clear all filters
            DidFinishFilterSelection([]);
            return;
        }

        var list = new List<ParametricFilter>();
        foreach (var filterType in selectedFilters)
        {
            if (filterType.FilterTitle == nameof(ColorDocumentFilter))
            {
                list.Add(GetParametricFilterFromItems(filterType.FilterTitle, null));
                continue;
            }
            
            // get the Filter parameters only
            var properties = FilterItems?.Where(item => item.FilterTitle == filterType.FilterTitle && !string.IsNullOrEmpty(item.Caption)).ToList() ?? new List<FilterItem>();
            list.Add(GetParametricFilterFromItems(filterType.FilterTitle, properties));
        }

        DidFinishFilterSelection(list.ToArray());
    }

    private void DidFinishFilterSelection(ParametricFilter[] list)
    {
        _filterSelectionChanged?.Invoke(list);
        this.Navigation.PopAsync(true);
    }

    private bool ValidateEmptyValues(List<FilterItem> selectedFilters)
    {
        bool isEmpty = false;
        foreach (var filter in selectedFilters)
        {
            var list = FilterItems?.Where(item => item.FilterTitle == filter.FilterTitle && item.ParameterType == FilterParameterType.Picker).ToList() ?? new List<FilterItem>();
            if (list.Any(item => string.IsNullOrEmpty(item.PickerSelectedValue)))
            {
                isEmpty = true;
                break;
            }
        }
        return isEmpty;
    }

    private ParametricFilter GetParametricFilterFromItems(string filterType, List<FilterItem> properties)
    {
        switch (filterType)
        {
            case nameof(ScanbotBinarizationFilter):
                return ParametricFilter.ScanbotBinarization(GetOutputMode(properties.First().PickerSelectedValue));

            case nameof(CustomBinarizationFilter):
            {
                var outputMode = properties.First(item => item.Caption == FilterItemConstants.OutputMode)
                    .PickerSelectedValue;
                var presets = properties.First(item => item.Caption == FilterItemConstants.Presets)
                    .PickerSelectedValue;
                var deniose = properties.First(item => item.Caption == FilterItemConstants.Denoise)
                    .SliderValue;
                var radius = properties.First(item => item.Caption == FilterItemConstants.Radius)
                    .SliderValue;
                return ParametricFilter.CustomBinarization(GetOutputMode(outputMode), deniose, (int)radius,
                    GetBinarizationPreset(presets));
            }
            case nameof(ColorDocumentFilter):
                return ParametricFilter.ColorDocument;

            case nameof(BrightnessFilter):
                return ParametricFilter.Brightness(properties.First().SliderValue);

            case nameof(ContrastFilter):
                return ParametricFilter.Contrast(properties.First().SliderValue);

            case nameof(GrayscaleFilter):
            {
                var borderWidth = properties.First(item => item.Caption == FilterItemConstants.BorderWidthFraction)
                    .SliderValue;
                var blackOutlier = properties.First(item => item.Caption == FilterItemConstants.BlackOutlierFraction)
                    .SliderValue;
                var whiteOutlier = properties.First(item => item.Caption == FilterItemConstants.WhiteOutlierFraction)
                    .SliderValue;

                return ParametricFilter.Grayscale(borderWidth, blackOutlier, whiteOutlier);
            }
            case nameof(WhiteBlackPointFilter):
            {
                var blackPoint = properties.First(item => item.Caption == FilterItemConstants.BlackPoint)
                    .SliderValue;
                var whitePoint = properties.First(item => item.Caption == FilterItemConstants.WhitePoint)
                    .SliderValue;
                
                return ParametricFilter.WhiteBlackPoint(blackPoint, whitePoint);
            }
        }

        throw new Exception("Filter type unsupported.");
    }

    private OutputMode GetOutputMode(string pickerValue)
    {
        Enum.TryParse(pickerValue, out OutputMode myStatus);
        return myStatus;
    }

    private BinarizationFilterPreset GetBinarizationPreset(string pickerValue)
    {
        Enum.TryParse(pickerValue, out BinarizationFilterPreset preset);
        return preset;
    }
}

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