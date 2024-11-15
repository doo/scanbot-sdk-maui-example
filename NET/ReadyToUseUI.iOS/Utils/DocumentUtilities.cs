using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Utils;

public class DocumentUtilities
{
    public static SBSDKIndexedImageStorage CreateStorage(NSUrl[] uris, SBSDKStorageCrypting encrypter)
    {
        var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
        var tmp = NSUrl.FromFilename(string.Format("{0}/{1}", url.Scheme == "file" ? url.Path : url.AbsoluteString, Guid.NewGuid()));
        var location = new SBSDKStorageLocation(tmp);
        var format = SBSDKImageFileFormat.Jpeg;

        return new SBSDKIndexedImageStorage(location, format, encrypter, uris);
    }

    internal static async Task<(SBSDKOCRResult, NSUrl)> PerformOCRAsync(SBSDKOpticalCharacterRecognizer ocrRecognizer,
                                                                    NSUrl[] inputUrls, NSUrl outputUrl, bool shouldGeneratePdf = true,
                                                                    SBSDKPDFRendererPageSize pageSize = SBSDKPDFRendererPageSize.Custom,
                                                                    SBSDKPDFRendererPageOrientation orientation = SBSDKPDFRendererPageOrientation.Auto, SBSDKStorageCrypting encrypter = null)
    {
        var storage = CreateStorage(inputUrls, encrypter);
        var (ocrResult, error) = await RecognizeText(ocrRecognizer, storage);

        if (error != null)
        {
            storage.RemoveAllImages();
            throw new NSErrorException(error);
        }

        if (shouldGeneratePdf)
        {
            // outputUrl = await CreatePDFAsync(inputUrls, outputUrl, pageSize, orientation, encrypter, ocrRecognizer.Configuration);
        }
        storage.RemoveAllImages();
        return (ocrResult, outputUrl);
    }

    private static Task<(SBSDKOCRResult result, NSError error)> RecognizeText(SBSDKOpticalCharacterRecognizer opticalCharacterRecognizer, SBSDKImageStoring storage)
    {
        TaskCompletionSource<(SBSDKOCRResult, NSError)> task = new TaskCompletionSource<(SBSDKOCRResult, NSError)>();
        opticalCharacterRecognizer.RecognizeOnImageStorage(storage, completion: (result, error) =>
        {
            task.SetResult((result, error));
        });
        return task.Task;
    }

    internal static (bool, NSUrl) CreateTIFF(SBSDKTIFFImageWriterParameters parameters, NSUrl[] inputUrls, NSUrl outputUrl, SBSDKStorageCrypting encrypter = null)
    {
        bool success;
        var outputTiffUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".tiff");
        
        if (encrypter != null)
        {
            success = new SBSDKTIFFImageWriter(parameters: parameters).WriteTIFFFromToFile(inputUrls, outputTiffUrl);
        }
        else
        {
            success = new SBSDKTIFFImageWriter(parameters: parameters, encrypter: encrypter).WriteTIFFFromToFile(inputUrls, outputTiffUrl);
        }
        return (success, outputTiffUrl);
    }

    internal static List<UIImage> LoadImagesFromUrl(List<NSUrl> urls)
    {
        var result = new List<UIImage>();

        foreach (var url in urls)
        {
            var data = NSData.FromUrl(url);
            result.Add(UIImage.LoadFromData(data));
        }
        return result;
    }
}
