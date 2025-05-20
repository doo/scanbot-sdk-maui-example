namespace ReadyToUseUI.Droid.Utils;

public static class StringUtils
{
    internal static string ToFormattedString(this IO.Scanbot.Sdk.Genericdocument.Entity.GenericDocument document)
    {
        if (document?.Fields == null || document.Fields.Count == 0)
            return string.Empty;
        
        var description = string.Empty;
        foreach (var field in document.Fields)
        {
            var typeValue = field.Value?.Text ?? string.Empty;
            description += $"{field.Type.Name}:  {typeValue}\n";
        }
        return description;
    }
}