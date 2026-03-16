using System.Diagnostics;

namespace ScanbotSdkExample.Maui.Utils;

public class ImagePicker
{
    // Workaround for a .NET MAUI bug (https://github.com/dotnet/maui/pull/34250):
    // After the native photo picker is dismissed, the platform needs a moment to
    // fully restore the original view/window hierarchy. Without this delay, any
    // subsequent UI call (e.g., Alert.ShowAsync) may silently fail because it tries
    // to present over a window that is still mid-transition.
    private const int PickerDismissalDelayMs = 500;

    private static async Task<FileResult> PickImageAsync()
    {
        var options = new MediaPickerOptions
        {
            Title = "Select a photo",
            SelectionLimit = 1
        };

        var pickedList = await MediaPicker.Default.PickPhotosAsync(options);
        await Task.Delay(PickerDismissalDelayMs);
        return pickedList?.FirstOrDefault();
    }

    /// <summary>
    /// Picks an image from the photos application.
    /// </summary>
    /// <returns>An <see cref="ImageSource"/> of the picked image, or <c>null</c> if cancelled.</returns>
    public static async Task<ImageSource> PickImageAsSourceAsync()
    {
        try
        {
            var file = await PickImageAsync();
            if (file is null)
                return null;

            var stream = await file.OpenReadAsync();
            return ImageSource.FromStream(() => stream);
        }
        catch (Exception ex)
        {
            await Alert.ShowAsync("Error", $"Unable to pick image: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Picks an image from the photos application.
    /// </summary>
    /// <returns>The local file path of the picked image, or <c>null</c> if cancelled.</returns>
    public static async Task<string> PickImageAsPathAsync()
    {
        try
        {
            var file = await PickImageAsync();
            if (file?.FullPath is null)
                return null;

            var path = file.FullPath;
            if (IsValidPath(path))
                return path;

            // On iOS, FullPath may return only a filename without a valid absolute path.
            // In that case, copy the picked image to the local cache directory.
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(extension))
                extension = ".jpg";

            var cacheDir = Path.Combine(FileSystem.CacheDirectory, "gallery-picked-items");
            Directory.CreateDirectory(cacheDir);
            path = Path.Combine(cacheDir, file.FileName);

            var stream = await file.OpenReadAsync();
            await using var destinationStream = File.Create(path);
            await stream.CopyToAsync(destinationStream);

            return path;
        }
        catch (Exception ex)
        {
            await Alert.ShowAsync("Error", $"Unable to pick image: {ex.Message}");
        }

        return null;
    }

    private static bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        try
        {
            return File.Exists(path);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not validate path. Details:\n{e.Message}");
            return false;
        }
    }
}