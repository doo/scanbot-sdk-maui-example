using AndroidX.AppCompat.App;
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
		var document = _scanbotSdk.DocumentApi.LoadDocument("Your_Document_Id").Get<Document>();;
		
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
							pageSize:PageSize.A4, 
							pageDirection:PageDirection.Auto, 
							pageFit:PageFit.None, 
							dpi:200, 
							jpegQuality:100, 
							ResamplingMethod.None,
							ParametricFilter.ScanbotBinarizationFilter());

		// Render the images to a PDF file.
		// todo: Testing required
		var isPdfRendered = _scanbotSdk.CreatePdfGenerator(null).Generate(document, pdfConfig).GetValue<bool>();
		 if (isPdfRendered && document?.PdfUri != null)
		{
			// Do something with the PDF file
		}
	}
	
	private void CreatePdfFromImage(List<Android.Net.Uri> inputUris)
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
		var resultWrapper = _scanbotSdk.CreatePdfGenerator(null).Generate(inputUris.ToArray(), encryptionEnabled, pdfConfig);
		// todo: Testing required
		var pdfFile = resultWrapper.Get<Java.IO.File>();
		
		if (pdfFile != null && pdfFile.Exists())
		{
			// Do something with the PDF file
		}
	}
}