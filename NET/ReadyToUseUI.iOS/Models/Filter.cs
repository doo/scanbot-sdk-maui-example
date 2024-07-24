using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Models
{
    public enum FilterType
    {
        LegacyFilter,
        NewFilter
    }

    public struct Filter
    {
        public string Title { get; set; }

        public FilterType FilterType { get; set; }

        public bool IsSection { get; set; }

        public Filter(string sectionTitle, bool isSection)
        {
            IsSection = isSection;
            Title = sectionTitle;
        }
        
        public Filter(SBSDKImageFilterType filter)
        {
            Title = filter.ToString();
            FilterType = FilterType.LegacyFilter;
        }
        
        public Filter(Type parametricFilterType)
        {
            Title = parametricFilterType.Name;
            FilterType = FilterType.NewFilter;
        }
    }
}
