using ScanbotSdkExample.iOS.Utils;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller;

public partial class MainViewController
{
    private void ScanMrz()
    {
        var configuration = new SBSDKUI2MRZScannerScreenConfiguration();
        SBSDKUI2MRZScannerViewController.PresentOn(this, configuration, result =>
        {
            if (result?.MrzDocument == null)
                return;

            ShowPopup(this, result.MrzDocument.ToFormattedString());
        });
    }

    private void ScanEhic()
    {
        var configuration = SBSDKUIHealthInsuranceCardRecognizerConfiguration.DefaultConfiguration;
        configuration.TextConfiguration.CancelButtonTitle = "Done";
        var controller = SBSDKUIHealthInsuranceCardRecognizerViewController.CreateWithConfiguration(configuration, null);
        controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        controller.DidDetectCard += (_, args) =>
        {
            controller.DismissViewController(true, () => ShowPopup(this, args.Card.Fields.ToFormattedString()));
        };
        PresentViewController(controller, false, null);
    }

    private void ExtractDocumentData()
    {
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();
        SBSDKUI2DocumentDataExtractorViewController.PresentOn(this, configuration, result =>
        {
            if (result?.Document == null)
                return;
            
            // Display the results.
            ShowPopup(this, result.Document?.ToFormattedString());
        });
    }

    private void ScanCheck()
    {
        var configuration = new SBSDKUI2CheckScannerScreenConfiguration();
        SBSDKUI2CheckScannerViewController.PresentOn(this, configuration, result =>
        {
            if (result?.Check == null)
                return;
           
            // Display the results.
            ShowPopup(this, result.Check?.ToFormattedString());
        });
    }

    private void ScanTextPattern()
    {
        var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";
        SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, result =>
        {
            if (!string.IsNullOrEmpty(result?.RawText))
            {
                Alert.Show(this, "Result Text:", result.RawText);
            }
        });
    }

    private void ScanVin()
    {
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();
        SBSDKUI2VINScannerViewController.PresentOn(this, configuration, result =>
        {
            if (result?.TextResult?.RawText == null)
                return;

            Alert.Show(this, "Result Text:", result.TextResult.RawText);
        });
    }

    private void ScanMedicalCertificate()
    {
        var configuration = SBSDKUIMedicalCertificateScannerConfiguration.DefaultConfiguration;
        configuration.TextConfiguration.CancelButtonTitle = "Done";
        var scanner = SBSDKUIMedicalCertificateScannerViewController.CreateWithConfiguration(configuration, null);
        scanner.DidFinishWithResult += (_, args) =>
        {
            scanner.IsRecognitionEnabled = false;
            scanner.DismissViewController(true, () =>
            {
                ShowPopupWithAttributedText(this, args.Result?.ToFormattedAttributeString());
            });
        };
        PresentViewController(scanner, true, null);
    }

    private void ScanCreditCard()
    {
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();
        configuration.TopBar.CancelButton.Text = "Done";

        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result?.CreditCard == null)
                return;

            ShowPopup(this, result.CreditCard?.ToFormattedString());
        });
    }
}