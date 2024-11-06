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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            contentView = new MainView();
            View = contentView;

            Title = "Scanbot SDK RTU UI Example";

            documentScanners = new List<ListItem>
            {
                new ListItem("Single Document Scanning", SingleDocumentScanning),
                new ListItem("Single Finder Document Scanning", SingleFinderDocumentScanning),
                new ListItem("Multiple Document Scanning", MultipleDocumentScanning),
                new ListItem("Import Image", ImportImage)
            };

            dataDetectors = new List<ListItem>
            {
                new ListItem("Scan MRZ",                      ScanMrz),
                new ListItem("Scan Health Insurance card",    ScanEhic),
                new ListItem("Generic Document Recognizer",   RecongnizeGenericDocument),
                new ListItem("Check Recognizer",              RecognizeCheck),
                new ListItem("Text Data Recognizer",          TextDataRecognizerTapped),
                new ListItem("VIN Recognizer",                VinRecognizerTapped),
                new ListItem("License Plate Recognizer",      LicensePlateRecognizerTapped),
                new ListItem("Medical Certificate Recognizer", MedicalCertificateRecognizerTapped),
            };

            contentView.AddContent("Document Scanner", documentScanners);
            contentView.AddContent("Data Detectors", dataDetectors);

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

        public static void ShowPopup(UIViewController controller, string text, Action onClose = null)
        {
            if (IsPresented)
            {
                return;
            }

            IsPresented = true;

            var images = new List<UIImage>();
            var popover = new PopupController(text, images);

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