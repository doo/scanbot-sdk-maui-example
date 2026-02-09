namespace ScanbotSdkExample.Maui.Utils;

public class PdfPicker
{
    public static async Task<string> PickAsync()
    {
        var file = await FilePicker.Default.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Pdf,
            PickerTitle = "Select a pdf file",
        });

        if (file == null)
        {
            await Alert.ShowAsync("Alert", "Something went wrong while picking the file from the storage.");
            return null;
        }
        
        return file.FullPath;
    }
}