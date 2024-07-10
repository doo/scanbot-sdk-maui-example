using System.Text;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.EHIC;
using ScanbotSDK.MAUI.GenericDocument;
using ScanbotSDK.MAUI.MRZ;
using BarcodeItem = ScanbotSDK.MAUI.Barcode.BarcodeItem;
using SBSDK = ScanbotSDK.MAUI.ScanbotSDK;

namespace ReadyToUseUI.Maui.Utils
{
    public static class SDKUtils
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
                builder.AppendLine($"{code}: {code.Type}");
            }

            return builder.ToString();
        }

        public static string ParseMRZResult(MrzScannerResult result)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"DocumentType: {result.DocumentType}");
            foreach (var field in result.Document.Fields)
            {
                builder.AppendLine($"{field.Name}: {field.Value.Text} ({field.Value.Confidence:F2})");
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
                .Where((f) => f != null && f.Name != null && f.Name != null && f.Value.Text != null)
                .Select((f) => string.Format("{0}: {1}", f.Name, f.Value.Text)));
        }

        internal static string FilterToJson(ParametricFilter filter)
        {
            var text = System.Text.Json.JsonSerializer.Serialize(filter);
            return text;
        }
        
        internal static ParametricFilter JsonToFilter(string jsonString)
        {
            var item = System.Text.Json.JsonSerializer.Deserialize<ParametricFilter>(jsonString);
            return item;
        }
        
        internal static void PrintJson(object modelObject)
        {
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(modelObject);
            System.Diagnostics.Debug.WriteLine(text);
        }
    }
}
