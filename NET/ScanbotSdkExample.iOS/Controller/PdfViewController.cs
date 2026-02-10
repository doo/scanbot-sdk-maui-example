using ScanbotSdkExample.iOS.View;

namespace ScanbotSdkExample.iOS.Controller;

public sealed class PdfViewController : UIViewController
{
    private PdfContainerView ContentView { get; set; }
    private readonly NSUrl _uri;
    private readonly bool _ocr;
    readonly string _ocrResult;

    public PdfViewController(NSUrl uri, bool ocr, string ocrResult)
    {
        _uri = uri;
        _ocr = ocr;
        _ocrResult = ocrResult;
        ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        ContentView = new PdfContainerView(_uri, _ocr);
        View = ContentView;
        Title = _uri.LastPathComponent;
    }

    public override void ViewDidAppear(bool animated)
    {
        base.ViewDidAppear(animated);
        if (!string.IsNullOrEmpty(_ocrResult))
        {
            Utils.Alert.Show(title: "Info", body: _ocrResult);
        }
    }
}