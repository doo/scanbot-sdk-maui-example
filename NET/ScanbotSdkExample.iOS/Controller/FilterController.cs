using ScanbotSdkExample.iOS.Models;
using ScanbotSdkExample.iOS.View;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller
{
    public class FilterController : UIViewController
    {
        private FilterView _contentView;
        private SBSDKParametricFilter _selectedParametricFilter;
        private SBSDKScannedDocument _scannedDocument;
        private readonly List<FilterItem> _filters;
        private Action<SBSDKParametricFilter> _onFilterSelected;

        public FilterController()
        {
            _filters =
            [
                new FilterItem("New Filters"),
                new FilterItem(nameof(SBSDKScanbotBinarizationFilter), () => OnFilterSelected(new SBSDKScanbotBinarizationFilter(outputMode: SBSDKOutputMode.Binary))),
                new FilterItem(nameof(SBSDKCustomBinarizationFilter), () => OnFilterSelected(new SBSDKCustomBinarizationFilter(outputMode: SBSDKOutputMode.Binary, denoise: 0.5, radius: 32, preset: SBSDKBinarizationFilterPreset.SBSDKBinarizationFilterPresetPreset1))),
                new FilterItem(nameof(SBSDKColorDocumentFilter), () => OnFilterSelected(new SBSDKColorDocumentFilter())),
                new FilterItem(nameof(SBSDKBrightnessFilter), () => OnFilterSelected(new SBSDKBrightnessFilter(brightness: 0.2))),
                new FilterItem(nameof(SBSDKContrastFilter), () => OnFilterSelected(new SBSDKContrastFilter(contrast: 2))),
                new FilterItem(nameof(SBSDKGrayscaleFilter), () => OnFilterSelected(new SBSDKGrayscaleFilter(blackOutliersFraction: 0.0, borderWidthFraction: 0.6, whiteOutliersFraction: 0.02))),
                new FilterItem(nameof(SBSDKWhiteBlackPointFilter), () => OnFilterSelected(new SBSDKWhiteBlackPointFilter(blackPoint: 0.2, whitePoint: 0.8)))
            ];
        }

        internal void NavigateData(Action<SBSDKParametricFilter> onFilterSelected, SBSDKScannedDocument scannedDocument)
        {
            _scannedDocument = scannedDocument;
            _onFilterSelected = onFilterSelected;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _contentView = new FilterView();
            View = _contentView;

            _contentView.SetPickerModel(_filters);
            _contentView.ImageView.Image = _scannedDocument.Pages.First().DocumentImage;

            Title = "Choose filter";

            var button = new UIBarButtonItem("Apply", UIBarButtonItemStyle.Done, FilterChosen);
            NavigationItem.SetRightBarButtonItem(button, false);
        }
        
        private void OnFilterSelected(SBSDKParametricFilter filter)
        {
            _selectedParametricFilter = filter;

            foreach (var page in _scannedDocument.Pages)
            {
                page.Filters = [ _selectedParametricFilter ];
            }
            
            _contentView.ImageView.Image = _scannedDocument.Pages.First().DocumentImage;
        }

        private void FilterChosen(object sender, EventArgs e)
        {
            _onFilterSelected?.Invoke(_selectedParametricFilter);
            NavigationController?.PopViewController(true);
        }
    }
}
