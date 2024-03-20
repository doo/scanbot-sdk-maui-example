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

        return new SBSDKIndexedImageStorage(location);
    }

    internal static Task<NSUrl> CreatePDFAsync(NSUrl[] inputUrls, NSUrl outputUrl, SBSDKPDFRendererPageSize pageSize = SBSDKPDFRendererPageSize.Custom,
                                                SBSDKPDFRendererPageOrientation orientation = SBSDKPDFRendererPageOrientation.Auto, SBSDKStorageCrypting encrypter = null,
                                                SBSDKOpticalCharacterRecognizerConfiguration ocrConfiguration = null)
    {
        TaskCompletionSource<NSUrl> task = new TaskCompletionSource<NSUrl>();

        var storage = CreateStorage(inputUrls, encrypter);
        var outputPdfUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".pdf");
        // Create the PDF rendering options.
        var options = new SBSDKPDFRendererOptions(pageSize: pageSize,
                                                pageFitMode: SBSDKPDFRendererPageFitMode.FitIn,
                                                pageOrientation: orientation,
                                                ocrConfiguration: ocrConfiguration,
                                                dpi: 300,
                                                resample: true,
                                                jpegQuality: 80);

        // Create the PDF renderer and pass the PDF options to it.
        var renderer = new SBSDKPDFRenderer(options);

        if (encrypter != null)
        {
            renderer.RenderImageStorage(storage, indexSet: null, encrypter: encrypter, output: outputPdfUrl, completion: (isComplete, error) =>
            {
                storage.RemoveAllImages();
                if (error != null)
                {
                    throw new NSErrorException(error);
                }
                task.SetResult(outputPdfUrl);
            });
        }
        else
        {
            renderer.RenderImageStorage(storage, indexSet: null, output: outputPdfUrl, encrypter: null, completion: (isComplete, error) =>
            {
                storage.RemoveAllImages();
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
                                                                    NSUrl[] inputUrls, NSUrl outputUrl, bool shouldGeneratePdf = true,
                                                                    SBSDKPDFRendererPageSize pageSize = SBSDKPDFRendererPageSize.Custom,
                                                                    SBSDKPDFRendererPageOrientation orientation = SBSDKPDFRendererPageOrientation.Auto, SBSDKStorageCrypting encrypter = null)
    {
        var storage = CreateStorage(inputUrls, encrypter);
        var result = await RecognizeText(ocrRecognizer, storage);

        if (result.error != null)
        {
            storage.RemoveAllImages();
            throw new NSErrorException(result.error);
        }

        if (shouldGeneratePdf)
        {
            outputUrl = await CreatePDFAsync(inputUrls, outputUrl, pageSize, orientation, encrypter, ocrRecognizer.Configuration);
        }
        storage.RemoveAllImages();
        return (result.result, outputUrl);
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

        var images = LoadImagesFromUrl(inputUrls.ToList()).ToArray();

        var write = new SBSDKTIFFImageWriter(parameters, encrypter);

        success = write.WriteTIFFWithToFile(images, outputTiffUrl);
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
