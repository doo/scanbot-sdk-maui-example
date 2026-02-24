using AndroidX.AppCompat.App;
using IO.Scanbot.Common;
using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Pdfgeneration;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Snippets;

public class PdfSnippet : AppCompatActivity
{
    private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Returns the singleton instance of the Sdk.
        _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
        var document = _scanbotSdk.DocumentApi.LoadDocument("Your_Document_Id").GetOrThrow<Document>();;
		
        if (_scanbotSdk.LicenseInfo.IsValid)
        {
            CreatePdfFromDocument(document);
        }
    }

    private void CreatePdfFromDocument(Document document)
    {
        // Create the PDF attributes.  
        var pdfAttributes = new PdfAttributes(
            author: "Your author",
            creator: "Your creator",
            title: "Your title",
            subject: "Your subject",
            keywords: "Your keywords");

        // Create the PDF rendering configurations.
        var pdfConfig = new PdfConfiguration(attributes: pdfAttributes,
            pageSize: PageSize.A4,
            pageDirection: PageDirection.Auto,
            pageFit: PageFit.None,
            dpi: 200,
            jpegQuality: 100,
            ResamplingMethod.None,
            ParametricFilter.ScanbotBinarizationFilter());

        // Render the images to a PDF file.
        var result = _scanbotSdk.CreatePdfGenerator(null).Generate(document, pdfConfig);
        if (result is IResult.Success && document?.PdfUri != null)
        {
            // Do something with the PDF file
        }
    }

    private void CreatePdfFromImage(List<Android.Net.Uri> inputUris)
    {
        var pdfFile = new Java.IO.File("Your path to pdf file.");
		
        // Create the PDF attributes.  
        var pdfAttributes = new PdfAttributes(
            author: "Your author",
            creator: "Your creator",
            title: "Your title",
            subject: "Your subject",
            keywords: "Your keywords");
		
        // Create the PDF rendering configurations.
        var pdfConfig = new PdfConfiguration(attributes: pdfAttributes, 
            pageSize:PageSize.A4, 
            pageDirection:PageDirection.Auto, 
            pageFit:PageFit.None, 
            dpi:200, 
            jpegQuality:100, 
            ResamplingMethod.None,
            ParametricFilter.ScanbotBinarizationFilter());

        // Notify the renderer that the images are encrypted with global sdk-encryption settings
        var encryptionEnabled = false;

        // Render the images to a PDF file.
        var result = _scanbotSdk.CreatePdfGenerator(null).Generate(inputUris.ToArray(), outputFile: pdfFile, encryptionEnabled, pdfConfig);
        if (result is IResult.Success && pdfFile.Exists())
        {
            // Do something with the PDF file
        }
    }
}