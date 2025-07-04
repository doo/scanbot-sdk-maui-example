using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.Ehic;
using ScanbotSDK.MAUI.DocumentData;
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
        
        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.White;

        // Add a top guidance title
        configuration.TopUserGuidance.Title = new StyledText
        {
            Text = "Scan MRZ",
            Color = Constants.Colors.ScanbotRed,
            UseShadow = true,
        };

        // Modify the action bar
        configuration.ActionBar.FlipCameraButton.Visible = false;
        configuration.ActionBar.FlashButton.ActiveForegroundColor = Constants.Colors.ScanbotRed;

        // Configure the scanner
        configuration.ScannerConfiguration.IncompleteResultHandling = MrzIncompleteResultHandling.Accept;

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
        
        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.White;

        // Add a top guidance title
        configuration.TopUserGuidance.Title = new StyledText
        {
            Text = "Scan Text Pattern",
            Color = Constants.Colors.ScanbotRed,
            UseShadow = true,
        };

        // Modify the action bar
        configuration.ActionBar.FlipCameraButton.Visible = false;
        configuration.ActionBar.FlashButton.ActiveForegroundColor = Constants.Colors.ScanbotRed;

        configuration.ScannerConfiguration.MinimumNumberOfRequiredFramesWithEqualScanningResult = 4;
        
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
            CancelButtonTitle = "Done",
            TopBarBackgroundColor = Constants.Colors.ScanbotRed,
            // ExtractVinFromBarcode = true,
        };

        var result = await Rtu.VinScanner.LaunchAsync(configuration);

        if (result.Status == OperationResult.Ok)
        {
            ViewUtils.Alert(this, $"Vin Result", result.Result.TextResult.RawText);
        }
    }
        
    private async Task MedicalCertificateScannerClicked()
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