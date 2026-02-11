using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Utils;
        
public static class Alert
{
    public static void Show(string title, string body)
    {
        var alert = UIAlertController.Create(title, body, UIAlertControllerStyle.Alert);
        alert.AddAction(UIAlertAction.Create("Close", UIAlertActionStyle.Cancel, delegate { }));
        AppDelegate.NavigationController.PresentViewController(alert, true, null);
    }
    
    public static void Show(Exception exception) => Show("Alert", exception.Message);
    
    public static void Show(NSError error) => Show("Alert", error.Description);
    
    /// <summary>
    /// Validate if error was a managed exception or Native iOS NSErrorException.
    /// Also checks if the Error was thrown due to RTU screen cancellation. In that case no error is displayed.
    /// </summary>
    /// <param name="ex">Exception object.</param>
    public static void ValidateAndShowError(Exception ex)
    {
        if (ex is not NSErrorException nsErrorException)
        {
            Show( "Error", ex.Message);
            return;
        }
            
        // Check if Scanner was canceled, with the Cancellation code.
        if (nsErrorException.Code == (nint)SBSDKErrorType.OperationCanceled)
        {
            // The RTU screen was cancelled.
            return;
        }
            
        // Highlight errors
        Show("Error", nsErrorException.Message);
    }
        
    /// <summary>
    /// Displays the error message.
    /// Validates if the Error was thrown due to RTU screen cancellation. In that case no error is displayed.
    /// </summary>
    /// <param name="error">NSError object.</param>
    public static void ValidateAndShowError(NSError error)
    {
        // Check if Scanner was cancelled, with the Cancellation code.
        if (error.Code == (nint)SBSDKErrorType.OperationCanceled)
        {
            // The RTU screen was cancelled.
            return;
        }
            
        // Highlight errors
        Show("Error", error.Description);
    }
}
