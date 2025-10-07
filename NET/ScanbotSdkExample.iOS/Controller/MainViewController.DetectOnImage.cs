using ScanbotSdkExample.iOS.Utils;
using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class MainViewController
{
	private async void RecognizeMrz()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

		var config = new SBSDKMRZScannerConfiguration
		{
			EnableDetection = true,
			IncompleteResultHandling = SBSDKMRZIncompleteResultHandling.Accept
		};
		
		var scanner = new SBSDKMRZScanner(config, out var initError);
		var result = scanner.RunWithImage(imageRef, out var error);
		if (result?.Document == null || !result.Success)
		{
			Alert.Show(this, "Error", "Unable to detect the MRZ.");
			return;
		}

		ShowPopup(this, result.Document?.ToFormattedString());
	}
	
	private async void RecognizeDocumentData()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());
		
		// todo: Check native SDKs.
		// var detectionTypes = SBSDKDocumentsModelRootType.AllDocumentTypes?.Cast<string>()?.ToArray();
		// var configuration = new SBSDKDocumentDataExtractorConfiguration();
		// configuration.SetAcceptedDocumentTypes(detectionTypes);
		// configuration.SetReturnCrops(true);
		
		var configuration = new SBSDKDocumentDataExtractorConfiguration();
		
		// todo: Check native SDKs.
		// configuration.Configurations =
		// [
		// 	new SBSDKDocumentDataExtractorCommonConfiguration
		// 	{
		// 		AcceptedDocumentTypes = detectionTypes
		// 	}
		// ];
		
		configuration.ReturnCrops = true;
		
		var extractor = new SBSDKDocumentDataExtractor(configuration, out var initError);
		var result = extractor.RunWithImage(imageRef, out var error);
		if (result?.Document == null)
		{
			Alert.Show(this, "Error", "Unable to extract the Document data.");
			return;
		}
	
		ShowPopup(this, result.Document?.ToFormattedString());
	}
	
	private async void RecognizeCheck()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());

		var config = new SBSDKCheckScannerConfiguration();
		config.DocumentDetectionMode = SBSDKCheckDocumentDetectionMode.DetectDocument;
		var scanner = new SBSDKCheckScanner(new SBSDKCheckScannerConfiguration(), out var initError);
		
		var result = scanner.RunWithImage(imageRef, out NSError error);
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
		var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());
		var configuration = new SBSDKCreditCardScannerConfiguration();
		configuration.ProcessingMode = SBSDKProcessingMode.SingleShot;
		
		var scanner = new SBSDKCreditCardScanner(configuration, out var initError);
		
		var result = scanner.RunWithImage(imageRef, out var runError);
		if (result?.CreditCard == null)
		{
			Alert.Show(this, "Error", "Unable to detect the Credit card.");
			return;
		}
		ShowPopup(this, result.CreditCard.ToFormattedString());
	}
	
	private async void RecognizeMedicalCertificate()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var imageRef = SBSDKImageRef.FromUIImageWithImage(image, new SBSDKRawImageLoadOptions());
		var scanner = SBSDKMedicalCertificateScanner.CreateAndReturnError(out var initError);
		var result = scanner.RunWithImage(imageRef, new SBSDKMedicalCertificateScanningParameters(), out var runError);
		if (result == null || !result.ScanningSuccessful)
		{
			Alert.Show(this, "Error", "Unable to detect the Medical certificate.");
			return;
		}
		
		ShowPopupWithAttributedText(this, result.ToFormattedAttributeString());
	}
}