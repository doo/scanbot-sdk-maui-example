using ScanbotSDK.iOS;

namespace ClassicComponent.iOS.Models
{
    public class ImageProcessingParameters
	{
		public SBSDKPolygon Polygon { get; set; }

		public int Rotation { get; set; }

		public SBSDKImageFilterType Filter { get; set; } = SBSDKImageFilterType.None;
	}
}

