using ScanbotSdkExample.iOS.Utils;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class MainViewController
{
	private async void RecognizeMrz()
	{
		var image = await ImagePicker.Instance.PickImageAsync();

		var config = new SBSDKMRZScannerConfiguration
		{
			EnableDetection = true,
			IncompleteResultHandling = SBSDKMRZIncompleteResultHandling.Accept
		};
		
		var scanner = new SBSDKMRZScanner(config);
		var result = scanner.ScanFromImage(image);
		if (result?.Document == null)
		{
			Alert.Show(this, "Error", "Unable to detect the MRZ.");
			return;
		}

		ShowPopup(this, result.Document?.ToFormattedString());
	}

	private async void RecognizeEhic()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		
		var recognizer = new SBSDKHealthInsuranceCardRecognizer();
		var result = recognizer.RecognizeFromImage(image, false);
		if (result?.Fields == null || result.Status != SBSDKEuropeanHealthInsuranceCardRecognitionResultRecognitionStatus.Success)
		{
			Alert.Show(this, "Error", "Unable to detect the EHIC.");
			return;
		}

		ShowPopup(this, result.Fields.ToFormattedString());
	}
	
	private async void RecognizeDocumentData()
	{
		var image = await ImagePicker.Instance.PickImageAsync();

		var detectionTypes = SBSDKDocumentsModelRootType.AllDocumentTypes;
		var builder = new SBSDKDocumentDataExtractorConfigurationBuilder();
		builder.SetAcceptedDocumentTypes(detectionTypes);
		builder.SetReturnCrops(true);

		var extractor = new SBSDKDocumentDataExtractor(builder.BuildConfiguration);

		var result = extractor.ExtractFromImage(image, false);
		if (result?.Document == null || !result.DocumentDetectionResult.IsScanningStatusOK)
		{
			Alert.Show(this, "Error", "Unable to extract the Document data.");
			return;
		}
	
		ShowPopup(this, result.Document?.ToFormattedString());
	}
	
	private async void RecognizeCheck()
	{
		var image = await ImagePicker.Instance.PickImageAsync();

		var config = new SBSDKCheckScannerConfiguration();
		config.DocumentDetectionMode = SBSDKCheckDocumentDetectionMode.DetectDocument;
		
		var scanner = new SBSDKCheckScanner(config, SBSDKCheckDocumentModelRootType.AllDocumentTypes);
		
		var result = scanner.ScanFromImage(image, false);
		if (result?.Check == null)
		{
			Alert.Show(this, "Error", "Unable to detect the Check.");
			return;
		}
	
		ShowPopup(this, result.Check?.ToFormattedString());
	}

	private async void RecognizeCreditCard()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		
		var configuration = new SBSDKCreditCardScannerConfiguration();
		configuration.ScanningMode = SBSDKCreditCardScanningMode.SingleShot;
		
		var scanner = new SBSDKCreditCardScanner(configuration);
		var result = scanner.ScanFromImage(image);
		if (result?.CreditCard == null || result.ScanningStatus != SBSDKCreditCardScanningStatus.Success)
		{
			Alert.Show(this, "Error", "Unable to detect the Credit card.");
			return;
		}
		ShowPopup(this, result.CreditCard.ToFormattedString());
	}
	
	private async void RecognizeMedicalCertificate()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var scanner = new SBSDKMedicalCertificateScanner();
		var result = scanner.ScanFromImage(image, new SBSDKMedicalCertificateScanningParameters());
		if (result == null || !result.ScanningSuccessful || result.DocumentDetectionResult == null)
		{
			Alert.Show(this, "Error", "Unable to detect the Medical certificate.");
			return;
		}
		
		ShowPopupWithAttributedText(this, result.ToFormattedAttributeString());
	}
}