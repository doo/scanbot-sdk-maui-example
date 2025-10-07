using ScanbotSdkExample.iOS.Models;
using ScanbotSdkExample.iOS.Utils;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class ScannedDocumentsViewController
{
    private void OnExportButtonClick(object sender, EventArgs e)
    {
        var docs = NSSearchPathDirectory.DocumentDirectory;
        var url = NSFileManager.DefaultManager.GetUrls(docs, NSSearchPathDomain.User)[0];
        var alertController = UIAlertController.Create(Texts.Save, Texts.SaveHow, UIAlertControllerStyle.ActionSheet);
        if (!ScanbotSDKGlobal.IsLicenseValid)
        {
            var title = "Oops";
            var body = "Your license has expired";
            Alert.Show(this, title, body);
            return;
        }

        var pdf = CreateButton(Texts.SavePdf, (_) =>  CreatePdfAsync(_scannedDocument, url));
        var sandwichPdf = CreateButton(Texts.SaveSandwichPdf, (_) =>  CreateSandwichedPdf(_scannedDocument, url));
        var ocr = CreateButton(Texts.PerformOcr, (_) =>  PerformOcr(_scannedDocument));
        var tiff = CreateButton(Texts.Tiff, (_) => WriteTiff(_scannedDocument, url));
        var cancel = CreateButton(Texts.CancelDialogButton, delegate { }, UIAlertActionStyle.Cancel);

        alertController.AddAction(pdf);
        alertController.AddAction(sandwichPdf);
        alertController.AddAction(ocr);
        alertController.AddAction(tiff);
        alertController.AddAction(cancel);

        UIPopoverPresentationController presentationPopover = alertController.PopoverPresentationController;
        if (presentationPopover != null)
        {
            presentationPopover.SourceView = View!;
            presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
        }

        PresentViewController(alertController, true, null);
    }

    private void PerformOcr(SBSDKScannedDocument scannedDocument)
    {
        var recognitionMode = SBSDKOCREngineMode.ScanbotOCR;
        
        // This is the new OCR configuration with ML which doesn't require the languages.
        var ocrConfiguration = SBSDKOCREngineConfiguration.ScanbotOCR;

        // to use legacy configuration we have to pass the installed languages.
        if (recognitionMode == SBSDKOCREngineMode.Tesseract)
        {
            var installedLanguages = SBSDKOCRLanguagesManager.InstalledLanguages;
            ocrConfiguration = SBSDKOCREngineConfiguration.TesseractWith(installedLanguages);
        }

        try
        {
            var ocrEngine = SBSDKOCREngine.CreateAndReturnError(out var initError);
            // todo: Check native SDKs -- How to handle Document object for OCRs
            // ocrEngine.RunWithImage().RecognizeFromScannedDocument(scannedDocument, completion: (ocrResult, error) =>
            // {
            //     if (error != null)
            //     {
            //         Alert.Show(this, "Perform OCR", error.LocalizedDescription);
            //         return;
            //     }
            //
            //     Alert.Show(this, "Perform OCR", ocrResult.RecognizedText);
            // });
        }
        catch (Exception exception)
        {
            Alert.Show(this, "Perform OCR", exception.Message);
        }
    }

    private void CreatePdfAsync(SBSDKScannedDocument document, NSUrl outputUrl)
    {
        try
        {
            // Set the name and path for the pdf file
            var outputPdfUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".pdf");
            
            // Create the PDF rendering options object with default options.
            var configuration = new SBSDKPDFConfiguration();
            
            configuration.Dpi = 200;
            configuration.ResamplingMethod = SBSDKResamplingMethod.Linear;
            configuration.JpegQuality = 100;
            configuration.PageSize = SBSDKPageSize.A4;
            configuration.PageDirection = SBSDKPageDirection.Auto;
            configuration.PageFit = SBSDKPageFit.FitIn;
            configuration.Attributes =  new SBSDKPDFAttributes(
                                author: "Your author",
                                creator: "Your creator",
                                title: "Your title",
                                subject: "Your subject",
                                keywords: "PDF, ScanbotSDK");

             // Renders the document into a searchable PDF at the specified file url
             var generator = new SBSDKPDFGenerator(configuration: configuration, ocrConfiguration: null, ScanbotSDKGlobal.DefaultImageStoreEncrypter?.StorageCrypting);
             
             // Start the rendering operation and store the SBSDKProgress to watch the progress or cancel the operation.
             generator.GenerateFromScannedDocument(document, output: outputPdfUrl, 
                completion: (isComplete, error) =>
                {
                    if (error != null)
                    {
                        Alert.Show(this, "Create PDF", error.LocalizedDescription);
                    }
                    
                    OpenDocument(outputPdfUrl, false);
                });
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Create PDF", ex.Message);
        }
    }
    
    private void CreateSandwichedPdf(SBSDKScannedDocument document, NSUrl outputUrl)
    {
        var recognitionMode = SBSDKOCREngineMode.ScanbotOCR;
        
        // This is the new OCR configuration with ML which doesn't require the languages.
        // Create and set the OCR configuration for HOCR.
        var ocrConfiguration = SBSDKOCREngineConfiguration.ScanbotOCR;

        // to use legacy configuration we have to pass the installed languages.
        if (recognitionMode == SBSDKOCREngineMode.Tesseract)
        {
            var installedLanguages = SBSDKOCRLanguagesManager.InstalledLanguages;
            ocrConfiguration = SBSDKOCREngineConfiguration.TesseractWith(installedLanguages);
        }

        try 
        {
            var outputPdfUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".pdf");
            
            // Create the PDF rendering options object with default options.
            var configuration = new SBSDKPDFConfiguration();
            
            configuration.Dpi = 200;
            configuration.ResamplingMethod = SBSDKResamplingMethod.Linear;
            configuration.JpegQuality = 100;
            configuration.PageSize = SBSDKPageSize.A4;
            configuration.PageDirection = SBSDKPageDirection.Auto;
            configuration.PageFit = SBSDKPageFit.FitIn;
            configuration.Attributes =  new SBSDKPDFAttributes(
                author: "Your author",
                creator: "Your creator",
                title: "Your title",
                subject: "Your subject",
                keywords: "PDF, ScanbotSDK");

            // Renders the document into a searchable PDF at the specified file url
            var generator = new SBSDKPDFGenerator(configuration: configuration, ocrConfiguration: ocrConfiguration, ScanbotSDKGlobal.DefaultImageStoreEncrypter?.StorageCrypting);
 
            // Start the rendering operation and store the SBSDKProgress to watch the progress or cancel the operation.
            generator.GenerateFromScannedDocument(document, output: outputPdfUrl, 
                completion: (isComplete, error) =>
                {
                    if (error != null)
                    {
                        Alert.Show(this, "Sandwiched PDF", error.LocalizedDescription);
                    }
                    
                    OpenDocument(outputPdfUrl, false);
                });
        }
        catch (Exception ex)
        {
            Alert.Show(this, "Sandwiched PDF", ex.Message);
        }
    }

    private void WriteTiff(SBSDKScannedDocument scannedDocument, NSUrl outputUrl)
    {
        // Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
        var options = new SBSDKTIFFGeneratorParameters();
        options.BinarizationFilter = new SBSDKScanbotBinarizationFilter(SBSDKOutputMode.Binary);
        options.Compression = SBSDKCompressionMode.CcittT4;
        options.Dpi = 300;

        var outputTiffUrl = new NSUrl(outputUrl.AbsoluteString + Guid.NewGuid() + ".tiff");
        var tiffWriter = new SBSDKTIFFGenerator(parameters: options);
        var success = tiffWriter.GenerateFromScannedDocumentToFile(scannedDocument, outputTiffUrl);
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
}