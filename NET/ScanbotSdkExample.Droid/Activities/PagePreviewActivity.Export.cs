using Android.Content;
using AndroidX.Core.Content;
using IO.Scanbot.Sdk.Imageprocessing;
using IO.Scanbot.Sdk.Ocr;
using IO.Scanbot.Sdk.Ocr.Process;
using IO.Scanbot.Sdk.Pdfgeneration;
using IO.Scanbot.Sdk.Tiffgeneration;
using IO.Scanbot.Sdk.Util.Thread;
using ScanbotSDK.Droid.Helpers;
using ScanbotSdkExample.Droid.Model;
using ScanbotSdkExample.Droid.Utils;
using AndroidUri = Android.Net.Uri;

namespace ScanbotSdkExample.Droid.Activities;

public partial class PagePreviewActivity
{
        public void SaveTiff() => SaveDocument(SaveType.Tiff);

        public void SaveWithOcr() => SaveDocument(SaveType.Ocr);
        
        public void SaveSandwichPdf() => SaveDocument(SaveType.SandwichPdf);
        
        public void SavePdf() => SaveDocument(SaveType.Pdf);

        void SaveDocument(SaveType type)
        {
            if (!_scanbotSdk.LicenseInfo.IsValid)
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

		var pdfConfig = new PdfConfiguration(
						attributes: pdfAttributes,
						pageSize: PageSize.A4,
						pageDirection: PageDirection.Auto,
						pageFit: PageFit.FitIn,
						dpi: 72,
						jpegQuality: 80,
						ResamplingMethod.None,
						null); // ParametricFilter.ScanbotBinarizationFilter()
		
		// todo: Testing required
		var isSuccess = _scanbotSdk.CreatePdfGenerator().GenerateFromDocument(_document, new Java.IO.File(output.Path), pdfConfig: pdfConfig).GetValue<bool>();
		return output;
	}

	private AndroidUri CreateSandwichPdf()
	{
		var output = GetOutputUri(".pdf");
		
		// This is the new OCR configuration with ML which doesn't require the languages.
		var recognitionMode =  IOcrEngineManager.EngineMode.ScanbotOcr;
		IOcrEngineManager.OcrConfig ocrConfig = new IOcrEngineManager.OcrConfig(recognitionMode);
		
		// to use legacy configuration we have to pass the installed languages.
		if (recognitionMode == IOcrEngineManager.EngineMode.Tesseract)
		{
			var languages = _scanbotSdk.CreateOcrEngineManager().InstalledLanguages;
			if (languages.Count == 0)
			{
				RunOnUiThread(delegate { Alert.Toast(this, "OCR languages blobs are not available"); });
				return null;
			}

			ocrConfig = new IOcrEngineManager.OcrConfig(recognitionMode, languages);
		}

		var pdfAttributes = new PdfAttributes(
							author: "Your author",
							creator: "Your creator",
							title: "Your title",
							subject: "Your subject",
							keywords: "Your keywords");

		var pdfConfig = new PdfConfiguration(attributes: pdfAttributes,
							pageSize: PageSize.A4,
							pageDirection: PageDirection.Auto,
							pageFit: PageFit.FitIn,
							dpi: 72,
							jpegQuality: 80,
							ResamplingMethod.None,
							binarizationFilter: null);

		// todo: Testing required
		var pdfGenerated = _scanbotSdk.CreatePdfGenerator().GenerateWithOcrFromDocument(_document, pdfConfig, ocrConfig).GetValue<bool>();
		if (pdfGenerated)
		{
			if (string.IsNullOrEmpty(_document.PdfUri?.Path))
				return output;
			
			File.Move(_document.PdfUri.Path, new Java.IO.File(output.Path!).AbsolutePath);
		}

		return output;
	}

	private void PerformOcr()
	{
		// This is the new OCR configuration with ML which doesn't require the languages.
		var recognitionMode = IOcrEngineManager.EngineMode.ScanbotOcr;
		var recognizer = _scanbotSdk.CreateOcrEngineManager();

		// to use legacy configuration we have to pass the installed languages.
		if (recognitionMode == IOcrEngineManager.EngineMode.Tesseract)
		{
			var languages = recognizer.InstalledLanguages;
			if (languages.Count == 0)
			{
				RunOnUiThread(delegate { Alert.Toast(this, "OCR languages blobs are not available"); });
				return;
			}

			var ocrConfig = new IOcrEngineManager.OcrConfig(recognitionMode, languages);
			recognizer.SetOcrConfig(ocrConfig);
		}
		else
		{
			recognizer.SetOcrConfig(new IOcrEngineManager.OcrConfig(recognitionMode));
		}

		var ocrResult = recognizer.RecognizeFromDocument(_document).Get<OcrResult>();
		RunOnUiThread(delegate
	      {
	          Alert.Show(this, "Ocr Result", ocrResult?.RecognizedText);
	      });
	}

	private AndroidUri CreateTiff()
	{
		var output = GetOutputUri(".tiff");
		var defaultParams = TiffGeneratorParameters.Default();

		// Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
		var options = new TiffGeneratorParameters(
			CompressionMode.None,
			jpegQuality: defaultParams.JpegQuality,
			zipCompressionLevel: defaultParams.ZipCompressionLevel,
			dpi: 200,
			userFields: Array.Empty<UserField>(),
			ParametricFilter.ScanbotBinarizationFilter());
		var isTiffGenerated = _scanbotSdk.CreateTiffGeneratorManager().GenerateFromDocument(_document, new Java.IO.File(output.Path), options).GetValue<bool>();
		return output;
	}
}