using ScanbotSDK.MAUI;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Snippets;

public static class MockCameraSnippet
{
    public static async Task ConfigureMockCameraAsync()
    {
        var imagePath = await ImagePicker.PickImageAsPathAsync();
        if (string.IsNullOrWhiteSpace(imagePath)) return;

        if (ImageSource.FromFile(imagePath) is not FileImageSource fileImage) return;

        ScanbotSDKMain.MockCamera(fileImage.File);
    }
}