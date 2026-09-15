using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Common;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSDK.MAUI.Core.Check;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.ClassicUI.Pages;

public partial class ClassicCheckScannerPage : ContentPage
{
      private const string Finder = "Finder", Flash = "Flash", Freeze = "Stop Scan", Unfreeze = "Start Scan", Visibility = "Visibility";
      private FinderConfiguration _finderConfiguration;
      
      public ClassicCheckScannerPage()
      {
            InitializeComponent();

            // CollectionView Buttons
            ScannerButtons =
            [
                  new(Finder, () => IsFinderEnabled = !IsFinderEnabled),
                  new(Flash, () => IsFlashEnabled = !IsFlashEnabled),
                  new(Visibility, () => IsCameraVisible = !IsCameraVisible),
                  new(Freeze, null, true)
            ];

            BindingContext = this;

            CheckView.KeepScreenOn = true;
            CheckView.CameraPreviewMode = CameraPreviewMode.FitIn;
            CheckView.FinderConfiguration = new FinderConfiguration
            {
                  AspectRatio = new AspectRatio(5, 1),
                  LineColor = Colors.BlueViolet,
                  LineWidth = 3.5f,
                  OverlayColor = Colors.DarkSlateBlue.WithAlpha(0.5f),
                  MinimumPadding = 30,
            };
            
            CheckView.ScannerConfiguration = new CheckScannerConfiguration
            {
                  // ReturnCrops = true,
                  // EnableDetection = true,
                  ProcessingMode = ProcessingMode.SingleShot
            };
            
      }

      private bool _isFlashEnabled;
      public bool IsFlashEnabled
      {
            get => _isFlashEnabled;
            set
            {
                  _isFlashEnabled = value;
                  OnPropertyChanged();
            }
      }

      private bool _isFinderEnabled;
      public bool IsFinderEnabled
      {
            get => _isFinderEnabled;
            set
            {
                  _isFinderEnabled = value;
                  OnPropertyChanged();
                  UpdateFinderConfig(value);
            }
      }

      private bool _isCameraVisible = true;
      public bool IsCameraVisible
      {
            get => _isCameraVisible;
            set
            {
                  _isCameraVisible = value;
                  OnPropertyChanged();
            }
      }

      private List<ClassicCollectionItem> _scannerButtons = new List<ClassicCollectionItem>();
      public List<ClassicCollectionItem> ScannerButtons
      {
            get => _scannerButtons;
            set
            {
                  _scannerButtons = value;
                  OnPropertyChanged();
            }
      }
      
      private void UpdateFinderConfig(bool isFinderEnabled)
      {
            _finderConfiguration = new FinderConfiguration
            {
                  AspectRatio = new AspectRatio(5, 1),
                  LineColor = Colors.BlueViolet,
                  LineWidth = 3.5f,
                  OverlayColor = Colors.DarkSlateBlue.WithAlpha(0.5f),
                  MinimumPadding = 90,
                  Enabled = isFinderEnabled
            };
            CheckView.FinderConfiguration = _finderConfiguration;
      }

      private void ScannerButtonOnClicked(object sender, EventArgs e)
      {
            var selectedItem = (sender as Button)?.BindingContext as ClassicCollectionItem;
            if (selectedItem == null) return;

            selectedItem.ClickAction?.Invoke();
            selectedItem.Selected = !selectedItem.Selected;

            if (selectedItem.Title != Freeze && selectedItem.Title != Unfreeze)
                  return;

            // Start and Stop Toggle.
            if (selectedItem.Selected)
            {
                  selectedItem.Title = Freeze;
                  CheckView.IsCameraFrozen = false;
            }
            else
            {
                  selectedItem.Title = Unfreeze;
                  CheckView.IsCameraFrozen = true;
            }
      }

      private async void OnCheckScanningResult(object sender, CheckScanningResult e)
      {
            if (e.Status != CheckMagneticInkStripScanningStatus.Success) return;
            
            CheckView.IsCameraFrozen = true;
            if (await Alert.ShowAsync("Check Result", StringUtils.GenericDocumentToString(e.Check), "Retry", "Cancel"))
                  CheckView.IsCameraFrozen = false;
      }
}