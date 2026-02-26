namespace ScanbotSdkExample.Maui.Utils;

public class PdfPicker
{
    public static async Task<string> PickAsync()
    {
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Select a pdf file",
            });
            
            return file?.FullPath;
        } 
        catch (Exception ex) 
        {
            await Alert.ShowAsync("Error", $"Unable to pick pdf file: {ex.Message}");
            return null;
        }
    }
}