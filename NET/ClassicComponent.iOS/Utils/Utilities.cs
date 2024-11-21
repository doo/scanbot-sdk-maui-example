using ClassicComponent.iOS.Models;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS.Utils
{
    public class Utilities
	{
        internal static T GetViewController<T>(string storyboardID) where T : UIViewController
        {
            var viewControllerId = typeof(T)?.Name ?? string.Empty;
            UIStoryboard storyboard = UIStoryboard.FromName(storyboardID, null);
            var viewController = storyboard.InstantiateViewController(viewControllerId) as T;
            return viewController;
        }

        internal static void CreateRoundedCardView(UIView view)
        {
            view.Layer.CornerRadius = 10.0f;
            view.Layer.ShadowColor = UIColor.LightGray.CGColor;
            view.Layer.ShadowRadius = 4.0f;
            view.Layer.ShadowOpacity = 0.3f;
            view.Layer.ShadowOffset = new CoreGraphics.CGSize(0, 2);
            view.ClipsToBounds = false;
        }

        internal static void ConfigureDefaultCell(UITableViewCell cell, string text)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
            {
                var config = UIListContentConfiguration.CellConfiguration;
                config.Text = text;
                config.TextProperties.Color = UIColor.SystemBlue;
                config.TextProperties.Font = UIFont.FromName("Helvetica", 16.0f);
                config.TextProperties.Alignment = UIListContentTextAlignment.Center;
                cell.ContentConfiguration = config;
            }
            else
            {
                cell.TextLabel.Text = text;
                cell.TextLabel.TextColor = UIColor.SystemBlue;
                cell.TextLabel.Font = UIFont.FromName("Helvetica", 16.0f);
                cell.TextLabel.TextAlignment = UITextAlignment.Center;
            }
        }

        internal static void ShowMessage(string title, string message)
        {
            var window = (UIApplication.SharedApplication.Delegate as AppDelegate).Window;
            window?.InvokeOnMainThread(() =>
            {
                var   presenter = window.RootViewController;
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                presenter?.PresentViewController(alertController, true, null);
            });
        }

        internal static NSUrl GenerateRandomFileUrlInDemoTempStorage(string fileExtension)
        {
            var targetFile = System.IO.Path.Combine(
                AppDelegate.Directory, new NSUuid().AsString().ToLower() + fileExtension);
            return NSUrl.FromFilename(targetFile);
        }

        internal static UIImage GetProcessedImage(ref UIImage originalImage, ImageProcessingParameters parameters)
        {
            if (parameters.Rotation != 0)
            {
                originalImage = originalImage.ImageRotatedCounterClockwise(parameters.Rotation);
            }
            var image = originalImage.ImageWarpedByPolygon(parameters.Polygon, 1.0f);
            parameters.Rotation = 0;
            image = image.ImageFilteredWithFilters(new[] { parameters.Filter });
            return image;
        }
    }
}

