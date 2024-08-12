using ReadyToUseUI.iOS.Utils;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public partial class MainViewController
{
    private void ScanMrz()
        {
            var config = SBSDKUIMRZScannerConfiguration.DefaultConfiguration;
            config.TextConfiguration.CancelButtonTitle = "Done";
            var controller = SBSDKUIMRZScannerViewController
                .CreateWithConfiguration(config, null);

            controller.DidDetect += (viewController, args) =>
            {
                controller.IsRecognitionEnabled = false;
                controller.DismissViewController(true, delegate
                {
                    ShowPopup(this, args.Zone.StringRepresentation);
                });
            };

            PresentViewController(controller, true, null);
        }

        private void ScanEhic()
        {
            var configuration = SBSDKUIHealthInsuranceCardScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var controller = SBSDKUIHealthInsuranceCardScannerViewController
                .CreateWithConfiguration(configuration, null);

            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            controller.DidDetectCard += (_, args) =>
            {
                ShowPopup(controller, args.Card.StringRepresentation);
            };
            PresentViewController(controller, false, null);
        }

        private void RecongnizeGenericDocument()
        {
            var configuration = SBSDKUIGenericDocumentRecognizerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            // Specify Document types if needed
            // configuration.BehaviorConfiguration.DocumentType = SBSDKUIDocumentType.IdCardFrontBackDE;
            var controller = SBSDKUIGenericDocumentRecognizerViewController.CreateWithConfigurationAndDelegate(configuration, null);

            controller.DidFinishWithDocuments += (_, args) =>
            {
                if (args.Documents == null || args.Documents.Length == 0)
                {
                    return;
                }

                // We only take the first document for simplicity
                var firstDocument = args.Documents.First();
                var fields = firstDocument.Fields
                    .Where((f) => f != null && f.Type != null && f.Type.Name != null && f.Value != null && f.Value.Text != null)
                    .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text))
                    .ToList();
                var description = string.Join("\n", fields);
                ShowPopup(this, description);
            };
            PresentViewController(controller, false, null);
        }

        private void RecognizeCheck()
        {
            var configuration = SBSDKUICheckRecognizerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            configuration.BehaviorConfiguration.AcceptedCheckStandards = new SBSDKCheckDocumentRootType[] {
                SBSDKCheckDocumentRootType.AusCheck,
                SBSDKCheckDocumentRootType.FraCheck,
                SBSDKCheckDocumentRootType.IndCheck,
                SBSDKCheckDocumentRootType.KwtCheck,
                SBSDKCheckDocumentRootType.UsaCheck,
                SBSDKCheckDocumentRootType.UaeCheck,
                SBSDKCheckDocumentRootType.CanCheck,
                SBSDKCheckDocumentRootType.IsrCheck,
            };
            var controller = SBSDKUICheckRecognizerViewController.CreateWithConfiguration(configuration, null);
            controller.DidRecognizeCheck += (_, args) =>
            {
                if (args.Result == null || args.Result.Document == null)
                {
                    return;
                }

                var fields = args.Result.Document.Fields
                    .Where((f) => f != null && f.Type != null && f.Type.Name != null && f.Value != null && f.Value.Text != null)
                    .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text))
                    .ToList();
                var description = string.Join("\n", fields);
                Console.WriteLine(description);

                controller.DismissViewController(true, null);

                ShowPopup(this, description);
            };
            PresentViewController(controller, false, null);
        }

        private void TextDataRecognizerTapped()
        {
            var configuration = SBSDKUITextDataScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUITextDataScannerViewController.CreateWithConfiguration(configuration, null);
            scanner.DidFinishStepWithResult += (_, args) =>
            {
                scanner.DismissViewController(true, () => Alert.Show(this, "Result Text:", args?.Result?.Text));
            };

            PresentViewController(scanner, true, null);
        }

        private void VinRecognizerTapped()
        {
            var configuration = SBSDKUIVINScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUIVINScannerViewController.CreateNew(configuration: configuration, @delegate: null);
            scanner.DidFinishWithResult += (_, args) =>
            {
                scanner.DismissViewController(true, () => Alert.Show(this, "Result Text:", args?.Result?.Text));
            };

            PresentViewController(scanner, true, null);
        }

        private void LicensePlateRecognizerTapped()
        {
            var configuration = SBSDKUILicensePlateScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUILicensePlateScannerViewController.CreateNew(configuration: configuration, @delegate: null);
            scanner.DidRecognizeLicensePlate += (_, args) =>
            {
                Alert.Show(this, "Result Text:", args?.Result?.RawString);
            };
            PresentViewController(scanner, true, null);
        }

        private void MedicalCertificateRecognizerTapped()
        {
            var configuration = SBSDKUIMedicalCertificateScannerConfiguration.DefaultConfiguration;
            configuration.TextConfiguration.CancelButtonTitle = "Done";
            var scanner = SBSDKUIMedicalCertificateScannerViewController.CreateWithConfiguration(configuration, null);
            scanner.DidFinishWithResult  += (_, args) =>
            {
                Alert.Show(this, "Result Text:", args.Result.StringRepresentation);
            };
            PresentViewController(scanner, true, null);
        }
}