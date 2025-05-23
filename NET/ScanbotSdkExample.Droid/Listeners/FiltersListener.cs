using IO.Scanbot.Sdk.Imagefilters;

namespace ScanbotSdkExample.Droid.Listeners
{
    public interface IFiltersListener
    {
        void ApplyFilter(ParametricFilter type);
    }
}
