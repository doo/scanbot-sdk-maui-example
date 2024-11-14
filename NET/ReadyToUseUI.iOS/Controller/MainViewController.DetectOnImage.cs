using Scanbot.ImagePicker.iOS;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
	private async void DetectMrz()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
        
		// Create an instance of the recognizezr
		var recognizer = new SBSDKMachineReadableZoneRecognizer();
		var result = recognizer.RecognizePersonalIdentityFromImage(image);
		if (result != null && !string.IsNullOrEmpty(result.StringRepresentation))
		{
			ShowPopup(this, result.StringRepresentation);
		}
	}

	private async void DetectEhic()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		
		var recognizer = new SBSDKHealthInsuranceCardRecognizer();
		var result = recognizer.RecognizeOnStillImage(image);
		if (result != null && !string.IsNullOrEmpty(result.StringRepresentation))
		{
			ShowPopup(this, result.StringRepresentation);
		}
	}

	private async void DetectGenericDocument()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var recognizer = new SBSDKGenericDocumentRecognizer(SBSDKGenericDocumentRootType.AllDocumentTypes);

		var result = recognizer.RecognizeDocumentOnImage(image);

		if (result?.Document == null)
		{
			return;
		}
		
		// We only take the first document for simplicity
		var fields = result.Document.Fields
							.Where(f => !string.IsNullOrEmpty(f?.Type?.Name) && !string.IsNullOrEmpty(f?.Value?.Text))
							.Select(f => $"{f.Type.Name}: {f.Value.Text}")
							.ToList();
		var description = string.Join("\n", fields);
		ShowPopup(this, description);
	}

	private async void DetectCheck()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		
		var recognizer = new SBSDKCheckRecognizer();
		recognizer.AcceptedCheckTypes = new SBSDKCheckDocumentRootType[]
		{
							SBSDKCheckDocumentRootType.AusCheck,
							SBSDKCheckDocumentRootType.FraCheck,
							SBSDKCheckDocumentRootType.IndCheck,
							SBSDKCheckDocumentRootType.KwtCheck,
							SBSDKCheckDocumentRootType.UsaCheck,
							SBSDKCheckDocumentRootType.UaeCheck,
							SBSDKCheckDocumentRootType.CanCheck,
							SBSDKCheckDocumentRootType.IsrCheck,
		};

		var result = recognizer.RecognizeOnImage(image);
		if (result != null && !string.IsNullOrEmpty(result.StringRepresentation))
		{
			ShowPopup(this, result.StringRepresentation);
		}
	}

	private async void DetectMedicalCertificate()
	{
		var image = await ImagePicker.Instance.PickImageAsync();
		var recognizer = new SBSDKMedicalCertificateRecognizer();
		var result = recognizer.RecognizeFromImage(image, true);
		if (result != null && !string.IsNullOrEmpty(result.StringRepresentation))
		{
			ShowPopup(this, result.StringRepresentation);
		}
	}
}