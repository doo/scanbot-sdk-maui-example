using ScanbotSDK.MAUI.Core.Image;
using ScanbotSDK.MAUI.Image;

namespace ScanbotSdkExample.Maui.Snippets.ImageRefs;

public class SaveImageSnippet
{
    static void SaveImage(ImageRef imageRef, String destinationPath)
    {
        imageRef.SaveImage(
            destinationPath,
            options: new SaveImageOptions
            {
                Quality = 100,
                EncryptionMode = EncryptionMode.Auto,
                // to disable decryption while saving this specific file, use
                // EncryptionMode = EncryptionMode.Disabled,
            }
        );

        // Return the stored image as a byte array
        var byteArray = imageRef.EncodeImage();
    }
}