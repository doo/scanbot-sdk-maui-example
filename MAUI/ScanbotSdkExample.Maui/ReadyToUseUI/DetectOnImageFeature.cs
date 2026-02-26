using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Check;
using ScanbotSDK.MAUI.Core.CreditCard;
using ScanbotSDK.MAUI.Core.DocumentData;
using ScanbotSDK.MAUI.Core.Mrz;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSdkExample.Maui.Results;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.ReadyToUseUI;

public static class DetectOnImageFeature
{
    public static async Task MrzDetectorClicked()
    {
        // @Tag("Scan MRZ from Image")
        var image = await ImagePicker.PickImageAsSourceAsync();
        if (image is null) return;

        var configuration = new MrzScannerConfiguration
        {
            IncompleteResultHandling = MrzIncompleteResultHandling.Reject
            // Configure other parameters as needed.
        };

        var result = await ScanbotSDKMain.Mrz.ScanFromImageAsync(image, configuration: configuration);
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        // success
        await Alert.ShowAsync("MRZ result", StringUtils.GenericDocumentToString(result.Value.Document));
        // @EndTag("Scan MRZ from Image")
    }

    public static async Task DocumentDataExtractorClicked()
    {
        var image = await ImagePicker.PickImageAsSourceAsync();
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
        var result = await ScanbotSDKMain.DocumentDataExtractor.ExtractFromImageAsync(image, configuration);
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        // success
        await Alert.ShowAsync("Document Data Result", StringUtils.GenericDocumentToString(result.Value.Document));
    }

    public static async Task CheckDetectorClicked()
    {
        var image = await ImagePicker.PickImageAsSourceAsync();
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

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        if (result.Value.CroppedImage == null)
        {
            await Alert.ShowAsync("Check Result", StringUtils.GenericDocumentToString(result.Value.Check));
            return;
        }

        // Executes when the ExtractCroppedImage is set to true.
        var accepted = await Alert.ShowAsync("Check Result", StringUtils.GenericDocumentToString(result.Value.Check), "Ok", "Cancel");
        if (accepted)
        {
            var resultPage = new DetectOnImageResultPage();
            var source = result.Value.CroppedImage.ToImageSource();
            resultPage.NavigateData(source);
            App.Navigation.PushAsync(resultPage);
        }
    }

    public static async Task CreditCardDetectorClicked()
    {
        // @Tag("Scan Credit Card from Image")
        var image = await ImagePicker.PickImageAsSourceAsync();
        if (image is null) return;

        var configuration = new CreditCardScannerConfiguration
        {
            // Configure other parameters as needed.
            RequireCardholderName = true
        };

        var result = await ScanbotSDKMain.CreditCard.ScanFromImageAsync(image, configuration: configuration);
        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        await Alert.ShowAsync("Credit Card result", StringUtils.GenericDocumentToString(result.Value.CreditCard));
        // @EndTag("Scan Credit Card from Image")
    }
}