using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSdkExample.Maui.Pages.DetectOnImageResults;
using ScanbotSdkExample.Maui.Utils;
using static ScanbotSDK.MAUI.ScanbotSDKMain;
using CheckScannerConfiguration = ScanbotSDK.MAUI.CheckScannerConfiguration;

namespace ScanbotSdkExample.Maui.Pages;

public partial class HomePage
{
    private async Task MrzDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();
        if (image is null) return;

        var configuration = new MrzScannerConfiguration
        {
            IncompleteResultHandling = MrzIncompleteResultHandling.Reject
            // Configure other parameters as needed.
        };

        var result = await Detector.Mrz.DetectOnImageAsync(image, configuration: configuration);
        if (result?.Document == null || result?.Success == false)
        {
            ViewUtils.Alert(this, "Error", "Could not detect the MRZ data.");
            return;
        }
        ViewUtils.Alert(this, "MRZ result", SdkUtils.GenericDocumentToString(result.Document));
    }

    private async Task EhicDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();

        var configuration = new EuropeanHealthInsuranceCardRecognizerConfiguration
        {
            MaxExpirationYear = 2100
            // Configure other parameters as needed.
        };

        var result = await Detector.Ehic.DetectOnImageAsync(image, configuration: configuration);
        if (result == null || result.Status == EuropeanHealthInsuranceCardRecognitionResult.RecognitionStatus.FailedDetection)
        { 
            ViewUtils.Alert(this, "Error", "Could not detect the Ehic data.");
            return;
        }
        
        var message = SdkUtils.ToAlertMessage(result);
        ViewUtils.Alert(this, "EHIC Scanner result", message);
    }

    private async Task DocumentDataExtractorClicked()
    {
        var image = await ImagePicker.PickImageAsync();
        if (image is null) return;
        
        var configuration = new DocumentDataExtractorConfiguration
        {
            Configurations = 
            [
                new DocumentDataExtractorCommonConfiguration
                {
                    AcceptedDocumentTypes = 
                    [
                        DocumentsModelRootType.MRZ.DocumentType.Name,
                        DocumentsModelRootType.DeIdCardBack.DocumentType.Name,
                        DocumentsModelRootType.DeIdCardFront.DocumentType.Name,
                        DocumentsModelRootType.DePassport.DocumentType.Name,
                        DocumentsModelRootType.DeDriverLicenseBack.DocumentType.Name,
                        DocumentsModelRootType.DeDriverLicenseFront.DocumentType.Name,
                        DocumentsModelRootType.DeResidencePermitBack.DocumentType.Name,
                        DocumentsModelRootType.DeResidencePermitFront.DocumentType.Name,
                        DocumentsModelRootType.EuropeanHealthInsuranceCard.DocumentType.Name,
                        DocumentsModelRootType.DeHealthInsuranceCardFront.DocumentType.Name,
                    ]
                }
            ]
        };
        var result = await Detector.DocumentData.DetectOnImageAsync(image, configuration);
        if (result?.Document == null || result.Status != DocumentDataExtractionStatus.Success)
        {
            ViewUtils.Alert(this, "Error", "Could not extract the Document data.");
            return;
        }

        var message = SdkUtils.GenericDocumentToString(result.Document);
        ViewUtils.Alert(this, "Document Data Result", message);
    }

    private async Task CheckDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();
        if (image is null) return;
        
        var result = await Detector.Check.DetectOnImageAsync(image, new CheckScannerConfiguration
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
            ],
            DocumentDetectionMode = CheckDocumentDetectionMode.DetectAndCropDocument
        });

        if (result?.Check == null || result.Status != CheckMagneticInkStripScanningStatus.Success)
        {
            ViewUtils.Alert(this, "Error", "Could not detect the Check data.");
            return;
        }
        
        if (result.CroppedImage == null)
        {
            ViewUtils.Alert(this, "Check Result", SdkUtils.GenericDocumentToString(result.Check));
            return;
        }

        // Executes when the ExtractCroppedImage is set to true.
        ViewUtils.Alert(this, "Check Result", SdkUtils.ToAlertMessage(result), () =>
        {
            var resultPage = new DetectOnImageResultPage();
            var source = ImageSource.FromStream(() => result.CroppedImage.ToPlatformImage().AsStream());
            resultPage.NavigateData(source);
            Navigation.PushAsync(resultPage);
        });
    }

    private async Task MedicalCertificateDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsync();
        if (image is null) return;
        
        var configuration = new MedicalCertificateScanningParameters
        {
            ExtractCroppedImage = true
            // Configure other parameters as needed.
        };
        
        var result = await Detector.MedicalCertificate.DetectOnImageAsync(image, configuration);
        if (result?.DocumentDetectionResult == null || result.DocumentDetectionResult.Status != DocumentDetectionStatus.Ok || !result.ScanningSuccessful)
        {
            ViewUtils.Alert(this, "Error", "Could not detect the Medical Certificate data.");
            return;
        }

        if (result.CroppedImage == null)
        {
            ViewUtils.Alert(this, "Medical Certificate Result", result.ToFormattedString());
            return;
        }
        
        // Executes when the ExtractCroppedImage is set to true.
        ViewUtils.Alert(this, "Medical Certificate Result", result.ToFormattedString(), () =>
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
        if (image is null) return;
        
        var configuration = new CreditCardScannerConfiguration
        {
            // Configure other parameters as needed.
            RequireCardholderName = true
        };
        
        var result = await Detector.CreditCard.DetectOnImageAsync(image, configuration: configuration);
        if (result?.CreditCard == null || result.ScanningStatus != CreditCardScanningStatus.Success)
        {
            ViewUtils.Alert(this, "Error", "Could not detect the Credit card data.");
            return;
        }
        ViewUtils.Alert(this, "Credit Card result", SdkUtils.GenericDocumentToString(result.CreditCard));
    }
}