using IO.Scanbot.Sdk.Imageprocessing;

namespace ScanbotSdkExample.Droid.Listeners
{
    public interface IFiltersListener
    {
        void ApplyFilter(ParametricFilter type);
    }
}
