using ReadyToUseUI.iOS.View;

namespace ReadyToUseUI.iOS.Controller
{
    public class PdfViewController : UIViewController
    {
        public PdfContainerView ContentView { get; set; }
        NSUrl uri;
        bool ocr;
        string ocrResult;

        public PdfViewController(NSUrl uri, bool ocr, string ocrResult)
        {
            this.uri = uri;
            this.ocr = ocr;
            this.ocrResult = ocrResult;
            ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new PdfContainerView(uri, ocr);
            View = ContentView;

            Title = uri.LastPathComponent;
            
            
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!string.IsNullOrEmpty(ocrResult))
            {
                Utils.Alert.Show(this, title: "Info", body: ocrResult);
            }
        }
    }
}
