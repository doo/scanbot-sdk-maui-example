using Android.Content;
using Android.Graphics;
using Android.Views;
using Google.Android.Material.BottomSheet;
using IO.Scanbot.Imagefilters;
using IO.Scanbot.Sdk.Process;
using ReadyToUseUI.Droid.Listeners;
using ReadyToUseUI.Droid.Model;

namespace ReadyToUseUI.Droid.Fragments;

public interface IFilterItemAdapter
{
    public Context AdapterContext { get; }

    public List<FilterItem> Filters { get; }
}

public class FilterListFragment : BottomSheetDialogFragment, IFilterItemAdapter
{
    public Context AdapterContext => this.Context;
    public List<FilterItem> Filters => this.filters;

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

        var adapter = new FilterItemAdapter(this);
        listView.Adapter = adapter;
        listView.ItemClick += FilterItemSelected;

        return view;
    }

    private void FilterItemSelected(object sender, AdapterView.ItemClickEventArgs e)
    {
        if (filters.Count > e.Position && !filters[e.Position].IsSection)
        {
            var selectedItem = GetParametricFilter(filters[e.Position]);
            (Activity as IFiltersListener).ApplyFilter(selectedItem);
            DismissAllowingStateLoss();
        }
    }

    private ParametricFilter GetParametricFilter(FilterItem filter)
    {
        if (filter.FilterType == FilterType.LegacyFilter)
        {
            return new LegacyFilter(filter.LegacyFilterCode);
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
    private IFilterItemAdapter interaction;

    public FilterItemAdapter(IFilterItemAdapter interaction)
    {
        this.interaction = interaction;
    }

    public override long GetItemId(int position) => position;

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        var linearLayout = new LinearLayout(interaction.AdapterContext);
        linearLayout.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        linearLayout.Orientation = Orientation.Vertical;

        var label = new TextView(interaction.AdapterContext);
        var labelParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
        labelParams.Gravity = GravityFlags.Center;
        label.SetPadding(20, 20, 20, 20);

        var frame = new FrameLayout(interaction.AdapterContext);
        var frameLayout = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 5);
        frameLayout.Gravity = GravityFlags.End;
        frame.LayoutParameters = frameLayout;
        frame.SetBackgroundColor(Android.Graphics.Color.DarkGray);

        linearLayout.AddView(label);
        linearLayout.AddView(frame);

        label.TextAlignment = TextAlignment.Center;
        if (interaction.Filters[position].IsSection)
        {
            label.SetTypeface(Typeface.DefaultBold, TypefaceStyle.Bold);
            label.SetTextColor(Android.Graphics.Color.Red);
        }
        else
        {
            label.SetTextColor(Android.Graphics.Color.DarkGray);
        }

        label.Text = interaction.Filters[position].Title;

        return linearLayout;
    }

    public override int Count => this.interaction.Filters?.Count ?? 0;

    public override FilterItem this[int position] => this.interaction.Filters[position];
}