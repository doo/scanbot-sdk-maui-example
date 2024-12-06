using ScanbotSDK.iOS;

namespace ClassicComponent.iOS.Models
{
    public class ImageProcessingParameters
	{
		public SBSDKPolygon Polygon { get; set; }

		public int Rotation { get; set; }

		public SBSDKParametricFilter Filter { get; set; } = new SBSDKParametricFilter();
	}
}

