namespace ScanbotSdkExample.iOS.Controller;

public class ImageResultViewController : UIViewController
{
      private readonly UIImageView _imageView;

      public ImageResultViewController(UIImage image)
      {
            _imageView = new UIImageView(image);
            _imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
      }
      
      public override void ViewDidLoad()
      {
            View.AddSubview(_imageView);
            _imageView.Frame = View.Frame;
            View.BackgroundColor = UIColor.White;
      }
}     