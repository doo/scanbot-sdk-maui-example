using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.App;
using IO.Scanbot.Pdf.Model;
using IO.Scanbot.Sdk.Docprocessing;

namespace ReadyToUseUI.Droid.Snippets;

public class PdfSnippet : AppCompatActivity
{
	private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;

	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Returns the singleton instance of the Sdk.
		_scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
		var document = _scanbotSdk.DocumentApi.LoadDocument("Your_Document_Id");
		
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
		var pdfConfig = new IO.Scanbot.Pdf.Model.PdfConfig(pdfAttributes: pdfAttributes, 
							pageSize:PageSize.A4, 
							pageDirection:PageDirection.Auto, 
							pageFit:PageFit.None, 
							dpi:200, 
							jpegQuality:100, 
							ResamplingMethod.None);

		// Render the images to a PDF file.
		var isPdfRendered = _scanbotSdk.CreatePdfRenderer().Render(document, pdfConfig);
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
		var pdfConfig = new IO.Scanbot.Pdf.Model.PdfConfig(pdfAttributes: pdfAttributes, 
							pageSize:PageSize.A4, 
							pageDirection:PageDirection.Auto, 
							pageFit:PageFit.None, 
							dpi:200, 
							jpegQuality:100, 
							ResamplingMethod.None);

		// Notify the renderer that the images are encrypted with global sdk-encryption settings
		var encryptionEnabled = false;

		// Render the images to a PDF file.
		var pdfFile = _scanbotSdk.CreatePdfRenderer().Render(inputUris.ToArray(), encryptionEnabled, pdfConfig);
		if (pdfFile != null && pdfFile.Exists())
		{
			// Do something with the PDF file
		}
	}
}