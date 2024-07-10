using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ReadyToUseUI.Maui.Pages.DocumentFilters;
using ReadyToUseUI.Maui.Utils;

namespace ReadyToUseUI.Maui.Pages;

public partial class FiltersPage : ContentPage
{
    private void PopulateData()
    {
        var filterItems = new ObservableCollection<FilterItem>();

        filterItems.Add( new FilterItem("None", new List<SubFilter>()));
        filterItems.Add(new FilterItem("Binarization",
            new List<SubFilter>
                { SubFilter.InitParameters("Output Mode", new List<string>() { "Binary", "AntiAliased" }) }));
        
        filterItems.Add( new FilterItem("CustomBinarization",
            new List<SubFilter>
            {
                SubFilter.InitParameters("Presets", new List<string>() { "Preset1", "Preset2", "Preset3", "Preset4", }),
                SubFilter.InitParameters("Output Mode", new List<string>() { "Binary", "AntiAliased" }),
                SubFilter.InitSlider( "Denoise", 0.4),
                SubFilter.InitSlider( "Radius", 0.5),
            }));
        
        filterItems.Add( new FilterItem("ColorDocument", new List<SubFilter>()));
        filterItems.Add(new FilterItem("Brightness", new List<SubFilter> {SubFilter.InitSlider( "", 0.4) }));
        filterItems.Add(new FilterItem("Contrast", new List<SubFilter> {SubFilter.InitSlider( "", 0.4) }));
        filterItems.Add(new FilterItem("Grayscale", new List<SubFilter>
        {
            SubFilter.InitSlider( "Border Width Fraction", 0.4),
            SubFilter.InitSlider( "Black Outlier Fraction", 0.4),
            SubFilter.InitSlider( "White Outlier Fraction", 0.4)
        }));
        
        filterItems.Add(new FilterItem("Legacy", new List<SubFilter>
        {
            SubFilter.InitLegacyFilter("No Filter"),
            SubFilter.InitLegacyFilter("Color"),
            SubFilter.InitLegacyFilter("Optimised Grayscale"),
            SubFilter.InitLegacyFilter("Binarized"),
            SubFilter.InitLegacyFilter("Color Document"),
            SubFilter.InitLegacyFilter("Pure Binarized"),
            SubFilter.InitLegacyFilter("Background Clean"),
            SubFilter.InitLegacyFilter("Black & White"),
            SubFilter.InitLegacyFilter("Otus Binarization"),
            SubFilter.InitLegacyFilter("Deep Binarization"),
            SubFilter.InitLegacyFilter("Edge Highlights"),
            SubFilter.InitLegacyFilter("Low light binarization"),
            SubFilter.InitLegacyFilter("Low light binarization 2"),
            SubFilter.InitLegacyFilter("Sensitive binarization"),
            SubFilter.InitLegacyFilter("Pure Greyscale"),
        }));
        
        filterItems.Add(new FilterItem("WhiteBlackPoint", new List<SubFilter>
        {
            SubFilter.InitSlider( "Border Width Fraction", 0.4),
            SubFilter.InitSlider( "Black Outlier Fraction", 0.4),
            SubFilter.InitSlider( "White Outlier Fraction", 0.4)
        }));
        

        BaseFilterItems = filterItems.ToList();
       
        FilterItems = new ObservableCollection<FilterItem>(RefreshList());
    }

    // Which item to be shown expanded. 
    public IEnumerable<FilterItem> RefreshList()
    {
        var list = new List<FilterItem>();
        foreach (var item in BaseFilterItems)
        {
            SDKUtils.PrintJson(item);
            if (item.IsExpanded)
            {
                var filter = new FilterItem(item.FilterTitle, item, selected: item.IsSelected)
                {
                    IsExpanded = true
                };
                list.Add(filter);
            }
            else
            {
                list.Add(new FilterItem(item.FilterTitle, new List<SubFilter>(), item.IsSelected));
            }
            
        }

        return list;
    }

    private ObservableCollection<FilterItem> _filterItems;

    public List<FilterItem> BaseFilterItems;
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
        // BindingContext = this;
        PopulateData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FilterssCollectionView.ItemsSource = FilterItems;
    }

    private void Filters_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection is SubFilter selectedItem && selectedItem != null)
        {
           
        }
    }

    private void SubFilters_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }

    private void GroupFilter_OnTapped(object sender, TappedEventArgs e)
    {
        UpdateList(sender as StackLayout,
            (index) =>
            {
                var isExpanded = !BaseFilterItems[index].IsExpanded;
                BaseFilterItems.ForEach(item => item.IsExpanded = false);
                BaseFilterItems[index].IsExpanded = isExpanded;
            });
    }

    private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        UpdateList(sender as CheckBox, (index) => BaseFilterItems[index].IsSelected = !BaseFilterItems[index].IsSelected);
    }

    private void UpdateList(Microsoft.Maui.Controls.View sender, Action<int> action)
    {
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (sender.BindingContext is FilterItem selectedItem)
            {
                
                
                var index = FilterItems.ToList().FindIndex(item => item.FilterTitle == selectedItem?.FilterTitle);
                action?.Invoke(index);
                FilterItems = new ObservableCollection<FilterItem>(RefreshList());
                FilterssCollectionView.ItemsSource = FilterItems;
                Debug.WriteLine("================MASTER LIST======================");
                SDKUtils.PrintJson(BaseFilterItems);
                Debug.WriteLine("===============FILTERED LIST=====================");
                SDKUtils.PrintJson(FilterItems);
            }
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}