using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
	private async void DetectMrz()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
        
		// Create an instance of the recognizer
		var recognizer = new SBSDKMRZScanner();
		var result = recognizer.ScanFromImage(image);
		if (result == null || !result.Success || result.Document == null)
			return;

		ShowPopup(this, FormattedString(result.Document));
	}

	private async void DetectEhic()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		
		var recognizer = new SBSDKHealthInsuranceCardRecognizer();
		var result = recognizer.RecognizeFromImage(image, false);
		if (result == null || result.Status != SBSDKEuropeanHealthInsuranceCardRecognitionResultRecognitionStatus.Success)
			return;
		
		ShowPopup(this, result.ToJsonWithConfiguration(new SBSDKToJSONConfiguration()));
	}
	
	private async void DetectCheck()
	{
		var image = await ImagePicker.Instance.PickImageAsync();

		var recognizer = new SBSDKCheckScanner();
		recognizer.AcceptedCheckTypes = SBSDKCheckDocumentModelRootType.AllDocumentTypes;

		var result = recognizer.ScanFromImage(image, false);
		if (result?.DocumentDetectionResult == null || !result.DocumentDetectionResult.IsScanningStatusOK)
			return;
	
		ShowPopup(this, FormattedString(result.Check));
	}

	private async void DetectMedicalCertificate()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var recognizer = new SBSDKMedicalCertificateScanner();
		var result = recognizer.ScanFromImage(image, new SBSDKMedicalCertificateScanningParameters());
		if (result == null || !result.ScanningSuccessful || result.DocumentDetectionResult == null)
			return;
		
		ShowPopup(this, result.DocumentDetectionResult.ToJsonWithConfiguration(new SBSDKToJSONConfiguration()));
	}

	private string FormattedString(SBSDKGenericDocument document)
	{
		var formattedString = string.Empty;
		foreach (var field in document.Fields)
		{
			if (string.IsNullOrEmpty(field?.Type?.Name))
				continue;
			formattedString += $"{field.Type.Name}: {field.Value?.Text ?? "-"}\n";
		}

		return formattedString;
	}
}