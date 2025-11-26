using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Check;
using ScanbotSDK.MAUI.Core.CreditCard;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Core.DocumentData;
using ScanbotSDK.MAUI.Core.MedicalCertificate;
using ScanbotSDK.MAUI.Core.Mrz;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSdkExample.Maui.Results;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.ReadyToUseUI;

public static class DetectOnImageFeature
{
    public static async Task MrzDetectorClicked()
    {
        var image = await HomePage.PickImageAsync();
        if (image is null) return;

        var configuration = new MrzScannerConfiguration
        {
            IncompleteResultHandling = MrzIncompleteResultHandling.Reject
            // Configure other parameters as needed.
        };

        var result = await ScanbotSDKMain.Mrz.ScanFromImageAsync(image, configuration: configuration);
        if (result?.Document == null || result.Success == false)
        {
            Alert.Show( "Error", "Could not detect the MRZ data.");
            return;
        }
        Alert.Show( "MRZ result", SdkUtils.GenericDocumentToString(result.Document));
    }

    public static async Task DocumentDataExtractorClicked()
    {
        var image = await HomePage.PickImageAsync();
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
                        DocumentsModelRootType.DeResidencePermitBack.DocumentType.Name,
                        DocumentsModelRootType.DeResidencePermitFront.DocumentType.Name,
                        DocumentsModelRootType.EuropeanHealthInsuranceCard.DocumentType.Name,
                        DocumentsModelRootType.DeHealthInsuranceCardFront.DocumentType.Name,
                    ]
                }
            ]
        };
        var result = await ScanbotSDKMain.DocumentDataExtractor.ScanFromImageAsync(image, configuration);
        if (result?.Document == null || result.Status != DocumentDataExtractionStatus.Ok)
        {
            Alert.Show( "Error", "Could not extract the Document data.");
            return;
        }

        var message = SdkUtils.GenericDocumentToString(result.Document);
        Alert.Show( "Document Data Result", message);
    }

    public static async Task CheckDetectorClicked()
    {
        var image = await HomePage.PickImageAsync();
        if (image is null) return;
        
        var result = await ScanbotSDKMain.Check.ScanFromImageAsync(image, new CheckScannerConfiguration
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
            Alert.Show( "Error", "Could not detect the Check data.");
            return;
        }
        
        if (result.CroppedImage == null)
        {
            Alert.Show( "Check Result", SdkUtils.GenericDocumentToString(result.Check));
            return;
        }

        // Executes when the ExtractCroppedImage is set to true.
        Alert.Show( "Check Result", SdkUtils.ToAlertMessage(result), () =>
        {
            var resultPage = new DetectOnImageResultPage();
            var source = ImageSource.FromStream(() => result.CroppedImage.ToPlatformImage().AsStream());
            resultPage.NavigateData(source);
            App.Navigation.PushAsync(resultPage);
        });
    }

    public static async Task MedicalCertificateDetectorClicked()
    {
        var image = await HomePage.PickImageAsync();
        if (image is null) return;
        
        var configuration = new MedicalCertificateScanningParameters
        {
            ExtractCroppedImage = true
            // Configure other parameters as needed.
        };
        
        var result = await ScanbotSDKMain.MedicalCertificate.ScanFromImageAsync(image, configuration);
        if (result?.DocumentDetectionResult == null || result.DocumentDetectionResult.Status != DocumentDetectionStatus.Ok || !result.ScanningSuccessful)
        {
            Alert.Show( "Error", "Could not detect the Medical Certificate data.");
            return;
        }

        if (result.CroppedImage == null)
        {
            Alert.Show( "Medical Certificate Result", result.ToFormattedString());
            return;
        }
        
        // Executes when the ExtractCroppedImage is set to true.
        Alert.Show( "Medical Certificate Result", result.ToFormattedString(), () =>
        {
            var resultPage = new DetectOnImageResultPage();
            var source = ImageSource.FromStream(() => result.CroppedImage.ToPlatformImage().AsStream());
            resultPage.NavigateData(source);
            App.Navigation.PushAsync(resultPage);

        });
    }

    public static async Task CreditCardDetectorClicked()
    {
        var image = await HomePage.PickImageAsync();
        if (image is null) return;
        
        var configuration = new CreditCardScannerConfiguration
        {
            // Configure other parameters as needed.
            RequireCardholderName = true
        };
        
        var result = await ScanbotSDKMain.CreditCard.ScanFromImageAsync(image, configuration: configuration);
        if (result?.CreditCard == null || result.ScanningStatus != CreditCardScanningStatus.Success)
        {
            Alert.Show( "Error", "Could not detect the Credit card data.");
            return;
        }
        Alert.Show( "Credit Card result", SdkUtils.GenericDocumentToString(result.CreditCard));
    }
}