using ScanbotSDK.MAUI.Configurations;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.RTU.v1;

namespace ClassicComponent.Maui
{
    public partial class MainPage : ContentPage
    {
        private bool isDetectionOn;
        /// <summary>
        /// Is Barcode scanning is On or Off.
        /// </summary>
        private bool IsDetectionOn
        {
            get => isDetectionOn;
            set
            {
                isDetectionOn = value;
                MainThread.BeginInvokeOnMainThread(() => ToggleUIOnScanning(value));
            }
        }

        /// <summary>
        /// Get the License of the SDK.
        /// </summary>
        private bool IsLicenseValid => ScanbotSDK.MAUI.ScanbotSDK.SDKService.IsLicenseValid;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            SetupViews();
        }

        /// <summary>
        /// Set Up UI on initial launch
        /// </summary>
        private void SetupViews()
        {
            cameraView.OverlayConfiguration = new SelectionOverlayConfiguration(false, BarcodeTextFormat.CodeAndType,
                textColor: Colors.Yellow,
                textContainerColor: Colors.Black,
                strokeColor: Colors.Yellow,
                highlightedTextColor: Colors.Red,
                highlightedTextContainerColor: Colors.Black,
                highlightedStrokeColor: Colors.Red);
            cameraView.OnBarcodeScanResult = (result) =>
            {
                string text = "";
                foreach (ScanbotSDK.MAUI.RTU.v1.Barcode barcode in result.Barcodes)
                {
                    text += string.Format("{0} ({1})\n", barcode.Text, barcode.Format.ToString().ToUpper());
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    resultsLabel.Text = text;
                });
            };
            SetupIOSAppearance();
        }

        /// <summary>
        /// OnAppearing - Invoked on every Page forground
        /// </summary>
        async protected override void OnAppearing()
        {
            base.OnAppearing();

            scanButton.Clicked += OnScanButtonPressed;
            infoButton.Clicked += OnInfoButtonPressed;

            if (!await CameraPermissionAllowed())
            {
                IsDetectionOn = false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(App.LICENSE_KEY))
                {
                    await ShowTrialLicenseAlert();
                }

                ToggleUIOnScanning(IsDetectionOn); // Update UI with IsDetection flag status
            }
        }

        /// <summary>
        /// OnDisappearing - Invoked on every Page background
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            scanButton.Clicked -= OnScanButtonPressed;
            infoButton.Clicked -= OnInfoButtonPressed;
            if (IsDetectionOn)
            {
                cameraView.StopDetection();
            }
        }

        /// Scanning Button click event.
        /// Checks for Camera Permission, License and invokes scanning feature.
        private async void OnScanButtonPressed(object sender, EventArgs e)
        {
            if (!await CameraPermissionAllowed())
                return;

            if (!IsLicenseValid)
            {
                IsDetectionOn = false;
                ShowExpiredLicenseAlert();
                return;
            }

            IsDetectionOn = !IsDetectionOn;
        }

        private void OnInfoButtonPressed(object sender, EventArgs e)
        {
            DisplayAlert("Info", IsLicenseValid ? "Your SDK License is valid." : "Your SDK License has expired.", "Close");
        }

        private void ShowExpiredLicenseAlert()
        {
            DisplayAlert("Error", "Your SDK license has expired", "Close");
        }

        async private Task ShowTrialLicenseAlert()
        {
            await DisplayAlert("Welcome", "You are using the Trial SDK License. The SDK will be active for one minute.", "Close");
        }

        private void SetupIOSAppearance()
        {
            if (DeviceInfo.Platform != DevicePlatform.iOS) { return; }

            var safeInsets = On<iOS>().SafeAreaInsets();
            safeInsets.Bottom = 0;
            Padding = safeInsets;
        }

        // Updates UI when the Barcode scanning is turned on/off.
        private void ToggleUIOnScanning(bool isDetectionOn)
        {
            if (isDetectionOn)
            {
                cameraView.StartDetection();
                scanButton.Text = "STOP SCANNING";
                resultsPreviewLayout.BackgroundColor = Colors.White;
            }
            else
            {
                scanButton.Text = "START SCANNING";
                resultsPreviewLayout.BackgroundColor = Color.FromArgb("#d2d2d2");
                cameraView.StopDetection();
                ToggleFlash(false);
            }
        }

        /// Check for Camera permissions.
        private async Task<bool> CameraPermissionAllowed()
        {
            var isAllowed = false;
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            switch (status)
            {
                case PermissionStatus.Unknown:  // Defult permission status For iOS is Unknown
                    {
                        await Permissions.RequestAsync<Permissions.Camera>();
                        isAllowed = await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Granted;
                    }
                    break;

                case PermissionStatus.Denied: // Default permission status for Android is Denied
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        status = await Permissions.RequestAsync<Permissions.Camera>();
                        isAllowed = await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Granted;
                    }

                    if (!isAllowed)
                    {
                        await DisplayAlert("Alert", "Please turn on the camera permissions from setting screen, to turn on scanning feature.", "Ok");
                    }
                    break;

                case PermissionStatus.Disabled:
                    break;

                case PermissionStatus.Granted:
                    isAllowed = true;
                    break;

                case PermissionStatus.Restricted:
                    break;
            }
            return isAllowed;
        }

        // ImageButton element clicked event. Handles Flash Light on/off.
        void FlashButtonClicked(System.Object sender, System.EventArgs e)
        {
            if (!IsDetectionOn)
                return;

            ToggleFlash(!cameraView.IsFlashEnabled);
        }

        /// <summary>
        /// Toggles the Flash button UI relecting On/Off and the camera view IsFlashEnabled property.
        /// </summary>
        /// <param name="isOn">Bool flag, if set to 'true' turns on the flash and 'false' to turn off the flash.</param>
        private void ToggleFlash(bool isOn)
        {
            cameraView.IsFlashEnabled = isOn;
            if (isOn)
            {
                imgButtonFlash.BackgroundColor = Colors.Yellow;
            }
            else
            {
                imgButtonFlash.BackgroundColor = Colors.Transparent;
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
        }
    }
}