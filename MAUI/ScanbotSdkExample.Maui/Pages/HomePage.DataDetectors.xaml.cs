using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.Ehic;
using ScanbotSDK.MAUI.DocumentData;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.MedicalCertificate;
using ScanbotSDK.MAUI.MRZ;
using ScanbotSDK.MAUI.textpattern;
using ScanbotSdkExample.Maui.Utils;
using static ScanbotSDK.MAUI.ScanbotSDKMain;
using VinScannerConfiguration = ScanbotSDK.MAUI.Vin.VinScannerConfiguration;

namespace ScanbotSdkExample.Maui.Pages;
public partial class HomePage
{
    private async Task MrzScannerClicked()
    {
        var configuration = new MrzScannerScreenConfiguration();

        var result = await Rtu.MrzScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SdkUtils.ParseMrzResult(result.Result);
            ViewUtils.Alert(this, "MRZ Result", message);
        }
    }

    private async Task EhicScannerClicked()
    {
        var configuration = new EhicScannerConfiguration
        {
            CancelButtonTitle = "Done",
            TopBarButtonsColor = Colors.Green
        };

        var result = await Rtu.EhicScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SdkUtils.ToAlertMessage(result.Result);
            ViewUtils.Alert(this, "EHIC Result", message);
        }
    }

    private async Task DocumentDataScannerClicked()
    {
        var configuration = new ScanbotSDK.MAUI.DocumentData.DocumentDataExtractorConfiguration
        {
            AcceptedDocumentTypes = 
            [
                DocumentDataRootType.DeIdCardFront,
                DocumentDataRootType.DeIdCardBack,
                DocumentDataRootType.DePassport,
                DocumentDataRootType.DeDriverLicenseFront,
                DocumentDataRootType.DeDriverLicenseBack,
                DocumentDataRootType.DeResidencePermitFront,
                DocumentDataRootType.DeResidencePermitBack,
                DocumentDataRootType.EuropeanHealthInsuranceCard,
                DocumentDataRootType.DeHealthInsuranceCardFront
            ]
        };
            
        var result = await Rtu.DocumentDataExtractor.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SdkUtils.ToAlertMessage(result.Result);
            ViewUtils.Alert(this, "Document Data Result", message);
        }
    }

    private async Task CheckScannerClicked()
    {
        var configuration = new ScanbotSDK.MAUI.Check.CheckScannerConfiguration
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
        };

        var result = await Rtu.CheckScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SdkUtils.ToAlertMessage(result.Result);
            ViewUtils.Alert(this, "Check Result", message);
        }
    }

    private async Task TextPatternScannerClicked()
    {
        var configuration = new TextPatternScannerScreenConfiguration();
        var result = await Rtu.TextPatternScanner.LaunchAsync(configuration);

        if (result.Status == OperationResult.Ok)
        {
            ViewUtils.Alert(this, $"Text Pattern Result", result.Result.RawText);
        }
    }

    private async Task VinScannerClicked()
    {
        var configuration = new VinScannerConfiguration
        {
            // specify custom colors or settings here
        };

        var result = await Rtu.VinScanner.LaunchAsync(configuration);

        if (result.Status == OperationResult.Ok)
        {
            ViewUtils.Alert(this, $"Vin Scanner Result", result.Result.TextResult.RawText);
        }
    }
        
    private async Task MedicalCertificateRecognizerClicked()
    {
        var configuration = new MedicalCertificateScannerConfiguration();
        var result = await Rtu.MedicalCertificateScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            ViewUtils.Alert(this, $"Medical Certificate Result", result.Result.ToFormattedString());
        }
    }

    private async Task CreditCardScannerClicked()
    {
        var configuration = new CreditCardScannerScreenConfiguration();
        var result = await Rtu.CreditCard.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            ViewUtils.Alert(this, $"Credit Card Result", SdkUtils.GenericDocumentToString(result.Result.CreditCard));
        }
    }
}