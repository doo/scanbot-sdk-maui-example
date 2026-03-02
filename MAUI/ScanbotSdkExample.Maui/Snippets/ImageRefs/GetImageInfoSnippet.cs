using ScanbotSDK.MAUI.Core.Image;
using ScanbotSDK.MAUI.Image;

namespace ScanbotSdkExample.Maui.Snippets.ImageRefs;

public class GetImageInfoSnippet
{
    static ImageInfo GetImageInfo(ImageRef imageRef) {
        var imageInfo = imageRef.Info();

        var width = imageInfo.Width;
        var height = imageInfo.Height;
        // size on disk or in memory depending on load mode
        var maxByteSize = imageInfo.MaxByteSize;

        return imageInfo;
    }
}