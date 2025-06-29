using System.Text;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.MRZ;

namespace ScanbotSdkExample.Maui.Utils;

public static class SdkUtils
{
    public static bool CheckLicense(Microsoft.Maui.Controls.Page context)
    {
        if (!ScanbotSDKMain.IsLicenseValid)
        {
            ViewUtils.Alert(context, "Oops!", "License expired or invalid");
        }
        return ScanbotSDKMain.IsLicenseValid;
    }

    public static string ParseMrzResult(MrzScannerUiResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"DocumentType: {result.MrzDocument.Type}");
        foreach (var field in result.MrzDocument.Fields)
        {
            builder.AppendLine($"{field.Type.Name}: {field.Value.Text} ({field.Value.Confidence:F2})");
        }
        return builder.ToString();
    }

    public static string ToAlertMessage(EuropeanHealthInsuranceCardRecognitionResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"DocumentType: European Health insurance card");
        foreach (var field in result.Fields)
        {
            builder.AppendLine($"{field.Type}: {field.Value} ({field.Confidence:F2})");
        }
        return builder.ToString();
    }

    public static string ToAlertMessage(DocumentDataExtractionResult[] result) {
        return GenericDocumentToString(result.First().Document);
    }

    public static string ToAlertMessage(CheckScanningResult result)
    {
        return GenericDocumentToString(result.Check);
    }

    internal static string GenericDocumentToString(GenericDocument document)
    {
        var formattedString = string.Empty;
        if (document?.Fields == null) return formattedString;
		
        foreach (var field in document.Fields)
        {
            if (string.IsNullOrEmpty(field?.Type?.Name))
                continue;
            formattedString += $"{field.Type.Name}: {field.Value?.Text ?? "-"}\n";
        }

        return formattedString;
    }
}