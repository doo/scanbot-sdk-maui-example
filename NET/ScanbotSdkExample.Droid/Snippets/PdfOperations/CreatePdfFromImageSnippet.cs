using IO.Scanbot.Common;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Ocr;
using IO.Scanbot.Sdk.Pdfgeneration;

namespace ScanbotSdkExample.Droid.Snippets.PdfOperations;

public static class CreatePdfFromImageSnippet
{
    public static void CreatePdfFromImage(IO.Scanbot.Sdk.ScanbotSDK sdk, List<Android.Net.Uri> imageFileUris)
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
            .Generate(imageFileUris, true, pdfConfig: pdfConfig);

        // Check if the file was generated.
        if (result is IResult.Success)
        {
            // Handle the result
        }
    }
}