using ReadyToUseUI.iOS.Models;
using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.View;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public class FilterController : UIViewController
    {
        private FilterView ContentView;
        private SBSDKImageFilterType Choice;
        private SBSDKUIPage Temp;

        private static readonly List<Filter> Filters = new List<Filter>
        {
            new Filter("None", SBSDKImageFilterType.None),
            new Filter("Low Light Binarization", SBSDKImageFilterType.LowLightBinarization),
            new Filter("Low Light Binarization 2", SBSDKImageFilterType.LowLightBinarization2),
            new Filter("Edge Highlight", SBSDKImageFilterType.EdgeHighlight),
            new Filter("Deep Binarization", SBSDKImageFilterType.DeepBinarization),
            new Filter("Otsu Binarization", SBSDKImageFilterType.OtsuBinarization),
            new Filter("Clean Background", SBSDKImageFilterType.BackgroundClean),
            new Filter("Color Document", SBSDKImageFilterType.ColorDocument),
            new Filter("Color", SBSDKImageFilterType.Color),
            new Filter("Grayscale", SBSDKImageFilterType.Gray),
            new Filter("Binarized", SBSDKImageFilterType.Binarized),
            new Filter("Pure Binarized", SBSDKImageFilterType.PureBinarized),
            new Filter("Black & White", SBSDKImageFilterType.BlackAndWhite)
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
            Choice = e.Type;

            if (Temp == null)
            {
                Temp = PageRepository.DuplicateCurrent(Choice);
            }

            Temp.Filter = Choice;
            ContentView.ImageView.Image = Temp.DocumentImage;
        }

        private void FilterChosen(object sender, EventArgs e)
        {
            PageRepository.Apply(Choice, PageRepository.Current);
            NavigationController.PopViewController(true);
        }
    }
}
