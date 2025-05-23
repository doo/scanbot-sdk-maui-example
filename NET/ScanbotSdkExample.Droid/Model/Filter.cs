namespace ScanbotSdkExample.Droid.Model;

public enum FilterType
{
    LegacyFilter,
    NewFilter
}

public struct FilterItem
{
    public string Title { get; set; }
    public bool IsSection { get; set; }
    
    public Action FilterSelected { get; set; }
 
    public FilterItem(string filterTitle, Action filterSelected = null)
    {
        Title = filterTitle;
        FilterSelected = filterSelected;
        IsSection = filterSelected == null;
    }
}