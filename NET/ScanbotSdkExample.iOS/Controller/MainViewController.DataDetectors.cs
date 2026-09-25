using ScanbotSdkExample.iOS.Utils;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class MainViewController
{
    private void ScanMrz()
    {
        var configuration = new SBSDKUI2MRZScannerScreenConfiguration();
        SBSDKUI2MRZScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            if (result?.MrzDocument == null)
                return;

            ShowPopup(this, result.MrzDocument.ToFormattedString());
        }, out var nsError);
    }
    
    private void ExtractDocumentData()
    {
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        SBSDKUI2DocumentDataExtractorViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }
            
            // Display the results.
            ShowPopup(this, result.Document?.ToFormattedString());
        }, out var nsError);
    }

    private void ScanCheck()
    {
        var configuration = new SBSDKUI2CheckScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        SBSDKUI2CheckScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }
           
            // Display the results.
            ShowPopup(this, result.Check?.ToFormattedString());
        },  out var nsError);
    }

    private void ScanTextPattern()
    {
        var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(result.RawText))
            {
                Alert.Show("Alert", "Something went wrong while scanning the text.");
                return;
            }
            Alert.Show("Result", result.RawText);
        },  out var nsError);
    }

    private void ScanVin()
    {
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        SBSDKUI2VINScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }

            if (!result.TextResult.ValidationSuccessful)
            {
                Alert.Show("Alert", "Something went wrong while scanning the text.");
                return;
            }

            Alert.Show("Result", result.TextResult.RawText);
        }, out var nsError);
    }

    private void ScanCreditCard()
    {
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";

        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (error != null)
            {
                Alert.ValidateAndShowError(error);
                return;
            }
            
            if (result?.CreditCard == null)
                return;

            ShowPopup(this, result.CreditCard?.ToFormattedString());
        }, out var nsError);
    }
}