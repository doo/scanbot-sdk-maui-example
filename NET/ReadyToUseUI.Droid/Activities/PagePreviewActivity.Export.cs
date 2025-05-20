using Android.Content;
using AndroidX.Core.Content;
using Com.Googlecode.Tesseract.Android;
using IO.Scanbot.Pdf.Model;
using IO.Scanbot.Sdk.Imagefilters;
using IO.Scanbot.Sdk.Ocr;
using IO.Scanbot.Sdk.Ocr.Intelligence;
using IO.Scanbot.Sdk.Process;
using IO.Scanbot.Sdk.Tiff.Model;
using IO.Scanbot.Sdk.Util.Thread;
using ReadyToUseUI.Droid.Model;
using ReadyToUseUI.Droid.Utils;
using AndroidUri = Android.Net.Uri;

namespace ReadyToUseUI.Droid.Activities;

public partial class PagePreviewActivity
{
        public void SaveTiff() => SaveDocument(SaveType.Tiff);

        public void SaveWithOcr() => SaveDocument(SaveType.Ocr);
        
        public void SaveSandwichPdf() => SaveDocument(SaveType.SandwichPdf);
        
        public void SavePdf() => SaveDocument(SaveType.Pdf);

        void SaveDocument(SaveType type)
        {
            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                Alert.ShowLicenseDialog(this);
                return;
            }

            Task.Run(delegate
            {
                 Android.Net.Uri output = null;

                 switch (type)
                 {
                     case SaveType.Pdf:
                         output = CreatePdf();
                         break;

                     case SaveType.SandwichPdf:
                         output = CreateSandwichPdf();
                         break;

                     case SaveType.Ocr:
                         PerformOcr();
                         break;

                     case SaveType.Tiff:
                         output = CreateTiff();
                         break;
                 }

                 if (output == null)
                     return;

                 Java.IO.File file = Copier.Copy(this, output);

                 var intent = new Intent(Intent.ActionView, output);

                 var authority = ApplicationContext.PackageName + ".provider";
                 var uri = FileProvider.GetUriForFile(this, authority, file);

                 intent.SetDataAndType(uri, MimeUtils.GetMimeByName(file.Name));
                 intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
                 intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);

                 RunOnUiThread(delegate
                 {
                   StartActivity(Intent.CreateChooser(intent, output.LastPathSegment));
                   Alert.Toast(this, "File saved to: " + output.Path);
                 });
            });
        }
        
	private AndroidUri CreatePdf()
	{
		var output = GetOutputUri(".pdf");
		var pdfAttributes = new PdfAttributes(
							author: "Your author",
							creator: "Your creator",
							title: "Your title",
							subject: "Your subject",
							keywords: "Your keywords");

		var pdfConfig = new IO.Scanbot.Pdf.Model.PdfConfiguration(
						attributes: pdfAttributes,
						pageSize: PageSize.A4,
						pageDirection: PageDirection.Auto,
						pageFit: PageFit.FitIn,
						dpi: 72,
						jpegQuality: 80,
						ResamplingMethod.None);

		scanbotSDK.CreatePdfGenerator().GenerateFromDocument(document, new Java.IO.File(output.Path), pdfConfig: pdfConfig);
		return output;
	}

	private AndroidUri CreateSandwichPdf()
	{
		var output = GetOutputUri(".pdf");
		
		// This is the new OCR configuration with ML which doesn't require the languages.
		var recognitionMode =  IOcrEngine.EngineMode.ScanbotOcr;
		IOcrEngine.OcrConfig ocrConfig = new IOcrEngine.OcrConfig(recognitionMode);
		
		// to use legacy configuration we have to pass the installed languages.
		if (recognitionMode == IOcrEngine.EngineMode.Tesseract)
		{
			var languages = scanbotSDK.CreateOcrEngine().InstalledLanguages;
			if (languages.Count == 0)
			{
				RunOnUiThread(delegate { Alert.Toast(this, "OCR languages blobs are not available"); });
				return null;
			}

			ocrConfig = new IOcrEngine.OcrConfig(recognitionMode, languages);
		}

		var pdfAttributes = new PdfAttributes(
							author: "Your author",
							creator: "Your creator",
							title: "Your title",
							subject: "Your subject",
							keywords: "Your keywords");

		var pdfConfig = new IO.Scanbot.Pdf.Model.PdfConfiguration(attributes: pdfAttributes,
							pageSize: PageSize.A4,
							pageDirection: PageDirection.Auto,
							pageFit: PageFit.FitIn,
							dpi: 72,
							jpegQuality: 80,
							ResamplingMethod.None);

		var pdfGenerated = scanbotSDK.CreatePdfGenerator().GenerateWithOcrFromDocument(document, pdfConfig, ocrConfig);
		if (pdfGenerated)
		{
			if (string.IsNullOrEmpty(document.PdfUri?.Path))
				return output;
			
			File.Move(document.PdfUri.Path, new Java.IO.File(output.Path!).AbsolutePath);
		}

		return output;
	}

	private void PerformOcr()
	{
		// This is the new OCR configuration with ML which doesn't require the languages.
		var recognitionMode = IOcrEngine.EngineMode.ScanbotOcr;
		var recognizer = scanbotSDK.CreateOcrEngine();

		// to use legacy configuration we have to pass the installed languages.
		if (recognitionMode == IOcrEngine.EngineMode.Tesseract)
		{
			var languages = recognizer.InstalledLanguages;
			if (languages.Count == 0)
			{
				RunOnUiThread(delegate { Alert.Toast(this, "OCR languages blobs are not available"); });
				return;
			}

			var ocrConfig = new IOcrEngine.OcrConfig(recognitionMode, languages);
			recognizer.SetOcrConfig(ocrConfig);
		}
		else
		{
			recognizer.SetOcrConfig(new IOcrEngine.OcrConfig(recognitionMode));
		}

		var ocrResult = recognizer.RecognizeFromDocument(document);
		RunOnUiThread(delegate
	      {
	          Alert.ShowAlert(this, "Ocr Result", ocrResult.RecognizedText);
	      });
	}

	private AndroidUri CreateTiff()
	{
		var output = GetOutputUri(".tiff");
		var defaultParams = TiffGeneratorParameters.Default();
		
		// Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
		var options = new IO.Scanbot.Sdk.Tiff.Model.TiffGeneratorParameters(
			IO.Scanbot.Sdk.Tiff.Model.CompressionMode.None,
			jpegQuality: defaultParams.JpegQuality,
			zipCompressionLevel: defaultParams.ZipCompressionLevel,
			dpi: 200,
			userFields: Array.Empty<UserField>(),
			ParametricFilter.ScanbotBinarizationFilter());

		scanbotSDK.CreateTiffGenerator().GenerateFromDocument(document, new Java.IO.File(output.Path), options);
		return output;
	}
}