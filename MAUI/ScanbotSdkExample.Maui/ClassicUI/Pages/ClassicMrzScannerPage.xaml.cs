using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Common;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSDK.MAUI.Core.Mrz;
using ScanbotSdkExample.Maui.Models;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.ClassicUI.Pages;

public partial class ClassicMrzScannerPage : ContentPage
{
      private const string Finder = "Finder", Flash = "Flash", Freeze = "Stop Scan", Unfreeze = "Start Scan", Visibility = "Visibility";
      private FinderConfiguration _finderConfiguration;
      public ClassicMrzScannerPage()
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

            MrzView.KeepScreenOn = true;
            MrzView.CameraPreviewMode = CameraPreviewMode.FitIn;
            UpdateFinderConfig(IsFinderEnabled);
            
            MrzView.ScannerConfiguration = new MrzScannerConfiguration
            {
                  ReturnCrops = true,
                  EnableDetection = true,
                  ProcessingMode = ProcessingMode.SingleShot,
            };
            
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
            MrzView.FinderConfiguration = _finderConfiguration;
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
                  MrzView.IsCameraFrozen = false;
            }
            else
            {
                  selectedItem.Title = Unfreeze;
                  MrzView.IsCameraFrozen = true;
            }
      }

      private async void MrzView_OnMrzScannerResult(object sender, MrzScannerResult e)
      {
            if (!e.Success) return;
            MrzView.IsCameraFrozen = true;
            if (await Alert.ShowAsync("MRZ Result", e.RawMRZ, "Retry", "Cancel"))
                  MrzView.IsCameraFrozen = false;
      }
}