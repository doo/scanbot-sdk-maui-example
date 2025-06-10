using ScanbotSdkExample.iOS.View;
using ScanbotSdkExample.iOS.Models;
using ScanbotSdkExample.iOS.Utils;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Controller
{
    public partial class MainViewController : UIViewController
    {
        private MainView _contentView;
        private List<ListItem> _documentScanners;
        private List<ListItem> _dataDetectors;
        private List<ListItem> _dataDetectionOnImage;
        private List<ListItem> _miscellaneousItems;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _contentView = new MainView();
            View = _contentView;

            Title = "Scanbot SDK RTU UI Example";

            _documentScanners =
            [
                new ListItem("Single Document Scanning", SingleDocumentScanning),
                new ListItem("Single Finder Document Scanning", SingleFinderDocumentScanning),
                new ListItem("Multiple Document Scanning", MultipleDocumentScanning),
                new ListItem("Create Document From Image", CreateDocFromImage),
                new ListItem("Classic Document Scanner View", ClassicDocumentScannerView)
            ];

            _dataDetectors =
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

            _dataDetectionOnImage =
            [
                new ListItem("Detect Check From Image", DetectCheck),
                new ListItem("Detect Credit Card From Image", DetectCreditCard),
                new ListItem("Extract Document Data From Image", ExtractDocumentData),
                new ListItem("Detect EHIC From Image", DetectEhic),
                new ListItem("Detect Medical Certificate From Image", DetectMedicalCertificate),
                new ListItem("Detect MRZ From Image", DetectMrz),
            ];
            
            _miscellaneousItems =
            [
                new ListItem("View License Info", DisplayLicenseInfo)
            ];

            _contentView.AddContent("Document Scanner", _documentScanners);
            _contentView.AddContent("Data Detectors", _dataDetectors);
            _contentView.AddContent("Data Detection On Image", _dataDetectionOnImage);
            _contentView.AddContent("Miscellaneous", _miscellaneousItems);

            _contentView.LicenseIndicator.Text = Texts.NoLicenseFoundTheAppWillTerminateAfterOneMinute;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                _contentView.LayoutSubviews();
            }

            foreach (var button in _contentView.AllButtons)
            {
                button.Click += OnScannerButtonClick;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            foreach (var button in _contentView.AllButtons)
            {
                button.Click -= OnScannerButtonClick;
            }
        }

        private void OnScannerButtonClick(object sender, EventArgs e)
        {
            if (!ScanbotSDKGlobal.IsLicenseValid)
            {
                _contentView.LayoutSubviews();
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
        
        private void DisplayLicenseInfo()
        {
            var message = "License Valid: " + (ScanbotSDKGlobal.IsLicenseValid ? "Yes" : "No");
            message += "\nLicense status: " + ScanbotSDKGlobal.LicenseStatus;
            
            Alert.Show(this, "License Info", message);
        }
    }
}