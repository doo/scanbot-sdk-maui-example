using ReadyToUseUI.iOS.Models;
using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.View;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public class FilterController : UIViewController
    {
        private FilterView ContentView;
        private Filter SelectedFilter = new Filter(SBSDKImageFilterType.None);
        private SBSDKParametricFilter SelectedParametricFilter => GetParametricFilter(SelectedFilter);
        private SBSDKDocumentPage Temp;

        private readonly List<Filter> Filters = new List<Filter>
        {
            new Filter("New Filters", true),
            new Filter(typeof(SBSDKScanbotBinarizationFilter)),
            new Filter(typeof(SBSDKCustomBinarizationFilter)),
            new Filter(typeof(SBSDKColorDocumentFilter)),
            new Filter(typeof(SBSDKBrightnessFilter)),
            new Filter(typeof(SBSDKContrastFilter)),
            new Filter(typeof(SBSDKGrayscaleFilter)),
            new Filter(typeof(SBSDKWhiteBlackPointFilter)),
            
            new Filter("Legacy Filters", true),
            new Filter(SBSDKImageFilterType.None),
            new Filter(SBSDKImageFilterType.LowLightBinarization),
            new Filter(SBSDKImageFilterType.LowLightBinarization2),
            new Filter(SBSDKImageFilterType.EdgeHighlight),
            new Filter(SBSDKImageFilterType.DeepBinarization),
            new Filter(SBSDKImageFilterType.OtsuBinarization),
            new Filter(SBSDKImageFilterType.BackgroundClean),
            new Filter(SBSDKImageFilterType.ColorDocument),
            new Filter(SBSDKImageFilterType.Color),
            new Filter(SBSDKImageFilterType.Gray),
            new Filter(SBSDKImageFilterType.Binarized),
            new Filter(SBSDKImageFilterType.PureBinarized),
            new Filter(SBSDKImageFilterType.BlackAndWhite),
        };

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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ContentView.Model.SelectionChanged += OnFilterSelected;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            ContentView.Model.SelectionChanged -= OnFilterSelected;
        }

        private void OnFilterSelected(object sender, FilterEventArgs e)
        {
            SelectedFilter = e.Type;

            if (Temp == null)
            {
                Temp = PageRepository.DuplicateCurrent(SelectedParametricFilter);
            }

            Temp.ParametricFilters = new[] { SelectedParametricFilter };
            ContentView.ImageView.Image = Temp.DocumentImage;
        }

        private void FilterChosen(object sender, EventArgs e)
        {
            PageRepository.Apply(SelectedParametricFilter, PageRepository.Current);
            PageRepository.UpdateCurrent(PageRepository.Current.DocumentImage, PageRepository.Current.Polygon);
            NavigationController.PopViewController(true);
        }

        private SBSDKParametricFilter GetParametricFilter(Filter filter)
        {
            if (filter.FilterType == FilterType.LegacyFilter)
            {
                SBSDKImageFilterType legacyFilter;
                Enum.TryParse(filter.Title, true, out legacyFilter);
                return new SBSDKLegacyFilter((int)legacyFilter);
            }

            switch (filter.Title)
            {
                case nameof(SBSDKScanbotBinarizationFilter):
                    return new SBSDKScanbotBinarizationFilter();
                
                case nameof(SBSDKCustomBinarizationFilter):
                    return new SBSDKCustomBinarizationFilter();
                
                case nameof(SBSDKColorDocumentFilter):
                    return new SBSDKColorDocumentFilter();
                
                case nameof(SBSDKBrightnessFilter):
                    return new SBSDKBrightnessFilter();
                
                case nameof(SBSDKContrastFilter):
                    return new SBSDKContrastFilter();
                
                case nameof(SBSDKGrayscaleFilter):
                    return new SBSDKGrayscaleFilter();
                
                case nameof(SBSDKWhiteBlackPointFilter):
                    return new SBSDKWhiteBlackPointFilter();
            }
            
            return new SBSDKLegacyFilter((int)SBSDKImageFilterType.None);
        }
    }
}
