using Android.Views;
using Google.Android.Material.BottomSheet;
using IO.Scanbot.Imagefilters;
using IO.Scanbot.Sdk.Process;
using ReadyToUseUI.Droid.Model;

namespace ReadyToUseUI.Droid.Fragments;

public class FilterListFragment : BottomSheetDialogFragment
{
    private readonly List<FilterItem> filters = new List<FilterItem>
    {
        new FilterItem("New Filters", true),
        new FilterItem(typeof(ScanbotBinarizationFilter)),
        new FilterItem(typeof(CustomBinarizationFilter)),
        new FilterItem(typeof(ColorDocumentFilter)),
        new FilterItem(typeof(BrightnessFilter)),
        new FilterItem(typeof(ContrastFilter)),
        new FilterItem(typeof(GrayscaleFilter)),
        new FilterItem(typeof(WhiteBlackPointFilter)),
            
        new FilterItem("Legacy Filters", true),
        new FilterItem(ImageFilterType.None),
        new FilterItem(ImageFilterType.LowLightBinarization),
        new FilterItem(ImageFilterType.LowLightBinarization2),
        new FilterItem(ImageFilterType.EdgeHighlight),
        new FilterItem(ImageFilterType.DeepBinarization),
        new FilterItem(ImageFilterType.OtsuBinarization),
        new FilterItem(ImageFilterType.BackgroundClean),
        new FilterItem(ImageFilterType.ColorDocument),
        new FilterItem(ImageFilterType.ColorEnhanced),
        new FilterItem(ImageFilterType.Grayscale),
        new FilterItem(ImageFilterType.PureGrayscale),
        new FilterItem(ImageFilterType.Binarized),
        new FilterItem(ImageFilterType.PureBinarized),
        new FilterItem(ImageFilterType.BlackAndWhite),
    };
    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = inflater.Inflate(Resource.Layout.filter_list_layout, container, false);
        var listView = view.FindViewById<ListView>(Resource.Id.listView);

        var adapter = new FilterItemAdapter(filters);
        listView.Adapter = adapter;
        listView.ItemClick += FilterItemSelected; 
        
        return view;
    }

    private void FilterItemSelected(object sender, AdapterView.ItemClickEventArgs e)
    {
        if (filters.Count > e.Position && !filters[e.Position].IsSection)
        {
            var selectedItem = filters[e.Position].IsSection;
        }
    }

    private ParametricFilter GetParametricFilter(FilterItem filter)
    {
        if (filter.FilterType == FilterType.LegacyFilter)
        {
            ImageFilterType legacyFilter;
            // todo add converter for the ImageFilterType
            // Enum.TryParse(filter.Title, true, out legacyFilter);
            return new LegacyFilter((int)legacyFilter);
        }

        switch (filter.Title)
        {
            case nameof(ScanbotBinarizationFilter):
                return new ScanbotBinarizationFilter();
                
            case nameof(CustomBinarizationFilter):
                return new CustomBinarizationFilter();
                
            case nameof(ColorDocumentFilter):
                return new ColorDocumentFilter();
                
            case nameof(BrightnessFilter):
                return new BrightnessFilter();
                
            case nameof(ContrastFilter):
                return new ContrastFilter();
                
            case nameof(GrayscaleFilter):
                return new GrayscaleFilter();
                
            case nameof(WhiteBlackPointFilter):
                return new WhiteBlackPointFilter();
        }
            
        return new LegacyFilter((int)ImageFilterType.None);
    }
}

public class FilterItemAdapter : BaseAdapter<FilterItem>
{
    private List<FilterItem> filters;

    public FilterItemAdapter(List<FilterItem> filters)
    {
        this.filters = filters;
    }

    public override long GetItemId(int position) => position;

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        
    }

    public override int Count => this.filters?.Count ?? 0;

    public override FilterItem this[int position] => this.filters[position];
} 