using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Utils;

public static class StringUtils
{
    
    internal static string ToFormattedString(this SBSDKGenericDocument document)
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
	
    internal static string ToFormattedString(this SBSDKEuropeanHealthInsuranceCardRecognitionResultField[] fields)
    {
        var formattedString = string.Empty;
        if (fields == null) return formattedString;

        foreach (var field in fields)
        {
            formattedString += $"{field.Type}: {field.Value ?? "-"}\n";
        }

        return formattedString;
    }
}