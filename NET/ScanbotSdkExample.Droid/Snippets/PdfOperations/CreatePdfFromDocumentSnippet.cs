using IO.Scanbot.Common;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Ocr;
using IO.Scanbot.Sdk.Pdfgeneration;

namespace ScanbotSdkExample.Droid.Snippets.PdfOperations;

public static class CreatePdfFromDocumentSnippet
{
    public static void CreatePdfFromDocument(IO.Scanbot.Sdk.ScanbotSDK sdk, Document document, string outputPath)
    {
        var pdfAttributes = new PdfAttributes(
            author: "Your author",
            creator: "Your creator",
            title: "Your title",
            subject: "Your subject",
            keywords: "Your keywords");

        var pdfConfig = new PdfConfiguration(
            attributes: pdfAttributes,
            pageSize: PageSize.A4,
            pageDirection: PageDirection.Auto,
            pageFit: PageFit.FitIn,
            dpi: 72,
            jpegQuality: 80,
            resamplingMethod: ResamplingMethod.None,
            binarizationFilter: ParametricFilter.ScanbotBinarizationFilter());

        var ocrConfig = new IOcrEngineManager.OcrConfig
            { EngineMode = IOcrEngineManager.EngineMode.ScanbotOcr };

        var result = sdk.CreatePdfGenerator(ocrConfig)
            .Generate(document, new Java.IO.File(outputPath), pdfConfig: pdfConfig);

        // Check if the file was generated.
        if (result is IResult.Success && File.Exists(outputPath))
        {
            // Handle the result
        }
    }
}