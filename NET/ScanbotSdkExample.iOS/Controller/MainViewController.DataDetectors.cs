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

    private void ScanDocumentData()
    {
        var configuration = SBSDKUIDocumentDataExtractorConfiguration.DefaultConfiguration;
        configuration.TextConfiguration.CancelButtonTitle = "Done";
        // Specify Document types if needed
        // configuration.BehaviorConfiguration.DocumentType = SBSDKUIDocumentType.IdCardFrontBackDE;
        var controller = SBSDKUIDocumentDataExtractorViewController.CreateWithConfigurationAndDelegate(configuration, null);

        controller.DidFinishWithResults += (_, args) =>
        {
            if (args?.Results == null || args.Results.Length == 0)
            {
                return;
            }

            // We only take the first document for simplicity
            var firstDocument = args.Results.First();

            ShowPopup(this, firstDocument.Document?.ToFormattedString());
        };
        PresentViewController(controller, false, null);
    }

    private void ScanCheck()
    {
        var configuration = SBSDKUICheckScannerConfiguration.DefaultConfiguration;
        configuration.TextConfiguration.CancelButtonTitle = "Done";
        configuration.BehaviorConfiguration.AcceptedCheckStandards =
        [
            SBSDKCheckDocumentModelRootType.AusCheck,
            SBSDKCheckDocumentModelRootType.FraCheck,
            SBSDKCheckDocumentModelRootType.IndCheck,
            SBSDKCheckDocumentModelRootType.KwtCheck,
            SBSDKCheckDocumentModelRootType.UsaCheck,
            SBSDKCheckDocumentModelRootType.UaeCheck,
            SBSDKCheckDocumentModelRootType.CanCheck,
            SBSDKCheckDocumentModelRootType.IsrCheck,
        ];

        var controller = SBSDKUICheckScannerViewController.CreateWithConfiguration(configuration, null);
        controller.DidScanCheck += async (_, args) =>
        {
            await controller.DismissViewControllerAsync(true);
            if (args?.Result == null || args.Result.Check == null)
                return;
           
            ShowPopup(this, args.Result.Check?.ToFormattedString());
        };
        PresentViewController(controller, false, null);
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
        var configuration = SBSDKUIVINScannerConfiguration.DefaultConfiguration;
        configuration.TextConfiguration.CancelButtonTitle = "Done";
        var scanner = SBSDKUIVINScannerViewController.CreateNew(configuration: configuration, @delegate: null);
        scanner.DidFinishWithResult += (_, args) =>
        {
            var text = args?.Result?.TextResult?.RawText ?? string.Empty;
            scanner.DismissViewController(true, () => Alert.Show(this, "Result Text:", text));
        };

        PresentViewController(scanner, true, null);
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