using ReadyToUseUI.iOS.Models;
using ReadyToUseUI.iOS.Utils;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class ScannedDocumentsViewController : UIViewController
{
    private void OnExportButtonClick(object sender, EventArgs e)
    {
        var input = _scannedDocument.DocumentImageURLs;
        var docs = NSSearchPathDirectory.DocumentDirectory;
        var url = NSFileManager.DefaultManager.GetUrls(docs, NSSearchPathDomain.User)[0];
        var alertController = UIAlertController.Create(Texts.save, Texts.SaveHow, UIAlertControllerStyle.ActionSheet);
        if (!ScanbotSDKGlobal.IsLicenseValid)
        {
            var title = "Oops";
            var body = "Your license has expired";
            Alert.Show(this, title, body);
            return;
        }

        var pdf = CreateButton(Texts.save_without_ocr, (action) =>  CreatePdfAsync(input, url));
        var ocr = CreateButton(Texts.save_with_ocr, (action) =>  PerformOcrAndCreatePdfAsync(input, url));
        var tiff = CreateButton(Texts.Tiff, (action) => WriteTiff(input, url));
        var cancel = CreateButton("Cancel", delegate { }, UIAlertActionStyle.Cancel);

        alertController.AddAction(pdf);
        alertController.AddAction(ocr);
        alertController.AddAction(tiff);
        alertController.AddAction(cancel);

        UIPopoverPresentationController presentationPopover = alertController.PopoverPresentationController;
        if (presentationPopover != null)
        {
            presentationPopover.SourceView = View;
            presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
        }

        PresentViewController(alertController, true, null);
    }
    
	private void CreatePdfAsync(NSUrl[] inputUrls, NSUrl outputUrl)
    {
        try
        {
            var storage = CreateStorage(inputUrls, ScanbotUI.DefaultImageStoreEncrypter);
            var outputPdfUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".pdf");
            
            // Create the PDF rendering options.
            var options = new SBSDKPDFRendererOptions();
            
            options.Dpi = 200;
            options.Resample = false;
            options.JpegQuality = 100;
            options.PageSize = SBSDKPDFRendererPageSize.A4;
            options.PageOrientation = SBSDKPDFRendererPageOrientation.Auto;
            options.PageFitMode = SBSDKPDFRendererPageFitMode.FitIn;
            options.PdfAttributes =  new SBSDKPDFAttributes(
                                author: "Your author",
                                creator: "Your creator",
                                title: "Your title",
                                subject: "Your subject",
                                keywords: ["PDF", "Scanbot", "SDK"]);

            // Create the PDF renderer and pass the PDF options to it.
            var renderer = new SBSDKPDFRenderer(options,  ScanbotUI.DefaultImageStoreEncrypter);
    
            renderer.RenderImageStorageAsync(imageStorage: storage, indexSet: null, output: outputPdfUrl,
                                completionHandler: (isComplete, error) =>
                                {
                                    storage.RemoveAllImages();
                                    if (error != null)
                                    {
                                        throw new NSErrorException(error);
                                    }
                                    
                                    OpenDocument(outputPdfUrl, false);
                                });
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Create PDF", ex.Message);
        }
    }

    private void PerformOcrAndCreatePdfAsync(NSUrl[] inputUrls, NSUrl outputUrl)
    {
        var recognitionMode = SBSDKOpticalCharacterRecognitionMode.ScanbotOCR;
        // This is the new OCR configuration with ML which doesn't require the languages.
        SBSDKOpticalCharacterRecognizerConfiguration ocrConfiguration =
                            SBSDKOpticalCharacterRecognizerConfiguration.ScanbotOCR;

        // to use legacy configuration we have to pass the installed languages.
        if (recognitionMode == SBSDKOpticalCharacterRecognitionMode.Tesseract)
        {
            var installedLanguages = SBSDKOCRLanguagesManager.InstalledLanguages;
            ocrConfiguration = SBSDKOpticalCharacterRecognizerConfiguration.TesseractWith(installedLanguages);
        }

        try
        {
            var opticalCharacterRecognizer = new SBSDKOpticalCharacterRecognizer(ocrConfiguration);
            var storage = CreateStorage(inputUrls, ScanbotUI.DefaultImageStoreEncrypter);
            opticalCharacterRecognizer.RecognizeOnImageStorage(storage,
            completion: (ocrResult, error) =>
            {
                // Create the PDF rendering options.
                var options = new SBSDKPDFRendererOptions();

                options.Dpi = 200;
                options.Resample = false;
                options.JpegQuality = 100;
                options.PageSize = SBSDKPDFRendererPageSize.A4;
                options.PageOrientation = SBSDKPDFRendererPageOrientation.Auto;
                options.PageFitMode = SBSDKPDFRendererPageFitMode.FitIn;
                options.PdfAttributes = new SBSDKPDFAttributes(
                                    author: "Your author",
                                    creator: "Your creator",
                                    title: "Your title",
                                    subject: "Your subject",
                                    keywords: ["PDF", "Scanbot", "SDK"]);

                // Create the PDF renderer and pass the PDF options to it.
                var renderer = new SBSDKPDFRenderer(options,
                                    ScanbotUI.DefaultImageStoreEncrypter);

                renderer.RenderImageStorageAsync(imageStorage: storage,
                                    indexSet: null, output: outputUrl,
                                    completionHandler: (isComplete, error) =>
                                    {
                                        storage.RemoveAllImages();
                                        if (error != null)
                                        {
                                            throw new NSErrorException(error);
                                        }

                                        OpenDocument(outputUrl, false);
                                    });

                if (ocrResult != null)
                {
                    OpenDocument(outputUrl, true, ocrResult.RecognizedText);
                }
                else
                {
                    ShowErrorAlert();
                }
            });
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Perform OCR", ex.Message);
        }
    }

    private void WriteTiff(NSUrl[] inputUrls, NSUrl outputUrl)
    {
        // Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
        var options = SBSDKTIFFImageWriterParameters.DefaultParametersForBinaryImages;
        options.Binarize = true;
        options.Compression = SBSDKTIFFImageWriterCompressionOptions.Ccitt_t4;
        options.Dpi = 250;

        var outputTiffUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".tiff");
        var tiffWriter = new SBSDKTIFFImageWriter(parameters: options);
        var success = tiffWriter.WriteTIFFFromToFile(inputUrls, 
                                                        ScanbotUI.DefaultImageStoreEncrypter,
                                                        outputTiffUrl);
        if (success)
        {
            var title = "Write TIFF";
            var body = "TIFF file saved to: " + outputTiffUrl;
            Alert.Show(this, title, body);
        }
        else
        {
            ShowErrorAlert();
        }
    }

    private SBSDKIndexedImageStorage CreateStorage(NSUrl[] uris, SBSDKStorageCrypting encrypter)
    {
        var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
        var tmp = NSUrl.FromFilename(string.Format("{0}/{1}", url.Scheme == "file" ? url.Path : url.AbsoluteString,
                            Guid.NewGuid()));
        var location = new SBSDKStorageLocation(tmp);
        var format = SBSDKImageFileFormat.Jpeg;

        return new SBSDKIndexedImageStorage(location, format, encrypter, uris);
    }
}