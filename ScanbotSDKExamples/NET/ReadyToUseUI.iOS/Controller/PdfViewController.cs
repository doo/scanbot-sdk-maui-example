using UIKit;
using Foundation;
using ReadyToUseUI.iOS.View;

namespace ReadyToUseUI.iOS.Controller
{
    public class PdfViewController : UIViewController
    {
        public PdfContainerView ContentView { get; set; }

        NSUrl uri;
        bool ocr;

        public PdfViewController(NSUrl uri, bool ocr)
        {
            this.uri = uri;
            this.ocr = ocr;
            
            ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new PdfContainerView(uri, ocr);
            View = ContentView;

            Title = uri.LastPathComponent;
        }
    }
}
