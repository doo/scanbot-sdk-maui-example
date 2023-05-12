using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentSDK.MAUI.Models;
using DocumentSDK.MAUI.Services;
using BarcodeItem = BarcodeSDK.MAUI.Models.Barcode;

namespace DocumentSDK.MAUI.Example.Utils
{
    public class SDKUtils
    {
        public static bool CheckLicense(ContentPage context)
        {
            if (!ScanbotSDK.SDKService.IsLicenseValid)
            {
                ViewUtils.Alert(context, "Oops!", "License expired or invalid");
            }
            return ScanbotSDK.SDKService.IsLicenseValid;
        }

        public static bool CheckPage(ContentPage context, IScannedPageService page)
        {
            var result = page != null;
            if (!result)
            {
                ViewUtils.Alert(context, "Oops!", "Please select a page");
            }
            return result;
        }

        public static bool CheckDocuments(ContentPage context, IEnumerable<ImageSource> documents)
        {
            var result = documents != null && documents.Count() > 0;
            if (!result)
            {
                ViewUtils.Alert(context, "Oops!", "Please import or scan a document first");
            }
            return result;
        }

        public static string ParseWorkflowResult(IWorkflowStepResult result)
        {
            var sb = new StringBuilder();
            if (result is IWorkflowBarcodeResult barcodeResult)
            {
                foreach (var barcode in barcodeResult.Barcodes)
                {
                    sb.AppendFormat("{0}\n{1}\n", barcode.Format, barcode.Text);
                }
            }
            if (result is IWorkflowMachineReadableZoneResult mrzResult)
            {
                if (mrzResult.MachineReadableZone != null
                    && mrzResult.MachineReadableZone.Fields != null)
                {
                    foreach (var field in mrzResult.MachineReadableZone.Fields)
                    {
                        sb.AppendFormat("{0}: {1}\n", field.Name, field.Value);
                    }
                    sb.AppendFormat("Valid check digits: {0}/{1}\n",
                        mrzResult.MachineReadableZone.ValidCheckDigitsCount,
                        mrzResult.MachineReadableZone.CheckDigitsCount);
                }
                else
                {
                    sb.AppendLine("MRZ not recognized");
                }
            }
            if (result is IWorkflowPayFormResult payformResult)
            {
                if (payformResult.PayForm != null && payformResult.PayForm.RecognizedFields != null)
                {
                    foreach (var field in payformResult.PayForm.RecognizedFields)
                    {
                        sb.AppendFormat("{0}: {1}\n", field.Token.Type, field.Value);
                    }
                }
                else
                {
                    sb.AppendLine("Payform not recognized");
                }
            }
            if (result is IWorkflowMedicalCertificateResult dcResult)
            {
                if (dcResult.MedicalCertificate.RecognitionSuccessful)
                {
                    foreach (var cb in dcResult.MedicalCertificate.Checkboxes)
                    {
                        sb.AppendFormat("{0}: {1}\n", cb.Type, cb.IsChecked ? "Yes" : "No");
                    }
                    foreach (var date in dcResult.MedicalCertificate.Dates)
                    {
                        sb.AppendFormat("{0}: {1}\n", date.Type, date.DateString);
                    }
                }
                else
                {
                    sb.AppendLine("DC not recognized");
                }
            }
            return sb.ToString();
        }

        public static string ParseWorkflowResults(IWorkflowStepResult[] results)
        {
            var builder = new StringBuilder();

            foreach(var result in results)
            {
                builder.Append(ParseWorkflowResult(result));
            }
            return builder.ToString();
        }

        public static string ParseBarcodes(List<BarcodeItem> barcodes)
        {
            var builder = new StringBuilder();

            foreach (var code in barcodes)
            {
                builder.AppendLine($"{code.Format}: {code.Text}");
            }

            return builder.ToString();
        }

        public static string ParseMRZResult(MrzScannerResult result)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"DocumentType: {result.DocumentType}");
            foreach (var field in result.Fields)
            {
                builder.AppendLine($"{field.Name}: {field.Value} ({field.Confidence:F2})");
            }
            return builder.ToString();
        }

        public static string ParseEHICResult(HealthInsuranceCardScannerResult result)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"DocumentType: European Health insurance card");
            foreach (var field in result.Fields)
            {
                builder.AppendLine($"{field.Type}: {field.Value} ({field.Confidence:F2})");
            }
            return builder.ToString();
        }

        public static string ParseGDRResult(GenericDocumentRecognizerResult result) {
            return GenericDocumentToString(result.Documents.First());
        }

        public static string ParseCheckResult(CheckRecognizerResult result)
        {
            return GenericDocumentToString(result.Document);
        }

        private static string GenericDocumentToString(GenericDocument document)
        {
            return string.Join("\n", document.Fields
                .Where((f) => f != null && f.Type != null && f.Type.Name != null && f.Value != null && f.Value.Text != null)
                .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text)));
        }
    }
}
