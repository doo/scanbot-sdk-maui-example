using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Mrz;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.DocumentData;
using ScanbotSDK.MAUI.Mrz;
using ScanbotSDK.MAUI.TextPattern;
using ScanbotSDK.MAUI.Vin;
using ScanbotSdkExample.Maui.Utils;

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

        var result = await ScanbotSDKMain.Mrz.StartScannerAsync(configuration);
        if (result.IsSuccess)
        {
            var message = SdkUtils.ParseMrzResult(result.Value);
            Alert.ShowAsync( "MRZ Result", message);
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
        
        var result = await ScanbotSDKMain.DocumentDataExtractor.StartScannerAsync(configuration);
        if (result.IsSuccess)
        {
            var message = SdkUtils.GenericDocumentToString(result.Value.Document);
            Alert.ShowAsync( "Document Data Result", message);
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

        var result = await ScanbotSDKMain.Check.StartScannerAsync(configuration);
        if (result.IsSuccess)
        {
            var message = SdkUtils.GenericDocumentToString(result.Value.Check);
            Alert.ShowAsync( "Check Result", message);
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
        
        var result = await ScanbotSDKMain.TextPattern.StartScannerAsync(configuration);

        if (result.IsSuccess)
        {
            Alert.ShowAsync( $"Text Pattern Result", result.Value.RawText);
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

        var result = await ScanbotSDKMain.Vin.StartScannerAsync(configuration);

        if (result.IsSuccess)
        {
            Alert.ShowAsync( $"Vin Result", result.Value.TextResult.RawText);
        }
    }
    
    public static async Task CreditCardScannerClicked()
    {
        var configuration = new CreditCardScannerScreenConfiguration();
        var result = await ScanbotSDKMain.CreditCard.StartScannerAsync(configuration);
        if (result.IsSuccess)
        {
            Alert.ShowAsync( $"Credit Card Result", SdkUtils.GenericDocumentToString(result.Value.CreditCard));
        }
    }
}