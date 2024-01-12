using ScanbotSDK.iOS;

namespace ClassicComponent.iOS.Utils;

internal class DocumentUtilities
{
    private static SBSDKIndexedImageStorage tempImageStorage;
    internal static SBSDKIndexedImageStorage GetTemporaryStorage(bool forceInit = false)
    {
        if (forceInit || tempImageStorage == null)
        {
            tempImageStorage?.RemoveAllImages();
            var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
            var tmp = NSUrl.FromFilename(string.Format("{0}/{1}", url.Scheme == "file" ? url.Path : url.AbsoluteString, Guid.NewGuid()));
            var location = new SBSDKStorageLocation(tmp);
            tempImageStorage = new SBSDKIndexedImageStorage(location, SBSDKImageFileFormat.Jpeg, ScanbotSDKUI.DefaultImageStoreEncrypter);
            return tempImageStorage;
        }
        return tempImageStorage;
    }

    internal static Task<NSUrl> CreatePDFAsync(SBSDKPDFRendererPageSize pageSize = SBSDKPDFRendererPageSize.Custom,
                                                SBSDKPDFRendererPageOrientation orientation = SBSDKPDFRendererPageOrientation.Auto, SBSDKStorageCrypting encrypter = null,
                                                SBSDKOpticalCharacterRecognizerConfiguration ocrConfiguration = null)
    {
        TaskCompletionSource<NSUrl> task = new TaskCompletionSource<NSUrl>();
        var outputPdfUrl = Utilities.GenerateRandomFileUrlInDemoTempStorage(".pdf");
        // Create the PDF rendering options.
        var options = new SBSDKPDFRendererOptions(pageSize: pageSize,
                                                pageOrientation: orientation,
                                                ocrConfiguration: ocrConfiguration);

        // Create the PDF renderer and pass the PDF options to it.
        var renderer = new SBSDKPDFRenderer(options);
        if (encrypter != null)
        {
            renderer.RenderImageStorage(tempImageStorage, indices: null, encrypter: encrypter, pdfOutputURL: outputPdfUrl, completion: (isComplete, error) =>
            {
                if (error != null)
                {
                    throw new NSErrorException(error);
                }
                task.SetResult(outputPdfUrl);
            });
        }
        else
        {
            renderer.RenderImageStorage(tempImageStorage, indices: null, pdfOutputURL: outputPdfUrl, completion: (isComplete, error) =>
            {
                if (error != null)
                {
                    throw new NSErrorException(error);
                }
                task.SetResult(outputPdfUrl);
            });
        }
        return task.Task;
    }

    internal static async Task<(SBSDKOCRResult, NSUrl)> PerformOCRAsync(SBSDKOpticalCharacterRecognizer ocrRecognizer,
                                                                    bool shouldGeneratePdf = true,
                                                                    SBSDKPDFRendererPageSize pageSize = SBSDKPDFRendererPageSize.Custom,
                                                                    SBSDKPDFRendererPageOrientation orientation = SBSDKPDFRendererPageOrientation.Auto, SBSDKStorageCrypting encrypter = null)
    {
        var image = GetImageFromStorage(index: 0); // we only have one word.
        var (ocrResult, error) = await RecognizeOnImage(ocrRecognizer, image);
        NSUrl outputPdfUrl = null;
        if (error != null)
        {
            throw new NSErrorException(error);
        }

        if (shouldGeneratePdf)
        {
            outputPdfUrl = await CreatePDFAsync(pageSize, orientation, encrypter, ocrRecognizer.Configuration);
        }
        return (ocrResult, outputPdfUrl);
    }

    private static Task<(SBSDKOCRResult result, NSError error)> RecognizeOnImage(SBSDKOpticalCharacterRecognizer opticalCharacterRecognizer, UIImage image)
    {
        TaskCompletionSource<(SBSDKOCRResult, NSError)> task = new TaskCompletionSource<(SBSDKOCRResult, NSError)>();
        opticalCharacterRecognizer.RecognizeOnImage(image, completion: (result, error) =>
        {
            task.SetResult((result, error));
        });
        return task.Task;
    }

    internal static (bool, NSUrl) CreateTIFF(SBSDKTIFFImageWriterParameters parameters, NSUrl[] inputUrls = null, SBSDKStorageCrypting encrypter = null)
    {
        bool success;
        var outputTiffUrl = Utilities.GenerateRandomFileUrlInDemoTempStorage(".tiff");
        var images = LoadImagesFromUrl(inputUrls.ToList()).ToArray(); // decrypt images before writing TIFF
        success = SBSDKTIFFImageWriter.WriteTIFF(images, outputTiffUrl, parameters);
        return (success, outputTiffUrl);
    }

    internal static List<UIImage> LoadImagesFromUrl(List<NSUrl> urls)
    {
        var result = new List<UIImage>();
        foreach (var url in urls)
        {
            result.Add(UIImage.LoadFromData(Decrypt(url)));
        }
        return result;
    }

    private static UIImage GetImageFromStorage(uint index)
    {
        UIImage image;
        if (ScanbotSDKUI.DefaultImageStoreEncrypter != null)
        {
            var imageData = Decrypt(tempImageStorage.ImageURLAtIndex(index));
            image = UIImage.LoadFromData(imageData);
        }
        else
        {
            image = tempImageStorage.ImageAtIndex(index);
        }
        return image;
    }

    internal static NSData Decrypt(NSUrl url)
    {
        var data = NSData.FromUrl(url);
        var decrypter = ScanbotSDKUI.DefaultImageStoreEncrypter;
        if (decrypter == null)
        {
            return data;
        }
        return decrypter.DecryptData(data);
    }
}
