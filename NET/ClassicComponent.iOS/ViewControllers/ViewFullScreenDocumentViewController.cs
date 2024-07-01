namespace ClassicComponent.iOS.ViewControllers;

[Register ("ViewFullScreenDocumentViewController")]
public class ViewFullScreenDocumentViewController : UIViewController {

	UIImage documentImage;
    UIImageView imageView;
    public ViewFullScreenDocumentViewController(UIImage image)
	{
        this.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
		documentImage = image;
    }

	public override void ViewDidLoad ()
	{
        base.ViewDidLoad();
		imageView = new UIImageView(View.Frame);
		imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
		imageView.Image = documentImage;
        View = imageView;

        SetTapGestureToDocumentImage();
    }

    private void SetTapGestureToDocumentImage()
    {
        imageView.UserInteractionEnabled = true;
        imageView.AddGestureRecognizer(new UITapGestureRecognizer(() => this.DismissViewController(true, null)));
    }
}

