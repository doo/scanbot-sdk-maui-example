namespace ScanbotSdkExample.Maui.Utils;
public static class ViewUtils
{
    public static void Alert(string title, string message)
    {
        _ = MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "Close");
        });
    }
        
    public static async void Alert(string title, string message, Action completion)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var result =  await Application.Current.MainPage.DisplayAlert(title, message, "View Detected Image", "Close");
            if (result)
            {
                completion?.Invoke();
            }
        });
    }
}