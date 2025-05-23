using _Microsoft.Android.Resource.Designer;
using Android.Views;
using Android.Content;
using Android.Runtime;
using ScanbotSdkExample.Droid.Utils;
using ScanbotSdkExample.Droid.Model;
using ScanbotSdkExample.Droid.Activities;

namespace ScanbotSdkExample.Droid
{
    [Activity(Label = "SBSDK Droid", MainLauncher = true, Icon = "@mipmap/icon")]
    public partial class MainActivity : AndroidX.AppCompat.App.AppCompatActivity
    {
        private const int ScanDocumentRequestCode = 1000;
        private const int ImportImageRequestCode = 2001;
        private const int ScanMrzRequestCode = 4001;
        private const int ExtractDocumentDataRequestCode = 4002;
        private const int ScanDataRequestCode = 4003;
        private const int ScanVinRequestCode = 4004;
        private const int ScanCreditCardRequestCode = 4005;
        private const int ScanEhicRequestCode = 4006;
        private const int ScanMedicalCertificateRequestCode = 4007;
        private const int ScanCheckRequestCode = 4008;
        private const int DetectMrzFromImageCode = 6001;
        private const int DetectEhicFromImageCode = 6002;
        private const int DetectMedicalCertificateFromImageCode = 6003;
        private const int DetectCheckFromImageCode = 6004;
        private const int DetectDocumentDataFromImageCode = 6005;
        private const int DetectCreditCardFromImageCode = 6006;

        private readonly List<ListItemButton> _buttons = new List<ListItemButton>();
        private IO.Scanbot.Sdk.ScanbotSDK _scanbotSdk;
        private ProgressBar _progress;
        private TextView _licenseIndicator;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(ResourceConstant.Layout.activity_main);

            _scanbotSdk = new IO.Scanbot.Sdk.ScanbotSDK(this);
            // pageFileStorage = scanbotSDK.CreatePageFileStorage();

            var container = (LinearLayout)FindViewById(ResourceConstant.Id.container)!;
            var title = container.FindViewById<TextView>(ResourceConstant.Id.textView)!;
            title.Text = Texts.scanbot_sdk_demo;
            
            var scanner = (LinearLayout)container.FindViewById(ResourceConstant.Id.document_scanner)!;
            var scannerTitle = (TextView)scanner.FindViewById(ResourceConstant.Id.textView)!;
            scannerTitle.Text = "Document Scanner";
            scanner.AddChildren(_buttons, [
                new ListItemButton(this, "Single Document Scanning", SingleDocumentScanning),
                new ListItemButton(this, "Single Finder Document Scanning", SingleFinderDocumentScanning),
                new ListItemButton(this, "Multiple Document Scanning", MultipleDocumentScanning),
                new ListItemButton(this, "Import Image", ImportImage),
                new ListItemButton(this, "Classic Document Scanner View", ClassicDocumentScannerView),
            ]);

            var detectors = (LinearLayout)container.FindViewById(ResourceConstant.Id.data_detectors)!;
            var detectorsTitle = (TextView)detectors.FindViewById(ResourceConstant.Id.textView)!;
            detectorsTitle.Text = "Data Detectors";
            detectors.AddChildren(_buttons, [
                new ListItemButton(this, "Scan Check", ScanCheck),
                new ListItemButton(this, "Scan Credit Card", ScanCreditCard),
                new ListItemButton(this, "Scan Document Data", ExtractDocumentData),
                new ListItemButton(this, "Scan EU Health Insurance Card", ScanEhic),
                new ListItemButton(this, "Scan Medical Certificate", ScanMedicalCertificate),
                new ListItemButton(this, "Scan MRZ", ScanMrz),
                new ListItemButton(this, "Scan Text Pattern", TextDataRecognizerTapped),
                new ListItemButton(this, "Scan VIN", VinRecognizerTapped),
            ]);

            var detectionOnImage = (LinearLayout)container.FindViewById(ResourceConstant.Id.data_detection_on_image)!;
            var detectionOnImageTitle = (TextView)detectionOnImage.FindViewById(ResourceConstant.Id.textView)!;
            detectionOnImageTitle.Text = "Data Detection On Image";
            detectionOnImage.AddChildren(_buttons, [
                new ListItemButton(this, "Detect Check on Image", () => LaunchImagePicker(DetectCheckFromImageCode)),
                new ListItemButton(this, "Detect Credit Card on Image", () => LaunchImagePicker(DetectCreditCardFromImageCode)),
                new ListItemButton(this, "Detect Document Data on Image", () => LaunchImagePicker(DetectDocumentDataFromImageCode)),
                new ListItemButton(this, "Detect EU Health Insurance Card on Image", () => LaunchImagePicker(DetectEhicFromImageCode)),
                new ListItemButton(this, "Detect Medical Certificate on Image", () => LaunchImagePicker(DetectMedicalCertificateFromImageCode)),
                new ListItemButton(this, "Detect MRZ on Image", () => LaunchImagePicker(DetectMrzFromImageCode))
            ]);
            
            var miscellaneousLayout = (LinearLayout)container.FindViewById(ResourceConstant.Id.miscellaneous_layout)!;
            var miscellaneousLayoutTitle = (TextView)miscellaneousLayout.FindViewById(ResourceConstant.Id.textView)!;
            miscellaneousLayoutTitle.Text = "Miscellaneous";
            miscellaneousLayout.AddChildren(_buttons, [
                new ListItemButton(this, "View License Info", DisplayLicenseInfo),
            ]);
            
            _progress = FindViewById<ProgressBar>(ResourceConstant.Id.progressBar)!;

            _licenseIndicator = container.FindViewById<TextView>(ResourceConstant.Id.licenseIndicator)!;
            _licenseIndicator.Text = Texts.no_license_found_the_app_will_terminate_after_one_minute;
            _licenseIndicator.Visibility = string.IsNullOrEmpty(MainApplication.LicenseKey) ? ViewStates.Visible : ViewStates.Gone;

            foreach (var button in _buttons)
            {
                button.Click += OnButtonClick;
            }
        }

        private void DisplayLicenseInfo()
        {
            var message = "License Valid: " + (_scanbotSdk.LicenseInfo.IsValid ? "Yes" : "No");
            message += "\nLicense status: " + _scanbotSdk.LicenseInfo.Status.Name();
            
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

            if (!_scanbotSdk.LicenseInfo.IsValid)
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

            if (DetectOnImageActions.TryGetValue(requestCode, out var detectOnImageAction))
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
            if (_scanbotSdk.LicenseInfo.IsValid)
            {
                _licenseIndicator.Visibility = ViewStates.Gone;
            }
            else
            {
                _licenseIndicator.Visibility = ViewStates.Visible;
                _licenseIndicator.Text = _scanbotSdk.LicenseInfo.LicenseStatusMessage;
                Alert.Toast(this, "Invalid or missing license");
            }

            return _scanbotSdk.LicenseInfo.IsValid;
        }
    }
}