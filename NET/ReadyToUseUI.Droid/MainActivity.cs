using Android.Views;
using Android.Content;
using Android.Runtime;
using ReadyToUseUI.Droid.Utils;
using DocumentSDK.NET.Model;


namespace ReadyToUseUI.Droid
{
    [Activity(Label = "NET RTU UI", MainLauncher = true, Icon = "@mipmap/icon")]
    public partial class MainActivity : AndroidX.AppCompat.App.AppCompatActivity
    {
        private const int SCAN_DOCUMENT_REQUEST_CODE = 1000;
        private const int IMPORT_IMAGE_REQUEST = 2001;

        private const int SCAN_MRZ_REQUEST = 4001;
        private const int EXTRACT_DOCUMENT_DATA_REQUEST = 4002;
        private const int SCAN_DATA_REQUEST = 4003;
        private const int SCAN_VIN_REQUEST = 4004;
        private const int SCAN_CREDIT_CARD_REQUEST = 4005;
        private const int SCAN_EHIC_REQUEST = 4006;
        private const int SCAN_MEDICAL_CERTIFICATE_REQUEST = 4007;
        private const int SCAN_CHECK_REQUEST = 4008;
        
        private const int DETECT_MRZ_FROM_IMAGE = 6001;
        private const int DETECT_EHIC_FROM_IMAGE = 6002;
        private const int DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE = 6003;
        private const int DETECT_CHECK_FROM_IMAGE = 6004;
        private const int DETECT_DOCUMENT_DATA_FROM_IMAGE = 6005;
        private const int DETECT_CREDIT_CARD_FROM_IMAGE = 6006;

        private readonly List<ListItemButton> buttons = new List<ListItemButton>();
        private IO.Scanbot.Sdk.ScanbotSDK scanbotSDK;
        private ProgressBar progress;
        private TextView licenseIndicator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            scanbotSDK = new IO.Scanbot.Sdk.ScanbotSDK(this);
            // pageFileStorage = scanbotSDK.CreatePageFileStorage();

            var container = (LinearLayout)FindViewById(Resource.Id.container);
            var title = container.FindViewById<TextView>(Resource.Id.textView);
            title.Text = Texts.scanbot_sdk_demo;
          
            var scanner = (LinearLayout)container.FindViewById(Resource.Id.document_scanner);
            var scannerTitle = (TextView)scanner.FindViewById(Resource.Id.textView);
            scannerTitle.Text = "Document Scanner";
            scanner.AddChildren(buttons, [
                new ListItemButton(this, "Single Document Scanning", SingleDocumentScanning),
                new ListItemButton(this, "Single Finder Document Scanning", SingleFinderDocumentScanning),
                new ListItemButton(this, "Multiple Document Scanning", MultipleDocumentScanning),
                new ListItemButton(this, "Import Image", ImportImage)
            ]);

            var detectors = (LinearLayout)container.FindViewById(Resource.Id.data_detectors);
            var detectorsTitle = (TextView)detectors.FindViewById(Resource.Id.textView);
            detectorsTitle.Text = "Data Detectors";
            detectors.AddChildren(buttons, [
                new ListItemButton(this, "Scan Check", ScanCheck),
                new ListItemButton(this, "Scan Credit Card", ScanCreditCard),
                new ListItemButton(this, "Scan Document Data", ExtractDocumentData),
                new ListItemButton(this, "Scan EU Health Insurance Card", ScanEhic),
                new ListItemButton(this, "Scan Medical Certificate", ScanMedicalCertificate),
                new ListItemButton(this, "Scan MRZ", ScanMrz),
                new ListItemButton(this, "Scan Text Pattern", TextDataRecognizerTapped),
                new ListItemButton(this, "Scan VIN", VinRecognizerTapped),
            ]);

            var detectionOnImage = (LinearLayout)container.FindViewById(Resource.Id.data_detection_on_image);
            var detectionOnImageTitle = (TextView)detectionOnImage.FindViewById(Resource.Id.textView);
            detectionOnImageTitle.Text = "Data Detection On Image";
            detectionOnImage.AddChildren(buttons, [
                new ListItemButton(this, "Detect Check on Image", () => LaunchImagePicker(DETECT_CHECK_FROM_IMAGE)),
                new ListItemButton(this, "Detect Credit Card on Image", () => LaunchImagePicker(DETECT_CREDIT_CARD_FROM_IMAGE)),
                new ListItemButton(this, "Detect Document Data on Image", () => LaunchImagePicker(DETECT_DOCUMENT_DATA_FROM_IMAGE)),
                new ListItemButton(this, "Detect EU Health Insurance Card on Image", () => LaunchImagePicker(DETECT_EHIC_FROM_IMAGE)),
                new ListItemButton(this, "Detect Medical Certificate on Image", () => LaunchImagePicker(DETECT_MEDICAL_CERTIFICATE_FROM_IMAGE)),
                new ListItemButton(this, "Detect MRZ on Image", () => LaunchImagePicker(DETECT_MRZ_FROM_IMAGE))
            ]);
            
            var miscellaneousLayout = (LinearLayout)container.FindViewById(Resource.Id.miscellaneous_layout);
            var miscellaneousLayoutTitle = (TextView)miscellaneousLayout.FindViewById(Resource.Id.textView);
            miscellaneousLayoutTitle.Text = "Miscellaneous";
            miscellaneousLayout.AddChildren(buttons, [
                new ListItemButton(this, "View License Info", DisplayLicenseInfo),
            ]);
            
            progress = FindViewById<ProgressBar>(Resource.Id.progressBar);

            licenseIndicator = container.FindViewById<TextView>(Resource.Id.licenseIndicator);
            licenseIndicator.Text = Texts.no_license_found_the_app_will_terminate_after_one_minute;
            licenseIndicator.Visibility = string.IsNullOrEmpty(MainApplication.LicenseKey) ? ViewStates.Visible : ViewStates.Gone;

            foreach (var button in buttons)
            {
                button.Click += OnButtonClick;
            }
        }

        private void DisplayLicenseInfo()
        {
            var message = "License Valid: " + (scanbotSDK.LicenseInfo.IsValid ? "Yes" : "No");
            message += "\nLicense status: " + scanbotSDK.LicenseInfo.Status.Name();
            
            Alert.ShowAlert(this, "License Info", message);
        }

        /**
         * Scanner returned, parse results
         */
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                return;
            }

            if (!scanbotSDK.LicenseInfo.IsValid)
            {
                Alert.ShowLicenseDialog(this);
                return;
            }

            if (documentScannerActions.TryGetValue(requestCode, out var documentScannerAction))
            {
                documentScannerAction(data);
            }

            if (dataDetectorActions.TryGetValue(requestCode, out var dataDetectorAction))
            {
                dataDetectorAction(data);
            }            

            if (detectOnImageActions.TryGetValue(requestCode, out var detectOnImageAction))
            {
                detectOnImageAction(data);
            }            
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (!CheckLicense())
            {
                return;
            }

            if (sender is ListItemButton button && button.DoAction != null)
            {
                button.DoAction();
            }
        }

        private bool CheckLicense()
        {
            if (scanbotSDK.LicenseInfo.IsValid)
            {
                licenseIndicator.Visibility = ViewStates.Gone;
            }
            else
            {
                licenseIndicator.Visibility = ViewStates.Visible;
                licenseIndicator.Text = scanbotSDK.LicenseInfo.LicenseStatusMessage;
                Alert.Toast(this, "Invalid or missing license");
            }

            return scanbotSDK.LicenseInfo.IsValid;
        }
    }
}