using Android.Content;
using Android.Graphics;
using Android.Views;
using Google.Android.Material.BottomSheet;
using IO.Scanbot.Imagefilters;
using IO.Scanbot.Sdk.Process;
using ReadyToUseUI.Droid.Listeners;
using ReadyToUseUI.Droid.Model;
using Orientation = Android.Widget.Orientation;

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

    private List<FilterItem> filters;

    public FilterListFragment()
    {
        filters = new List<FilterItem>
        {
            new FilterItem("Parametric Filters"),
            new FilterItem(nameof(ScanbotBinarizationFilter), () => FilterSelected(new ScanbotBinarizationFilter())),
            new FilterItem(nameof(CustomBinarizationFilter), () => FilterSelected(new CustomBinarizationFilter())),
            new FilterItem(nameof(ColorDocumentFilter), () => FilterSelected(new ColorDocumentFilter())),
            new FilterItem(nameof(BrightnessFilter), () => FilterSelected(new BrightnessFilter())),
            new FilterItem(nameof(ContrastFilter), () => FilterSelected(new ContrastFilter())),
            new FilterItem(nameof(GrayscaleFilter), () => FilterSelected(new  GrayscaleFilter())),
            new FilterItem(nameof(WhiteBlackPointFilter), () => FilterSelected(new  WhiteBlackPointFilter())),

            new FilterItem("Legacy Filters"),
            new FilterItem(nameof(ImageFilterType.None), () => FilterSelected(new  LegacyFilter(ImageFilterType.None.Code))),
            new FilterItem(nameof(ImageFilterType.LowLightBinarization), () => FilterSelected(new LegacyFilter(ImageFilterType.LowLightBinarization.Code))),
            new FilterItem(nameof(ImageFilterType.LowLightBinarization2), () => FilterSelected(new LegacyFilter(ImageFilterType.LowLightBinarization2.Code))),
            new FilterItem(nameof(ImageFilterType.EdgeHighlight), () => FilterSelected(new LegacyFilter(ImageFilterType.EdgeHighlight.Code))),
            new FilterItem(nameof(ImageFilterType.DeepBinarization), () => FilterSelected(new LegacyFilter(ImageFilterType.DeepBinarization.Code))),
            new FilterItem(nameof(ImageFilterType.OtsuBinarization), () => FilterSelected(new LegacyFilter(ImageFilterType.OtsuBinarization.Code))),
            new FilterItem(nameof(ImageFilterType.BackgroundClean), () => FilterSelected(new LegacyFilter(ImageFilterType.BackgroundClean.Code))),
            new FilterItem(nameof(ImageFilterType.ColorDocument), () => FilterSelected(new LegacyFilter(ImageFilterType.ColorDocument.Code))),
            new FilterItem(nameof(ImageFilterType.ColorEnhanced), () => FilterSelected(new LegacyFilter(ImageFilterType.ColorEnhanced.Code))),
            new FilterItem(nameof(ImageFilterType.Grayscale), () => FilterSelected(new LegacyFilter(ImageFilterType.Grayscale.Code))),
            new FilterItem(nameof(ImageFilterType.PureGrayscale), () => FilterSelected(new LegacyFilter(ImageFilterType.PureGrayscale.Code))),
            new FilterItem(nameof(ImageFilterType.Binarized), () => FilterSelected(new LegacyFilter(ImageFilterType.Binarized.Code))),
            new FilterItem(nameof(ImageFilterType.PureBinarized), () => FilterSelected(new LegacyFilter(ImageFilterType.PureBinarized.Code))),
            new FilterItem(nameof(ImageFilterType.BlackAndWhite), () => FilterSelected(new LegacyFilter(ImageFilterType.BlackAndWhite.Code))),
        };
    }

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
            filters[e.Position].FilterSelected?.Invoke();
        }
    }

    private void FilterSelected(ParametricFilter filter)
    {
        (Activity as IFiltersListener).ApplyFilter(filter);
        DismissAllowingStateLoss();
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