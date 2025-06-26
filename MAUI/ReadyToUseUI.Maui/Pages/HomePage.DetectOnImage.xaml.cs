using ReadyToUseUI.Maui.Pages.DetectOnImageResults;
using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using static ScanbotSDK.MAUI.ScanbotSDKMain;
using CheckScannerConfiguration = ScanbotSDK.MAUI.CheckScannerConfiguration;

namespace ReadyToUseUI.Maui.Pages;

public partial class HomePage
{
    private async Task MrzDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new MrzScannerConfiguration();
        var result = await Detector.Mrz.DetectMrzFromImageAsync(image, configuration: configuration);
        if (result == null)
        {
            ViewUtils.Alert(this, "MRZ Scanner result", "Could not detect MRZ data");
            return;
        }
        ViewUtils.Alert(this, "MRZ Scanner result", SdkUtils.GenericDocumentToString(result.Document));
    }

    private async Task EhicDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new EuropeanHealthInsuranceCardRecognizerConfiguration();
        var result = await Detector.Ehic.DetectEhicFromImageAsync(image, configuration: configuration);
        if (result == null || result.Status == EuropeanHealthInsuranceCardRecognitionResult.RecognitionStatus.FailedDetection)
        {
            var status = result?.Status ?? EuropeanHealthInsuranceCardRecognitionResult.RecognitionStatus.FailedDetection;
            var errorMessage = "Something went wrong, please try again.\nDetection status: " + status; 
            ViewUtils.Alert(this, "EHIC Scanner result", errorMessage);
            return;
        }
        
        var message = SdkUtils.ToAlertMessage(result);
        ViewUtils.Alert(this, "EHIC Scanner result", message);
    }

    private async Task DocumentDataExtractorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new DocumentDataExtractorConfiguration
        {
            // todo: confirm about the GenericDocument Detect on image types.
            // AcceptedDocumentTypes = new[]
            // {
            //         GenericDocumentRootType.DePassport,
            //         GenericDocumentRootType.DeDriverLicenseBack,
            //         GenericDocumentRootType.DeDriverLicenseFront,
            //         GenericDocumentRootType.DeIdCardBack,
            //         GenericDocumentRootType.DeIdCardFront,
            //         GenericDocumentRootType.DeResidencePermitBack,
            //         GenericDocumentRootType.DeResidencePermitFront,
            //     }
        };
        var result = await Detector.DocumentData.ExtractDocumentDataFromImageAsync(image, configuration);
        if (result == null)
        {
            ViewUtils.Alert(this, "GDR Result", "Could not detect GDR data");
            return;
        }

        var message = SdkUtils.GenericDocumentToString(result.Document);
        ViewUtils.Alert(this, "GDR Result", message);
    }

    private async Task CheckDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();
        var result = await Detector.Check.DetectCheckFromImageAsync(image, new CheckScannerConfiguration
        {
            AcceptedCheckStandards =
            [
                CheckStandard.Aus,
                CheckStandard.Can,
                CheckStandard.Fra,
                CheckStandard.Ind,
                CheckStandard.Isr,
                CheckStandard.Kwt,
                CheckStandard.Uae,
                CheckStandard.Usa
            ]
        });

        if (result == null || result.Status != CheckMagneticInkStripScanningStatus.Success)
        {
            ViewUtils.Alert(this, "Check Result", "Could not detect Check data");
            return;
        }

        var message = SdkUtils.ToAlertMessage(result);
        ViewUtils.Alert(this, "Check Result", message);
    }

    private async Task MedicalCertificateDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new MedicalCertificateScanningParameters();
        var result = await Detector.MedicalCertificate.DetectMedicalCertificateFromImageAsync(image, configuration);
        if (!result.ScanningSuccessful || result.DocumentDetectionResult.Status != DocumentDetectionStatus.Ok)
        {
            ViewUtils.Alert(this, "Medical Certificate Recognition Result", "Could not detect Medical Certificate data");
            return;
        }
        
        ViewUtils.Alert(this, $"Medical Certificate Recognition Result", result.ToFormattedString(),
            () =>
            {
                var resultPage = new DetectOnImageResultPage();
                var source = ImageSource.FromStream(() => result.CroppedImage.ToPlatformImage().AsStream());
                resultPage.NavigateData(source);
                Navigation.PushAsync(resultPage);
            });
    }

    private async Task CreditCardDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new CreditCardScannerConfiguration();
        var result = await Detector.CreditCard.DetectCreditCardFromImageAsync(image, configuration: configuration);
        if (result == null)
        {
            ViewUtils.Alert(this, "MRZ Scanner result", "Could not detect MRZ data");
            return;
        }
        ViewUtils.Alert(this, "MRZ Scanner result", SdkUtils.GenericDocumentToString(result.CreditCard));
    }
}