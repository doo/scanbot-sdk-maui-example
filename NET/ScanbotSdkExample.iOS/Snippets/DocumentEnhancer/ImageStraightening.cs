using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentEnhancer;

public class ImageStraightening
{
      public static void StraightenImage(SBSDKImageRef imageRef)
      {
            var straighteningParameters = new SBSDKDocumentStraighteningParameters
            {
                  // Configure the straightening mode as needed
                  StraighteningMode = SBSDKDocumentStraighteningMode.Straighten,
                  // The straightening parameters can be customized to fit the expected aspect ratio of the document to be straightened.
                  // This can help the straightening algorithm to achieve better results.
                  AspectRatios =
                  [
                        new SBSDKAspectRatio(width: 5, height: 7),
                        new SBSDKAspectRatio(width: 1, height: 1),
                        new SBSDKAspectRatio(width: 16, height: 9),
                        new SBSDKAspectRatio(width: 3, height: 4)
                  ]
            };

            try
            {
                  // Initialize the document enhancer.
                  var enhancer = SBSDKDocumentEnhancer.CreateAndReturnError(out var documentEnhancerError).GetOrThrow(documentEnhancerError);

                  // Straighten the image using the document enhancer.
                  var result = enhancer.StraightenWithImage(image: imageRef,
                        parameters: straighteningParameters,
                        priorCornersNormalized: [],
                        error: out var error).GetOrThrow(error);

                  // Handle the document straightening result
                  var straightenedImage = result.StraightenedImage;
            }
            catch (Exception e)
            {
                  // handle the error thrown from the GetOrThrow(...) function, referenced by the PresentOn(...) error object.
                  Console.WriteLine(e);
            }
      }
}