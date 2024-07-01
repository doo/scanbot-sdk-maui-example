using Android.Views;
using Google.Android.Material.BottomSheet;
using ReadyToUseUI.Droid.Listeners;
using DocumentSDK.NET.Model;
using IO.Scanbot.Sdk.Process;

namespace ReadyToUseUI.Droid.Fragments
{
    public class FilterBottomSheetMenuFragment : BottomSheetDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.filters_bottom_sheet, container, false);

            var lowLightBinarizationFilter = view.FindViewById<Button>(Resource.Id.lowLightBinarizationFilter);
            lowLightBinarizationFilter.Text = Texts.low_light_binarization_filter;
            lowLightBinarizationFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.LowLightBinarization);
                DismissAllowingStateLoss();
            };

            var lowLightBinarizationFilter2 = view.FindViewById<Button>(Resource.Id.lowLightBinarizationFilter2);
            lowLightBinarizationFilter2.Text = Texts.low_light_binarization_filter2;
            lowLightBinarizationFilter2.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.LowLightBinarization2);
                DismissAllowingStateLoss();
            };

            var edgeHighlightFilter = view.FindViewById<Button>(Resource.Id.edgeHighlightFilter);
            edgeHighlightFilter.Text = Texts.edge_highlight_filter;
            edgeHighlightFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.EdgeHighlight);
                DismissAllowingStateLoss();
            };

            var deepBinarizationFilter = view.FindViewById<Button>(Resource.Id.deepBinarizationFilter);
            deepBinarizationFilter.Text = Texts.deep_binarization_filter;
            deepBinarizationFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.DeepBinarization);
                DismissAllowingStateLoss();
            };


            var otsuBinarizationFilter = view.FindViewById<Button>(Resource.Id.otsuBinarizationFilter);
            otsuBinarizationFilter.Text = Texts.otsu_binarization_filter;
            otsuBinarizationFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.OtsuBinarization);
                DismissAllowingStateLoss();
            };


            var cleanBackgroundFilter = view.FindViewById<Button>(Resource.Id.cleanBackgroundFilter);
            cleanBackgroundFilter.Text = Texts.clean_background_filter;
            cleanBackgroundFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.BackgroundClean);
                DismissAllowingStateLoss();
            };

            var colorDocumentFilter = view.FindViewById<Button>(Resource.Id.colorDocumentFilter);
            colorDocumentFilter.Text = Texts.color_document_filter;
            colorDocumentFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.ColorDocument);
                DismissAllowingStateLoss();
            };

            var colorFilter = view.FindViewById<Button>(Resource.Id.colorFilter);
            colorFilter.Text = Texts.color_filter;
            colorFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.ColorEnhanced);
                DismissAllowingStateLoss();
            };

            var grayscaleFilter = view.FindViewById<Button>(Resource.Id.grayscaleFilter);
            grayscaleFilter.Text = Texts.grayscale_filter;
            grayscaleFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.Grayscale);
                DismissAllowingStateLoss();
            };

            var binarizedFilter = view.FindViewById<Button>(Resource.Id.binarizedFilter);
            binarizedFilter.Text = Texts.binarizedfilter;
            binarizedFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.Binarized);
                DismissAllowingStateLoss();
            };

            var pureBinarizedFilter = view.FindViewById<Button>(Resource.Id.pureBinarizedFilter);
            pureBinarizedFilter.Text = Texts.pure_binarized_filter;
            pureBinarizedFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.PureBinarized);
                DismissAllowingStateLoss();
            };

            var blackAndWhiteFilter = view.FindViewById<Button>(Resource.Id.blackAndWhiteFilter);
            blackAndWhiteFilter.Text = Texts.black_amp_white_filter;
            blackAndWhiteFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.BlackAndWhite);
                DismissAllowingStateLoss();
            };

            var noneFilter = view.FindViewById<Button>(Resource.Id.none);
            noneFilter.Text = Texts.none;
            noneFilter.Click += delegate {
                (Activity as IFiltersListener).ApplyFilter(ImageFilterType.None);
                DismissAllowingStateLoss();
            };

            return view;
        }
    }
}
