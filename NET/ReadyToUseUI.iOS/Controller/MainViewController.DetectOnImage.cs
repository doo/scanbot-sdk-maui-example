using ReadyToUseUI.iOS.Utils;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
	private async void DetectMrz()
	{
		var image = await ImagePicker.Instance.PickImageAsync();

		var config = new SBSDKMRZScannerConfiguration
		{
			EnableDetection = true,
			IncompleteResultHandling = SBSDKMRZIncompleteResultHandling.Accept
		};
		
		// Create an instance of the recognizer
		var recognizer = new SBSDKMRZScanner(config);
		var result = recognizer.ScanFromImage(image);
		if (result?.Document == null)
		{
			Alert.Show(this, "Error", "Unable to detect the document.");
			return;
		}

		ShowPopup(this, result.Document?.ToFormattedString());
	}

	private async void DetectEhic()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		
		var recognizer = new SBSDKHealthInsuranceCardRecognizer();
		var result = recognizer.RecognizeFromImage(image, false);
		if (result == null || result.Status != SBSDKEuropeanHealthInsuranceCardRecognitionResultRecognitionStatus.Success)
		{
			Alert.Show(this, "Error", "Unable to detect the document.");
			return;
		}

		ShowPopup(this, result.Fields?.ToFormattedString());
	}
	
	private async void DetectDocumentData()
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
			Alert.Show(this, "Error", "Unable to detect the document.");
			return;
		}
	
		ShowPopup(this, result.Document?.ToFormattedString());
	}
	
	private async void DetectCheck()
	{
		var image = await ImagePicker.Instance.PickImageAsync();

		var config = new SBSDKCheckScannerConfiguration();
		config.DocumentDetectionMode = SBSDKCheckDocumentDetectionMode.DetectDocument;
		var recognizer = new SBSDKCheckScanner(config, SBSDKCheckDocumentModelRootType.AllDocumentTypes);

		var result = recognizer.ScanFromImage(image, false);
		if (result?.Check == null)
		{
			Alert.Show(this, "Error", "Unable to detect the document.");
			return;
		}
	
		ShowPopup(this, result.Check?.ToFormattedString());
	}

	private async void DetectCreditCard()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var recognizer = new SBSDKCreditCardScanner();
		var result = recognizer.ScanFromImage(image);
		if (result?.CreditCard == null || result.ScanningStatus != SBSDKCreditCardScanningStatus.Success)
		{
			Alert.Show(this, "Error", "Unable to detect the document.");
			return;
		}
		ShowPopup(this, result.CreditCard?.ToFormattedString());
	}
	
	private async void DetectMedicalCertificate()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var scanner = new SBSDKMedicalCertificateScanner();
		var result = scanner.ScanFromImage(image, new SBSDKMedicalCertificateScanningParameters());
		if (result == null || !result.ScanningSuccessful || result.DocumentDetectionResult == null)
		{
			Alert.Show(this, "Error", "Unable to detect the document.");
			return;
		}
		
		ShowPopupWithAttributedText(this, result?.ToFormattedAttributeString());
	}
}