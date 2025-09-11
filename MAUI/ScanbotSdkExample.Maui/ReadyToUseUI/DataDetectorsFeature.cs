using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Mrz;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.Ehic;
using ScanbotSDK.MAUI.DocumentDataExtractor;
using ScanbotSDK.MAUI.MedicalCertificate;
using ScanbotSDK.MAUI.Mrz;
using ScanbotSDK.MAUI.TextPattern;
using ScanbotSDK.MAUI.Vin;
using ScanbotSdkExample.Maui.Utils;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ScanbotSdkExample.Maui.ReadyToUseUI;

public static class DataDetectorsFeature
{
    public static async Task MrzScannerClicked()
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
            Alert.Show( "MRZ Result", message);
        }
    }

    public static async Task EhicScannerClicked()
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
            Alert.Show( "EHIC Result", message);
        }
    }

    public static async Task DocumentDataScannerClicked()
    {
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.White;

        // Add a top guidance title
        configuration.TopUserGuidance.Title = new StyledText
        {
            Text = "Extract Document Data",
            Color = Constants.Colors.ScanbotRed,
            UseShadow = true,
        };

        // Modify the action bar
        configuration.ActionBar.FlipCameraButton.Visible = false;
        configuration.ActionBar.FlashButton.ActiveForegroundColor = Constants.Colors.ScanbotRed;
        
        var result = await Rtu.DocumentDataExtractor.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SdkUtils.GenericDocumentToString(result.Result.Document);
            Alert.Show( "Document Data Result", message);
        }
    }

    public static async Task CheckScannerClicked()
    {
        var configuration = new CheckScannerScreenConfiguration();
        
        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.White;

        // Add a top guidance title
        configuration.TopUserGuidance.Title = new StyledText
        {
            Text = "Extract Document Data",
            Color = Constants.Colors.ScanbotRed,
            UseShadow = true,
        };

        // Modify the action bar
        configuration.ActionBar.FlipCameraButton.Visible = false;
        configuration.ActionBar.FlashButton.ActiveForegroundColor = Constants.Colors.ScanbotRed;

        var result = await Rtu.CheckScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            var message = SdkUtils.GenericDocumentToString(result.Result.Check);
            Alert.Show( "Check Result", message);
        }
    }

    public static async Task TextPatternScannerClicked()
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
            Alert.Show( $"Text Pattern Result", result.Result.RawText);
        }
    }

    public static async Task VinScannerClicked()
    {
        var configuration = new VinScannerScreenConfiguration();
        
        // Set colors
        configuration.Palette.SbColorPrimary = Constants.Colors.ScanbotRed;
        configuration.Palette.SbColorOnPrimary = Colors.White;

        // Add a top guidance title
        configuration.TopUserGuidance.Title = new StyledText
        {
            Text = "Extract Document Data",
            Color = Constants.Colors.ScanbotRed,
            UseShadow = true,
        };

        // Modify the action bar
        configuration.ActionBar.FlipCameraButton.Visible = false;
        configuration.ActionBar.FlashButton.ActiveForegroundColor = Constants.Colors.ScanbotRed;

        var result = await Rtu.VinScanner.LaunchAsync(configuration);

        if (result.Status == OperationResult.Ok)
        {
            Alert.Show( $"Vin Result", result.Result.TextResult.RawText);
        }
    }
        
    public static async Task MedicalCertificateScannerClicked()
    {
        var configuration = new MedicalCertificateScannerConfiguration();
        var result = await Rtu.MedicalCertificateScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            Alert.Show( $"Medical Certificate Result", result.Result.ToFormattedString());
        }
    }

    public static async Task CreditCardScannerClicked()
    {
        var configuration = new CreditCardScannerScreenConfiguration();
        var result = await Rtu.CreditCardScanner.LaunchAsync(configuration);
        if (result.Status == OperationResult.Ok)
        {
            Alert.Show( $"Credit Card Result", SdkUtils.GenericDocumentToString(result.Result.CreditCard));
        }
    }
}