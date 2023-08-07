using SBSDK = ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Model
{
    public class Filters
    {
        public static readonly List<Filter> List = new List<Filter>
        {
            new Filter("None", SBSDK.SBSDKImageFilterType.None),
            new Filter("Low Light Binarization", SBSDK.SBSDKImageFilterType.LowLightBinarization),
            new Filter("Low Light Binarization 2", SBSDK.SBSDKImageFilterType.LowLightBinarization2),
            new Filter("Edge Highlight", SBSDK.SBSDKImageFilterType.EdgeHighlight),
            new Filter("Deep Binarization", SBSDK.SBSDKImageFilterType.DeepBinarization),
            new Filter("Otsu Binarization", SBSDK.SBSDKImageFilterType.OtsuBinarization),
            new Filter("Clean Background", SBSDK.SBSDKImageFilterType.BackgroundClean),
            new Filter("Color Document", SBSDK.SBSDKImageFilterType.ColorDocument),
            new Filter("Color", SBSDK.SBSDKImageFilterType.Color),
            new Filter("Grayscale", SBSDK.SBSDKImageFilterType.Gray),
            new Filter("Binarized", SBSDK.SBSDKImageFilterType.Binarized),
            new Filter("Pure Binarized", SBSDK.SBSDKImageFilterType.PureBinarized),
            new Filter("Black & White", SBSDK.SBSDKImageFilterType.BlackAndWhite)
        };
    }
}
