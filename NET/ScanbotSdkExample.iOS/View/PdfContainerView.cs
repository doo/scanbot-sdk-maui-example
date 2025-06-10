using PdfKit;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.View
{
    public sealed class PdfContainerView : UIView
    {
        private readonly UILabel _title;
        readonly PdfView _content;

        public PdfContainerView(NSUrl uri, bool ocr)
        {
            BackgroundColor = UIColor.White;

            _title = new UILabel();
            _title.TextAlignment = UITextAlignment.Center;
            _title.TextColor = Models.Colors.DarkGray;
            _title.Font = UIFont.FromName("HelveticaNeue-Bold", 13f);
            _title.Lines = 0;

            AddSubview(_title);

            _content = new PdfView();
            _content.DisplayMode = PdfDisplayMode.SinglePageContinuous;
            _content.AutoScales = true;

            if (uri.Path != null)
            {
                var data = NSData.FromFile(uri.Path);
                // If data is encrypted, SBSDK.Encrypter will be evaluated.
                // In that case, use it to decrypt the data
                if (ScanbotUI.DefaultImageStoreEncrypter != null)
                {
                    data = ScanbotUI.DefaultImageStoreEncrypter.DecryptData(data, "", out NSError error);
                }

                if (data != null)
                    _content.Document = new PdfDocument(data);
            }

            AddSubview(_content);

            if (ocr)
            {
                _title.Text =
                    "Good job! You created a sandwich .pdf.\n" +
                    "Go ahead, try to select part of the text of your saved file";
            }
            else
            {
                _title.Text =
                    "Good job! You saved a plain pdf.\n" +
                    "Try to select part of your text, you won't be able to";
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 5;

            float x = padding;
            float y = padding;
            float w = (float)Frame.Width - 2 * padding;
            float h = w / 5;

            _title.Frame = new CGRect(x, y, w, h);

            y += h + padding;
            h = (float)Frame.Height - (h + 3 * padding);

            _content.Frame = new CGRect(x, y, w, h);
        }
    }
}
