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

    internal static Task<NSUrl> CreatePDFAsync(NSUrl[] inputUrls, NSUrl outputUrl, SBSDKPDFRendererPageSize pageSize = SBSDKPDFRendererPageSize.Custom,
                                                SBSDKPDFRendererPageOrientation orientation = SBSDKPDFRendererPageOrientation.Auto, SBSDKStorageCrypting encrypter = null,
                                                SBSDKOpticalCharacterRecognizerConfiguration ocrConfiguration = null)
    {
        TaskCompletionSource<NSUrl> task = new TaskCompletionSource<NSUrl>();

        var storage = CreateStorage(inputUrls, encrypter);
        var outputPdfUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".pdf");
        // Create the PDF rendering options.
        var options = new SBSDKPDFRendererOptions(pageSize: pageSize,
                                                pageOrientation: orientation,
                                                ocrConfiguration: ocrConfiguration);

        // Create the PDF renderer and pass the PDF options to it.
        var renderer = new SBSDKPDFRenderer(options);

        if (encrypter != null)
        {
            renderer.RenderImageStorage(storage, indices: null, encrypter: encrypter, pdfOutputURL: outputPdfUrl, completion: (isComplete, error) =>
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
            renderer.RenderImageStorage(storage, indices: null, pdfOutputURL: outputPdfUrl, completion: (isComplete, error) =>
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
        var (ocrResult, error) = await RecognizeText(ocrRecognizer, storage);

        if (error != null)
        {
            storage.RemoveAllImages();
            throw new NSErrorException(error);
        }

        if (shouldGeneratePdf)
        {
            outputUrl = await CreatePDFAsync(inputUrls, outputUrl, pageSize, orientation, encrypter, ocrRecognizer.Configuration);
        }
        storage.RemoveAllImages();
        return (ocrResult, outputUrl);
    }

    private static Task<(SBSDKOCRResult result, NSError error)> RecognizeText(SBSDKOpticalCharacterRecognizer opticalCharacterRecognizer, SBSDKImageStoring storage)
    {
        TaskCompletionSource<(SBSDKOCRResult, NSError)> task = new TaskCompletionSource<(SBSDKOCRResult, NSError)>();
        opticalCharacterRecognizer.RecognizeText(storage, completion: (result, error) =>
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

        if (encrypter != null)
        {
            success = SBSDKTIFFImageWriter.WriteTIFF(images, outputTiffUrl, encrypter, parameters);
        }
        else
        {
            success = SBSDKTIFFImageWriter.WriteTIFF(images, outputTiffUrl, parameters);
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
