using Android.Content;
using AndroidX.Core.Content;
using IO.Scanbot.Pdf.Model;
using IO.Scanbot.Sdk.Imagefilters;
using IO.Scanbot.Sdk.Ocr;
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

		var pdfConfig = new IO.Scanbot.Pdf.Model.PdfConfig(pdfAttributes: pdfAttributes,
							pageSize: PageSize.A4, pageDirection: PageDirection.Auto, pageFit: PageFit.FitIn,
							dpi: 72, jpegQuality: 80, ResamplingMethod.None);

		scanbotSDK.CreatePdfRenderer()
							.Render(document, new Java.IO.File(output.Path), pdfConfig: pdfConfig);
		return output;
	}

	private AndroidUri CreateSandwichPdf()
	{
		var output = GetOutputUri(".pdf");
		// This is the new OCR configuration with ML which doesn't require the languages.
		var recognitionMode = IOpticalCharacterRecognizer.EngineMode.ScanbotOcr;
		var recognizer = scanbotSDK.CreateOcrRecognizer();

		// to use legacy configuration we have to pass the installed languages.
		if (recognitionMode == IOpticalCharacterRecognizer.EngineMode.Tesseract)
		{
			var languages = recognizer.InstalledLanguages;
			if (languages.Count == 0)
			{
				RunOnUiThread(delegate { Alert.Toast(this, "OCR languages blobs are not available"); });
				return null;
			}

			var ocrConfig = new IOpticalCharacterRecognizer.OcrConfig(recognitionMode, languages);
			recognizer.SetOcrConfig(ocrConfig);
		}
		else
		{
			recognizer.SetOcrConfig(new IOpticalCharacterRecognizer.OcrConfig(recognitionMode));
		}

		var pdfAttributes = new PdfAttributes(
							author: "Your author",
							creator: "Your creator",
							title: "Your title",
							subject: "Your subject",
							keywords: "Your keywords");

		var pdfConfig = new IO.Scanbot.Pdf.Model.PdfConfig(pdfAttributes: pdfAttributes,
							pageSize: PageSize.A4, pageDirection: PageDirection.Auto, pageFit: PageFit.FitIn,
							dpi: 72, jpegQuality: 80, ResamplingMethod.None);

		var ocrResult = recognizer.RecognizeTextWithPdfFromDocument(document, pdfConfig);
		File.Move(ocrResult.SandwichedPdfDocumentFile.AbsolutePath, new Java.IO.File(output.Path).AbsolutePath);
		return output;
	}

	private void PerformOcr()
	{
		// This is the new OCR configuration with ML which doesn't require the languages.
		var recognitionMode = IOpticalCharacterRecognizer.EngineMode.ScanbotOcr;
		var recognizer = scanbotSDK.CreateOcrRecognizer();

		// to use legacy configuration we have to pass the installed languages.
		if (recognitionMode == IOpticalCharacterRecognizer.EngineMode.Tesseract)
		{
			var languages = recognizer.InstalledLanguages;
			if (languages.Count == 0)
			{
				RunOnUiThread(delegate { Alert.Toast(this, "OCR languages blobs are not available"); });
				return;
			}

			var ocrConfig = new IOpticalCharacterRecognizer.OcrConfig(recognitionMode, languages);
			recognizer.SetOcrConfig(ocrConfig);
		}
		else
		{
			recognizer.SetOcrConfig(new IOpticalCharacterRecognizer.OcrConfig(recognitionMode));
		}

		var ocrResult = recognizer.RecognizeTextWithPdfFromDocument(document, PdfConfig.DefaultConfig());
		RunOnUiThread(delegate
	      {
	          Alert.ShowAlert(this, "Ocr Result", ocrResult.RecognizedText);
	      });
	}

	private AndroidUri CreateTiff()
	{
		var output = GetOutputUri(".tiff");
		// Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
		var options = new IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterParameters(
							new ScanbotBinarizationFilter(),
							250,
							IO.Scanbot.Sdk.Tiff.Model.TIFFImageWriterCompressionOptions.CompressionCcittfax4,
							Array.Empty<TIFFImageWriterUserDefinedField>());

		scanbotSDK.CreateTiffWriter().WriteTIFF(document, new Java.IO.File(output.Path), options);
		return output;
	}
}