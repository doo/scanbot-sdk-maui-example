using ReadyToUseUI.iOS.Models;
using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.View;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public class FilterController : UIViewController
    {
        private FilterView ContentView;
        private SBSDKParametricFilter SelectedParametricFilter;
        private SBSDKScannedPage Temp;
        private readonly List<FilterItem> Filters;

        public FilterController()
        {
            this.Filters = new List<FilterItem>
            {
                new FilterItem("New Filters"),
                new FilterItem(nameof(SBSDKScanbotBinarizationFilter), () => OnFilterSelected(new SBSDKScanbotBinarizationFilter(outputMode: SBSDKOutputMode.Binary))),
                new FilterItem(nameof(SBSDKCustomBinarizationFilter), () => OnFilterSelected(new SBSDKCustomBinarizationFilter(outputMode: SBSDKOutputMode.Binary, denoise: 0.5, radius: 32, preset: SBSDKBinarizationFilterPreset.SBSDKBinarizationFilterPresetPreset1))),
                new FilterItem(nameof(SBSDKColorDocumentFilter), () => OnFilterSelected(new SBSDKColorDocumentFilter())),
                new FilterItem(nameof(SBSDKBrightnessFilter), () => OnFilterSelected(new SBSDKBrightnessFilter(brightness: 0.2))),
                new FilterItem(nameof(SBSDKContrastFilter), () => OnFilterSelected(new SBSDKContrastFilter(contrast: 2))),
                new FilterItem(nameof(SBSDKGrayscaleFilter), () => OnFilterSelected(new SBSDKGrayscaleFilter(blackOutliersFraction: 0.0, borderWidthFraction: 0.6, whiteOutliersFraction: 0.02))),
                new FilterItem(nameof(SBSDKWhiteBlackPointFilter), () => OnFilterSelected(new SBSDKWhiteBlackPointFilter(blackPoint: 0.2, whitePoint: 0.8))),

                new FilterItem("Legacy Filters"),
                new FilterItem(nameof(SBSDKImageFilterType.None), () => OnFilterSelected(SBSDKImageFilterType.None.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.LowLightBinarization),() => OnFilterSelected(SBSDKImageFilterType.LowLightBinarization.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.LowLightBinarization2),() => OnFilterSelected(SBSDKImageFilterType.LowLightBinarization2.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.EdgeHighlight),() => OnFilterSelected(SBSDKImageFilterType.EdgeHighlight.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.DeepBinarization),() => OnFilterSelected(SBSDKImageFilterType.DeepBinarization.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.OtsuBinarization),() => OnFilterSelected(SBSDKImageFilterType.OtsuBinarization.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.BackgroundClean),() => OnFilterSelected(SBSDKImageFilterType.BackgroundClean.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.ColorDocument),() => OnFilterSelected(SBSDKImageFilterType.ColorDocument.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.Color),() => OnFilterSelected(SBSDKImageFilterType.Color.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.Gray),() => OnFilterSelected(SBSDKImageFilterType.Gray.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.Binarized),() => OnFilterSelected(SBSDKImageFilterType.Binarized.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.PureBinarized),() => OnFilterSelected(SBSDKImageFilterType.PureBinarized.ToLegacyFilter())),
                new FilterItem(nameof(SBSDKImageFilterType.BlackAndWhite),() => OnFilterSelected(SBSDKImageFilterType.BlackAndWhite.ToLegacyFilter())),
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new FilterView();
            View = ContentView;

            ContentView.SetPickerModel(Filters);
            ContentView.ImageView.Image = PageRepository.Current.DocumentImage;

            Title = "Choose filter";

            var button = new UIBarButtonItem("Apply", UIBarButtonItemStyle.Done, FilterChosen);
            NavigationItem.SetRightBarButtonItem(button, false);
        }
        
        private void OnFilterSelected(SBSDKParametricFilter filter)
        {
            SelectedParametricFilter = filter;
            if (Temp == null)
            {
                Temp = PageRepository.DuplicateCurrent(SelectedParametricFilter);
            }

            Temp.Filters = new[] { SelectedParametricFilter };
            ContentView.ImageView.Image = Temp.DocumentImage;
        }

        private void FilterChosen(object sender, EventArgs e)
        {
            PageRepository.Apply(SelectedParametricFilter, PageRepository.Current);
            NavigationController.PopViewController(true);
        }
    }
}
