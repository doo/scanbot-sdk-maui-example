using System.Text;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.GenericDocument;
using ScanbotSDK.MAUI.EHIC;
using ScanbotSDK.MAUI.MRZ;
using BarcodeItem = ScanbotSDK.MAUI.RTU.v1.Barcode;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Utils
{
    public class SDKUtils
    {
        public static bool CheckLicense(Page context)
        {
            if (!SBSDK.SDKService.IsLicenseValid)
            {
                ViewUtils.Alert(context, "Oops!", "License expired or invalid");
            }
            return SBSDK.SDKService.IsLicenseValid;
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
            foreach (var field in result.Document.Fields)
            {
                builder.AppendLine($"{field.Type.Name}: {field.Value.Text} ({field.Value.Confidence:F2})");
            }
            return builder.ToString();
        }

        public static string ToAlertMessage(HealthInsuranceCardScannerResult result)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"DocumentType: European Health insurance card");
            foreach (var field in result.Fields)
            {
                builder.AppendLine($"{field.Type}: {field.Value} ({field.Confidence:F2})");
            }
            return builder.ToString();
        }

        public static string ToAlertMessage(GenericDocumentRecognizerResult result) {
            return GenericDocumentToString(result.Documents.First());
        }

        public static string ToAlertMessage(CheckRecognizerResult result)
        {
            return GenericDocumentToString(result.Document);
        }

        private static string GenericDocumentToString(ScanbotSDK.MAUI.Common.GenericDocument document)
        {
            return string.Join("\n", document.Fields
                .Where((f) => f != null && f.Type.Name != null && f.Value.Text != null)
                .Select((f) => string.Format("{0}: {1}", f.Type.Name, f.Value.Text)));
        }
    }
}
