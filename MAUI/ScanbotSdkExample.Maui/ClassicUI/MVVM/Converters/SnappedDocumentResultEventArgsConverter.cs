using System.Globalization;
using ScanbotSDK.MAUI.Document.ClassicComponent;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.Converters;

public class SnappedDocumentResultEventArgsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value as SnappedDocumentResultEventArgs;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
