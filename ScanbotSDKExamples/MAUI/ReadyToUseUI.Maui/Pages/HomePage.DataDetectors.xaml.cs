using System.Text;
using ReadyToUseUI.Maui.Utils;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Constants;
using ScanbotSDK.MAUI.Models;
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
            // The AspectRatio is a required parameter for the Text Data Scanner.
            var aspectRatio = new AspectRatio(5, 1);
            var configuration = new TextDataScannerConfiguration(new TextDataScannerStep("", "", 0, aspectRatio))
            {
                // specify custom colors or settings here
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
                // specify custom colors or settings here
            };

            var result = await SBSDK.ReadyToUseUIService.LaunchVINScannerAsync(configuration);

            if (result.Status == OperationResult.Ok)
            {
                ViewUtils.Alert(this, $"Vin Scanner Result", result.Text);
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
        
        private async Task MedicalCertificateRecognizerClicked()
        {
            var configuration = new MedicalCertificateRecognizerConfiguration
            {

            };
            
            var result = await SBSDK.ReadyToUseUIService.LaunchMedicalCertificateScannerAsync(configuration);
            if (result.Status == OperationResult.Ok)
            {
                ViewUtils.Alert(this, $"Medical Certificate Recognition Result", FormatMedicalCertificateRecognitionResult(result));
            }
        }

        private string FormatMedicalCertificateRecognitionResult(MedicalCertificateResult result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append("Type: ").Append(result.Checkboxes?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckboxType.InitialCertificate) != null ? "Initial" :
                result.Checkboxes?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckboxType.RenewedCertificate) != null ? "Renewed" : "Unknown").Append("\n");
            
            stringBuilder.Append("Work Accident: ").Append(result.Checkboxes
                ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckboxType.WorkAccident) != null ? "Yes" : "No").Append("\n");
            
            stringBuilder.Append("Accident Consultant: ").Append(result.Checkboxes
                ?.FirstOrDefault(medicalCertificateInfoBox => medicalCertificateInfoBox.Type == MedicalCertificateCheckboxType.AssignedToAccidentInsuranceDoctor)
                != null ? "Yes" : "No").Append("\n");
            
            stringBuilder.Append("Start Date: ").Append(result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == MedicalCertificateDateType.IncapableOfWorkSince)?.DateString).Append("\n");
            
            stringBuilder.Append("End Date: ").Append(result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type ==  MedicalCertificateDateType.IncapableOfWorkUntil)?.DateString).Append("\n");
            
            stringBuilder.Append("Issue Date: ").Append(result.Dates?.FirstOrDefault(dateRecord => dateRecord.Type == MedicalCertificateDateType.DiagnosedOn)?.DateString).Append("\n");
            
            stringBuilder.Append($"Form type: {result.Type}").Append("\n");
            
            stringBuilder.Append(string.Join("\n", result.PatientFields.ToList().ConvertAll(field => $"{field.Type}: {field.Value}")));
            
            return stringBuilder?.ToString();
        }
    }
}

