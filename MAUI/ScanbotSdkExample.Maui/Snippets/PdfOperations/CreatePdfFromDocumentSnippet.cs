using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Ocr;
using ScanbotSDK.MAUI.Core.PdfGeneration;

namespace ScanbotSdkExample.Maui.Snippets.PdfOperations;

public class CreatePdfFromDocumentSnippet
{
    public static async Task CreatePdfFromDocumentAsync(string documentUuid)
    {
        var result =
            await ScanbotSDKMain.PdfGenerator.GenerateFromDocumentAsync(documentUuid,
                pdfConfiguration: new PdfConfiguration { PageSize = PageSize.A4 },
                ocrConfiguration: new OcrConfiguration { OcrMode = OcrMode.ScanbotOcr });

        if (result.IsSuccess)
        {
            // Handle the result
            Console.WriteLine(result.Value.LocalPath);
        }
    }
}