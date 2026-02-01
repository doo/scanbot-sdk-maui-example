using System.Text;
using ScanbotSDK.MAUI.Core.Check;
using ScanbotSDK.MAUI.Core.DocumentData;
using ScanbotSDK.MAUI.Core.GenericDocument;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Utils;
public static class StringUtils
{
    public static string CopyrightLabel => $"Copyright (c) {DateTime.Now.Year} Scanbot SDK GmbH. All rights reserved";
    
    public static string ParseMrzResult(MrzScannerUiResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"DocumentType: {result.MrzDocument.Type.Name}");
        foreach (var field in result.MrzDocument.Fields)
        {
            builder.AppendLine($"{field.Type.Name}: {field.Value.Text} ({field.Value.Confidence:F2})");
        }
        return builder.ToString();
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