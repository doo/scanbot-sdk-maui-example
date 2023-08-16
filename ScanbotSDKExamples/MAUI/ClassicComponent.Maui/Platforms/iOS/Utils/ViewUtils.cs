using UIKit;
using ClassicComponent.Maui.CustomViews;

namespace ClassicComponent.Maui.Platforms.iOS.Utils
{
	public static class ViewUtils
	{
        // ------------------------------------------------------------------------------------------------------------------------
        // This method returns the viewController from the iOS navigation hierarchy.
        // Initially the UIWindow object in iOS is not available(null), so we have to give it a delay, to get the UIWindow instance.
        // -------------------------------------------------------------------------------------------------------------------------
        internal static async Task<UIViewController> TryGetTopViewControllerAsync(BarcodeCameraViewHandler barcodeCameraViewHandler)
        {
            var retryCount = 5;

            UIViewController viewController = null;
            do
            {
                await Task.Delay(500);
                viewController = ExtractViewControllerFromWindow(barcodeCameraViewHandler?.PlatformView?.Window);
                retryCount++;
            } while (viewController == null || retryCount < 5);
            return viewController;
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // Extracts the TopViewController from the navigation hierarchy by accessing the root UIWindow object of the application.
        // ------------------------------------------------------------------------------------------------------------------------
        internal static UIViewController ExtractViewControllerFromWindow(UIWindow window)
        {
            var viewController = window?.RootViewController;

            if (viewController == null) return null;

            // If application has a Navigation Controller
            if (viewController is UINavigationController navigationController)
            {
                // Note: The Navigation Controller has a Navigation Renderer in between the NavigationController and MainViewController, so we cannot use "navigationController?.VisibleViewController".
                return navigationController?.VisibleViewController?.ChildViewControllers?.Last();
            }
            else if (viewController is UITabBarController tabBarController)
            {
                // It is itself a Page renderer.
                return tabBarController.SelectedViewController;
            }
            else
            {   // If application has no Navigation Controller OR TabBarController
                return viewController;
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        // Displays a popup message with message and a single button.s
        // ------------------------------------------------------------------------------------------------------------------------
        internal static void ShowAlert(string message, string buttonTitle)
        {
            var alert = UIAlertController.Create("Alert", message, UIAlertControllerStyle.Alert);
            var action = UIAlertAction.Create(buttonTitle ?? "Ok", UIAlertActionStyle.Cancel, (obj) => { });
            alert.AddAction(action);
            ExtractViewControllerFromWindow(AppDelegate.RootWindow)?.PresentViewController(alert, true, null);
        }
    }
}