using Microsoft.Maui.Graphics.Platform;

namespace ClassicComponent.Maui.Utils;

public static class ViewUtils
{
    /// <summary>
    /// Gets the ImageSource from PlatformImage.
    /// </summary>
    /// <param name="image">Platform image object</param>
    /// <param name="quality">Image quality. Defaults to 0.8f</param>
    /// <returns></returns>
    internal static ImageSource ToImageSource(this PlatformImage image, float quality = 0.8f)
    {
        var format = DeviceInfo.Platform == DevicePlatform.Android ? ImageFormat.Jpeg : ImageFormat.Bmp; 
        return ImageSource.FromStream(() => image.AsStream(format, quality));
    }
}