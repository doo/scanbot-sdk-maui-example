using System.Text;
using ReadyToUseUI.Maui.Pages.DocumentFilters;
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

        
        internal static string FilterToJson(ParametricFilter[] filters)
        {
            if (filters == null)
                return string.Empty;
            
            var dictionary = new Dictionary<string, string>();
            foreach (var filter in filters)
            {
                dictionary.Add(filter.GetType().Name, filter.ToJson());
            }

            var text = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
            return text;
        }

        internal static ParametricFilter[] JsonToFilter(string jsonString)
        {
            var filterObjects = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

            if (filterObjects == null)
                return null;
            
            var strongTypedList = new List<ParametricFilter>();
            foreach (var filterObject in filterObjects)
            { 
                switch (filterObject.Key)
                {
                    case nameof(ParametricFilter): // LegacyFilter
                        strongTypedList.Add(filterObject.Value.FromJson<LegacyFilter>());
                        continue;
                    
                    case nameof(ColorDocumentFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<ColorDocumentFilter>());
                        continue;
                    
                    case nameof(ScanbotBinarizationFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<ScanbotBinarizationFilter>());
                        continue;
                    
                    case nameof(CustomBinarizationFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<CustomBinarizationFilter>());
                        continue;
                    
                    case nameof(BrightnessFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<BrightnessFilter>());
                        continue;
                    
                    case nameof(ContrastFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<ContrastFilter>());
                        continue;
                    
                    case nameof(GrayscaleFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<GrayscaleFilter>());
                        continue;
                    
                    case nameof(WhiteBlackPointFilter): 
                        strongTypedList.Add(filterObject.Value.FromJson<WhiteBlackPointFilter>());
                        continue;
                    
                        default:
                        throw new Exception("Filter unsupported.");
                }
            }

            return strongTypedList.ToArray();
        }
        
        internal static void PrintJson(object modelObject)
        {
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(modelObject);
            System.Diagnostics.Debug.WriteLine(text);
        }
        
        internal static string ToJson(this object modelObject)
        {
            var text = Newtonsoft.Json.JsonConvert.SerializeObject(modelObject);
            System.Diagnostics.Debug.WriteLine(text);
            return text;
        }
        
        internal static T FromJson<T>(this string jsonString)
        {
            var modalObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
            return modalObject;
        }
    }
}
