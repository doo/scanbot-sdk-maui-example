using ReadyToUseUI.iOS.Repository;
using ReadyToUseUI.iOS.Utils;
using ReadyToUseUI.iOS.View;
using ReadyToUseUI.iOS.View.Collection;
using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;
using DocumentSDK.MAUI.Native.iOS.Configurations;
using ScanbotSDK.MAUI.Constants;
using SBSDK = ScanbotSDK.MAUI.Native.iOS.ScanbotSDK;
using ScanbotSDK.MAUI.Models;

namespace ReadyToUseUI.iOS.Controller
{
    public class ImageListController : UIViewController
    {
        public ImageCollectionView ContentView { get; private set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new ImageCollectionView();
            View = ContentView;

            Title = "Scanned documents";

            LoadPages();

            var saveButton = new UIBarButtonItem(Texts.save, UIBarButtonItemStyle.Done, OnSaveButtonClick);
            NavigationItem.SetRightBarButtonItem(saveButton, false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            LoadPages();

            ContentView.Collection.Selected += OnImageSelected;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            ContentView.Collection.Selected -= OnImageSelected;
        }

        void LoadPages()
        {
            ContentView.Collection.Pages.Clear();
            ContentView.Collection.Pages.AddRange(PageRepository.Items);
            ContentView.Collection.ReloadData();
        }

        void OnImageSelected(object sender, CollectionEventArgs e)
        {
            PageRepository.Current = e.Page;

            var controller = new ProcessingController();
            NavigationController.PushViewController(controller, true);
        }

        public SBSDKIndexedImageStorage CreateStorage(NSUrl[] uris)
        {
            var url = SBSDKStorageLocation.ApplicationSupportFolderURL;
            var tmp = NSUrl.FromFilename(string.Format("{0}/{1}", url.Scheme == "file" ? url.Path : url.AbsoluteString, Guid.NewGuid()));
            var location = new SBSDKStorageLocation(tmp);
            var format = SBSDKImageFileFormat.Jpeg;

            return new SBSDKIndexedImageStorage(location, format, ScanbotSDKUI.DefaultImageStoreEncrypter, uris);
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            if (PageRepository.DocumentImageURLs.Length == 0)
                return;

            var input = PageRepository.DocumentImageURLs;

            var docs = NSSearchPathDirectory.DocumentDirectory;
            var nsurl = NSFileManager.DefaultManager.GetUrls(docs, NSSearchPathDomain.User)[0];

            var controller = UIAlertController.Create(Texts.save, Texts.SaveHow, UIAlertControllerStyle.ActionSheet);

            var title = "Oops!";
            var body = "Something went wrong with saving your file. Please try again";

            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                title = "Oops";
                body = "Your license has expired";
                Alert.Show(this, title, body);
                return;
            }

            var pdf = CreateButton(Texts.save_without_ocr, async delegate
            {
                var output = new NSUrl(nsurl.AbsoluteString + Guid.NewGuid() + ".pdf");
                var createPDFConfig = new CreatePDFConfiguration
                {
                    InputImageUrls = input,
                    OutputUrl = output,
                    PageSize = PDFPageSize.A4
                };

                await SBSDK.CreatePDF(createPDFConfig);
                OpenDocument(output, false);
            });

            var ocr = CreateButton(Texts.save_with_ocr, async delegate
            {
                var output = new NSUrl(nsurl.AbsoluteString + Guid.NewGuid() + ".pdf");
                try
                {
                    var ocrConfiguration = new OCRDetectionConfiguration
                    {
                        RecognitionMode = ScanbotSDK.iOS.SBSDKOpticalCharacterRecognitionMode.Legacy,
                        // Languages property is only used in SBSDKOpticalCharacterRecognitionMode.Legacy
                        Langauges = SBSDK.InstalledLanguages,
                        InputImageUrls = input,
                        ShouldGeneratePDF = true,
                        OutputUrl = output,
                        PageSize = PDFPageSize.A4,
                    };
                    var result = await SBSDK.PerformOCR(ocrConfiguration);

                    OpenDocument(output, true, result.RecognizedText);
                }
                catch (Exception ex)
                {
                    body = ex.Message;
                    Alert.Show(this, title, body);
                }
            });

            var tiff = CreateButton(Texts.Tiff, delegate
            {
                var output = new NSUrl(nsurl.AbsoluteString + Guid.NewGuid() + ".tiff");

                // Please note that some compression types are only compatible for 1-bit encoded images (binarized black & white images)!
                var options = new TiffOptions { OneBitEncoded = true, Compression = TiffCompressionOptions.CompressionCcittfax4, Dpi = 250 };

                bool success = SBSDK.WriteTiff(input, output, options);
                if (success)
                {
                    title = "Info";
                    body = "TIFF file saved to: " + output.Path;
                }

                Alert.Show(this, title, body);
            });

            var cancel = CreateButton("Cancel", delegate { }, UIAlertActionStyle.Cancel);

            controller.AddAction(pdf);
            controller.AddAction(ocr);
            controller.AddAction(tiff);

            controller.AddAction(cancel);

            UIPopoverPresentationController presentationPopover = controller.PopoverPresentationController;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = View;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }

            PresentViewController(controller, true, null);
        }

        void OpenDocument(NSUrl uri, bool ocr, string ocrResult = null)
        {
            var controller = new PdfViewController(uri, ocr, ocrResult);
            NavigationController.PushViewController(controller, true);
        }

        UIAlertAction CreateButton(string text, Action<UIAlertAction> action,
            UIAlertActionStyle style = UIAlertActionStyle.Default)
        {
            return UIAlertAction.Create(text, style, action);
        }
    }
}
