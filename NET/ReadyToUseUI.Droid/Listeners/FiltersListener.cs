using IO.Scanbot.Imagefilters;
using IO.Scanbot.Sdk.Process;

namespace ReadyToUseUI.Droid.Listeners
{
    public interface IFiltersListener
    {
        void ApplyFilter(ParametricFilter type);
    }
}
