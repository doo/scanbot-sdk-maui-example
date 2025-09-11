namespace ScanbotSdkExample.Maui.Utils;

public class SharingUtils
{
    public static async Task ShareFileAsync(string localFilePath, string contentType)
    {
        if (string.IsNullOrEmpty(localFilePath) || !File.Exists(localFilePath))
        {
            // Handle file-not-found scenario
            Alert.Show("Error", "Unable to find the file:" + localFilePath);
            return;
        }

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Share PDF",
            File = new ShareFile(localFilePath, contentType ?? string.Empty)
        });
    }
}