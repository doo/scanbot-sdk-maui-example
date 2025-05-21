using ReadyToUseUI.iOS.View;
using ReadyToUseUI.iOS.Models;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller
{
    public partial class MainViewController : UIViewController
    {
        private MainView contentView;
        private List<ListItem> documentScanners;
        private List<ListItem> dataDetectors;
        private List<ListItem> dataDetectionOnImage;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            contentView = new MainView();
            View = contentView;

            Title = "Scanbot SDK RTU UI Example";

            documentScanners =
            [
                new ListItem("Single Document Scanning", SingleDocumentScanning),
                new ListItem("Single Finder Document Scanning", SingleFinderDocumentScanning),
                new ListItem("Multiple Document Scanning", MultipleDocumentScanning),
                new ListItem("Import Image", ImportImage)
            ];

            dataDetectors =
            [
                new ListItem("Scan Check", ScanCheck),
                new ListItem("Scan CreditCard", ScanCreditCard),
                new ListItem("Scan Document Data", ScanDocumentData),
                new ListItem("Scan EU Health Insurance Card", ScanEhic),
                new ListItem("Scan Medical Certificate", ScanMedicalCertificate),
                new ListItem("Scan MRZ", ScanMrz),
                new ListItem("Text Data Recognizer", ScanTextPattern),
                new ListItem("VIN Recognizer", ScanVin),
            ];

            dataDetectionOnImage =
            [
                new ListItem("Detect Check From Image", DetectCheck),
                new ListItem("Detect Credit Card From Image", DetectCreditCard),
                new ListItem("Detect Document Data From Image", DetectDocumentData),
                new ListItem("Detect EHIC From Image", DetectEhic),
                new ListItem("Detect Medical Certificate From Image", DetectMedicalCertificate),
                new ListItem("Detect MRZ From Image", DetectMrz),
            ];

            contentView.AddContent("Document Scanner", documentScanners);
            contentView.AddContent("Data Detectors", dataDetectors);
            contentView.AddContent("Data Detection On Image", dataDetectionOnImage);

            contentView.LicenseIndicator.Text = Texts.no_license_found_the_app_will_terminate_after_one_minute;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                contentView.LayoutSubviews();
            }

            foreach (var button in contentView.AllButtons)
            {
                button.Click += OnScannerButtonClick;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            foreach (var button in contentView.AllButtons)
            {
                button.Click -= OnScannerButtonClick;
            }
        }

        private void OnScannerButtonClick(object sender, EventArgs e)
        {
            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                contentView.LayoutSubviews();
                return;
            }

            if (sender is ScannerButton button)
            {
                button.Data.DoAction();
            }
        }

        private static bool IsPresented { get; set; }

        public static void ShowPopupWithAttributedText(UIViewController controller, NSAttributedString attributedString, Action onClose = null)
        {
            var popover = new PopupController(attributedString);
            ShowPopupInternal(controller, popover, onClose);
        }

        public static void ShowPopup(UIViewController controller, string text, Action onClose = null)
        {
            var images = new List<UIImage>();
            var popover = new PopupController(text, images);
            ShowPopupInternal(controller, popover, onClose);
        }

        private static void ShowPopupInternal(UIViewController controller, PopupController popover, Action onClose = null)
        {
            if (IsPresented)
            {
                return;
            }

            IsPresented = true;

            controller.PresentViewController(popover, true, delegate
            {
                popover.Content.CloseButton.Click += delegate
                {
                    IsPresented = false;
                    popover.Dismiss();
                    onClose?.Invoke();
                };
            });
        }
    }
}