using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Constants;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Pages
{
	public partial class HomePage
	{
        private async Task MRZScannerClicked()
        {
            var configuration = new MrzScannerConfiguration
            {
                CancelButtonTitle = "Done",
                TopBarButtonsColor = Colors.Green
            };

            var result = await SBSDK.ReadyToUseUIService.LaunchMrzScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ParseMRZResult(result);
                ViewUtils.Alert(this, "MRZ Scanner result", message);
            }
        }

        private async Task EHICScannerClicked()
        {
            var configuration = new HealthInsuranceCardConfiguration
            {
                CancelButtonTitle = "Done",
                TopBarButtonsColor = Colors.Green
            };

            var result = await SBSDK.ReadyToUseUIService.LaunchHealthInsuranceCardScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ToAlertMessage(result);
                ViewUtils.Alert(this, "EHIC Scanner result", message);
            }
        }

        private async Task GenericDocumentRecognizerClicked()
        {
            var configuration = new GenericDocumentRecognizerConfiguration
            {
                DocumentType = GenericDocumentType.DeIdCard
            };
            var result = await SBSDK.ReadyToUseUIService.LaunchGenericDocumentRecognizerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ToAlertMessage(result);
                ViewUtils.Alert(this, "GDR Result", message);
            }
        }

        private async Task CheckRecognizerClicked()
        {
            var configuration = new CheckRecognizerConfiguration
            {
                AcceptedCheckStandards = new List<CheckStandard>() {
                    CheckStandard.USA,
                    CheckStandard.AUS,
                    CheckStandard.IND,
                    CheckStandard.FRA,
                    CheckStandard.KWT,
                }
            };

            var result = await SBSDK.ReadyToUseUIService.LaunchCheckRecognizerAsync(configuration);

            if (result.Status == OperationResult.Ok)
            {
                var message = SDKUtils.ToAlertMessage(result);
                ViewUtils.Alert(this, "Check Result", message);
            }
        }

        private async Task TextDataRecognizerClicked()
        {
            var configuration = new TextDataScannerConfiguration(new TextDataScannerStep("", "", 0, null))
            {

            };

            var result = await SBSDK.ReadyToUseUIService.LaunchTextDataScannerAsync(configuration);

            if (result.Status == OperationResult.Ok)
            {
                ViewUtils.Alert(this, $"Text Data Result", result.Text);
            }
        }

        private async Task VinRecognizerClicked()
        {
            var configuration = new VINScannerConfiguration
            {

            };

            var result = await SBSDK.ReadyToUseUIService.LaunchVINScannerAsync(configuration);

            if (result.Status == OperationResult.Ok)
            {
                ViewUtils.Alert(this, $"Text Data Result", result.Text);
            }
        }

        private async Task LicensePlateRecognizerClicked()
        {
            var configuration = new LicensePlateScannerConfiguration
            {

            };

            var result = await SBSDK.ReadyToUseUIService.LaunchLicensePlateScannerAsync(configuration);

            if (result.Status == OperationResult.Ok)
            {
                ViewUtils.Alert(this, $"License Plate Result", result.Text);
            }
        }
    }
}

