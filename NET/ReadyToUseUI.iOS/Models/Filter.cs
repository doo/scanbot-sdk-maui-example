using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Models
{
    public struct Filter
    {
        public Filter(string title, SBSDKImageFilterType type)
        {
            Title = title;
            Type = type;
        }

        public string Title { get; set; }

        public SBSDKImageFilterType Type { get; set; }
    }
}
