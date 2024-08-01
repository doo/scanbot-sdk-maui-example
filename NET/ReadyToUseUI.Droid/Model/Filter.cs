using IO.Scanbot.Sdk.Process;

namespace ReadyToUseUI.Droid.Model;

public enum FilterType
{
    LegacyFilter,
    NewFilter
}

public struct FilterItem
{
    public string Title { get; set; }

    public FilterType FilterType { get; set; }

    public bool IsSection { get; set; }

    public int LegacyFilterCode { get; set; }

    public FilterItem(string sectionTitle, bool isSection)
    {
        IsSection = isSection;
        Title = sectionTitle;
    }
    
    public FilterItem(ImageFilterType filter)
    {
        Title = filter.FilterName;
        FilterType = FilterType.LegacyFilter;
        LegacyFilterCode = filter.Code;
    }
    
    public FilterItem(Type parametricFilterType)
    {
        Title = parametricFilterType.Name;
        FilterType = FilterType.NewFilter;
    }
}