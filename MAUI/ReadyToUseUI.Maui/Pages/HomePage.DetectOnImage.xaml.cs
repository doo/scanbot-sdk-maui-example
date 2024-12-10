using ReadyToUseUI.Maui.Pages.DetectOnImageResults;
using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.EHIC;
using ScanbotSDK.MAUI.GenericDocument;
using ScanbotSDK.MAUI.MedicalCertificate;
using static ScanbotSDK.MAUI.ScanbotSDKMain;
namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage
{
    private async Task MRZDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var result = await Detectors.Mrz.DetectMrzFromImage(image);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ParseMRZResult(result);
            ViewUtils.Alert(this, "MRZ Scanner result", message);
        }
        else
        {
            ViewUtils.Alert(this, "MRZ Scanner result", "Could not detect MRZ data");
        }
    }

    private async Task EHICDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var result = await Detectors.Ehic.DetectEHICFromImage(image);
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
        var image = await ImagePicker.PickImageAsync();

        var configuration = new GenericDocumentDetectorConfiguration
        {
            AcceptedDocumentTypes = new[]
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
        var result = await Detectors.GenericDocument.DetectGenericDocumentFromImage(image, configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SDKUtils.ToAlertMessage(result);
            ViewUtils.Alert(this, "GDR Result", message);
        }
        else
        {
            ViewUtils.Alert(this, "GDR Result", "Could not detect GDR data");
        }
    }

    private async Task CheckDetectorrClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var result = await Detectors.Check.DetectCheckFromImage(image, new List<CheckStandard>
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
        else
        {
            ViewUtils.Alert(this, "Check Result", "Could not detect Check data");
        }
    }

    private async Task MedicalCertificateDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new MedicalCertificateDetectorConfiguration
        {
            DetectDocument = true,
            RecognizeBarcode = true,
            RecognizePatientInfo = true,
            ReturnCroppedDocumentImage = true
        };

        var result = await Detectors.MedicalCertificate.DetectMedicalCertificateFromImage(image, configuration);
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
        else
        {
            ViewUtils.Alert(this, "Medical Certificate Recognition Result", "Could not detect Medical Certificate data");
        }
    }
}