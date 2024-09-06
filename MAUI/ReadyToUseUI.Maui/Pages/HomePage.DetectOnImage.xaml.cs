using ReadyToUseUI.Maui.Pages.DetectOnImageResults;
using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.EHIC;
using ScanbotSDK.MAUI.GenericDocument;
using ScanbotSDK.MAUI.MedicalCertificate;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;
namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage
{
    private async Task MRZDetectorClicked()
    {
        var source = await SBSDK.PickerService.PickImageAsync();
        if (source == null) return;

        var result = await SBSDK.DataDetectionService.DetectMrzFromImage(source);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ParseMRZResult(result);
            ViewUtils.Alert(this, "MRZ Scanner result", message);
        }
    }

    private async Task EHICDetectorClicked()
    {
        var source = await SBSDK.PickerService.PickImageAsync();
        if (source == null) return;
        
        var result = await SBSDK.DataDetectionService.DetectEHICFromImage(source);
        if (result.Status == OperationResult.Ok)
        {
            if (result.DetectionStatus == HealthInsuranceCardDetectionStatus.Success)
            {
                var message = SDKUtils.ToAlertMessage(result);
                ViewUtils.Alert(this, "EHIC Scanner result", message);
            }
            else
            {
                var message = "Something went wrong, please try again.\nDetection status: " + result.DetectionStatus;
                ViewUtils.Alert(this, "EHIC Scanner result", message);
            }
        }
    }

    private async Task GenericDocumentDetectorClicked()
    {
        var source = await SBSDK.PickerService.PickImageAsync();
        if (source == null) return;
        
        var configuration = new GenericDocumentDetectorConfiguration
        {
            AcceptedDocumentTypes = new []
            {
                GenericDocumentRootType.DePassport,
                GenericDocumentRootType.DeDriverLicenseBack,
                GenericDocumentRootType.DeDriverLicenseFront,
                GenericDocumentRootType.DeIdCardBack,
                GenericDocumentRootType.DeIdCardFront,
                GenericDocumentRootType.DeResidencePermitBack,
                GenericDocumentRootType.DeResidencePermitFront,
            }
        };
        var result = await SBSDK.DataDetectionService.DetectGenericDocumentFromImage(source, configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ToAlertMessage(result);
            ViewUtils.Alert(this, "GDR Result", message);
        }
    }

    private async Task CheckDetectorrClicked()
    {
        var source = await SBSDK.PickerService.PickImageAsync();
        if (source == null) return;
        
        var result = await SBSDK.DataDetectionService.DetectCheckFromImage(source, new List<CheckStandard>
        {
            CheckStandard.AUS,
            CheckStandard.CAN,
            CheckStandard.FRA,
            CheckStandard.IND,
            CheckStandard.ISR,
            CheckStandard.KWT,
            CheckStandard.UAE,
            CheckStandard.USA
        });
        
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ToAlertMessage(result);
            ViewUtils.Alert(this, "Check Result", message);
        }
    }

    private async Task MedicalCertificateDetectorClicked()
    {
        var source = await SBSDK.PickerService.PickImageAsync();
        if (source == null) return;
        
        var configuration = new MedicalCertificateDetectorConfiguration
        {
            DetectDocument = true,
            RecognizeBarcode = true,
            RecognizePatientInfo = true,
            ReturnCroppedDocumentImage = true
        };

        var result = await SBSDK.DataDetectionService.DetectMedicalCertificateFromImage(source, configuration);
        if (result.Status == OperationResult.Ok)
        {
            ViewUtils.Alert(this, $"Medical Certificate Recognition Result",
                FormatMedicalCertificateRecognitionResult(result), 
                () =>
                {
                    var resultPage = new DetectOnImageResultPage();
                    resultPage.NavigateData(result.CroppedImageSource);
                    Navigation.PushAsync(resultPage);
                });
        }
    }
}