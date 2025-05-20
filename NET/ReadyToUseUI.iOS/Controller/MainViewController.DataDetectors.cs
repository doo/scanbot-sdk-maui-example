using ReadyToUseUI.iOS.Utils;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
    private void ScanMrz()
        {
            var config = SBSDKUIMRZScannerConfiguration.DefaultConfiguration;
            config.TextConfiguration.CancelButtonTitle = "Done";
            var controller = SBSDKUIMRZScannerViewController.CreateWithConfiguration(config, null);
            controller.DidScan += (viewController, args) =>
            {
                controller.IsRecognitionEnabled = false;
                controller.DismissViewController(true, delegate
                {
                    ShowPopup(this, FormattedString(args.Zone.Document));
                });
            };

            PresentViewController(controller, true, null);
        }

        private void ScanEhic()
        {
            var configuration = SBSDKUIHealthInsuranceCardRecognizerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var controller = SBSDKUIHealthInsuranceCardRecognizerViewController.CreateWithConfiguration(configuration, null);
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            controller.DidDetectCard += (_, args) =>
            {
                ShowPopup(controller, args.Card.ToJsonWithConfiguration(new SBSDKToJSONConfiguration()));
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
                
                ShowPopup(this, FormattedString(firstDocument.Document));
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
                if (args?.Result == null || args.Result.Check == null)
                    return;

                await controller.DismissViewControllerAsync(true);
                ShowPopup(this, FormattedString(args.Result.Check));
            };
            PresentViewController(controller, false, null);
        }

        private void ScanTextPattern()
        {
            SBSDKUI2TextPatternScannerViewController textScannerController = null;
            var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();
            configuration.TopBar.CancelButton.Text = "Done";
            textScannerController = SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, result =>
            {
                textScannerController?.DismissViewController(true, () => Alert.Show(this, "Result Text:", result?.RawText));
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
            scanner.DidFinishWithResult  += (_, args) =>
            {
                scanner.IsRecognitionEnabled = false;
                scanner?.DismissViewController(true, null);
                Alert.Show(this, "Result Text:", args.Result.ToJsonWithConfiguration(new SBSDKToJSONConfiguration()));
            };
            PresentViewController(scanner, true, null);
        }

        private void ScanCreditCard()
        {
            var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();
            configuration.TopBar.CancelButton.Text = "Done";

            SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, async (result) =>
            {
                if (result == null || result.CreditCard == null)
                    return;

                ShowPopup(this, FormattedString(result.CreditCard));
            });
        }
}