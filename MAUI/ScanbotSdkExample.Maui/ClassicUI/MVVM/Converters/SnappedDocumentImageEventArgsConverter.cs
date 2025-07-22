using System.Globalization;
using ScanbotSDK.MAUI.Document.ClassicComponent;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.Converters;

public class SnappedDocumentImageEventArgsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var eventArgs = value as SnappedDocumentImageResultEventArgs;
        return eventArgs;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}