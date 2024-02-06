using CoreGraphics;
using UIKit;

using ScanbotSDK.iOS;
using ScanbotSDK.MAUI.Models;
using ScanbotSDK.MAUI.iOS;
using ScanbotSDK.MAUI.iOS.Utils;

using ClassicComponent.Maui.CustomViews;
using ClassicComponent.Maui.Platforms.iOS.Utils;

namespace ClassicComponent.Maui.Platforms.iOS.CustomViews
{
    public class BarcodeCameraView_iOS : UIView
	{
        private BarcodeCameraViewHandler barcodeCameraViewHandler;
        private SBSDKBarcodeScannerViewController cameraViewController;
        public BarcodeCameraView_iOS(CGRect frame) : base(frame) { }

        internal async void ConnectHandler(BarcodeCameraViewHandler barcodeCameraViewHandler)
        {
            this.barcodeCameraViewHandler = barcodeCameraViewHandler;
            var visibleViewController = await ViewUtils.TryGetTopViewControllerAsync(barcodeCameraViewHandler?.PlatformView);
            if (visibleViewController != null)
            {
                cameraViewController = new SBSDKBarcodeScannerViewController(visibleViewController, barcodeCameraViewHandler.PlatformView);
                cameraViewController.Delegate = new BarcodeScannerDelegate(barcodeCameraViewHandler);
                cameraViewController.BarcodeImageGenerationType = SBSDKBarcodeImageGenerationType.None;
                SetConfigurations();
            }
        }

        internal void MapIsFlashEnabled(bool isFlashEnabled)
        {
            if (cameraViewController == null) return;
            cameraViewController.FlashLightEnabled = isFlashEnabled;
        }

        // -----------------------------------------
        // Selection Overlay Config binding
        // -----------------------------------------
        internal void MapOverlayConfiguration(BarcodeCameraView commonView)
        {
            if (cameraViewController == null) return;
            var config = commonView.OverlayConfiguration;
            if (config.Enabled == true)
            {
                cameraViewController.IsTrackingOverlayEnabled = true;

                cameraViewController.IsTrackingOverlayEnabled = true;
                cameraViewController.TrackingOverlayController.Configuration.IsAutomaticSelectionEnabled = config.AutomaticSelectionEnabled;
                cameraViewController.TrackingOverlayController.Configuration.PolygonStyle.PolygonColor = config.PolygonColor.ToNative();
                cameraViewController.TrackingOverlayController.Configuration.TextStyle.TextColor = config.TextColor.ToNative();
                cameraViewController.TrackingOverlayController.Configuration.TextStyle.TextBackgroundColor = config.TextContainerColor.ToNative();

                if (config.HighlightedPolygonColor != null)
                {
                    cameraViewController.TrackingOverlayController.Configuration.PolygonStyle.PolygonSelectedColor = config.HighlightedPolygonColor?.ToNative();
                }

                if (config.HighlightedTextColor != null)
                {
                    cameraViewController.TrackingOverlayController.Configuration.TextStyle.SelectedTextColor = config.HighlightedTextColor?.ToNative();
                }

                if (config.HighlightedTextContainerColor != null)
                {
                    cameraViewController.TrackingOverlayController.Configuration.TextStyle.TextBackgroundSelectedColor = config.HighlightedTextContainerColor?.ToNative();
                }
            }
        }

        internal void MapStartDetectionHandler()
        {
            if (cameraViewController == null) return;
            cameraViewController.RecognitionEnabled = true;
        }

        internal void MapStopDetectionHandler()
        {
            if (cameraViewController == null) return;
            cameraViewController.RecognitionEnabled = false;
        }

        /// <summary>
        /// Set the configuration again after the view is initialised.
        /// </summary>
        private void SetConfigurations()
        {
            MapIsFlashEnabled(barcodeCameraViewHandler.VirtualView.IsFlashEnabled);
            MapOverlayConfiguration(barcodeCameraViewHandler.VirtualView);
        }

        // Since we cannot directly inherit from SBSDKBarcodeScannerViewControllerDelegate in our ViewRenderer,
        // we have created this wrapper class to allow binding to its events through the use of delegates
        private class BarcodeScannerDelegate : SBSDKBarcodeScannerViewControllerDelegate
        {
            private BarcodeCameraViewHandler barcodeCameraViewHandler;
            private bool alertShown = false;

            public BarcodeScannerDelegate(BarcodeCameraViewHandler barcodeCameraViewHandler)
            {
                this.barcodeCameraViewHandler = barcodeCameraViewHandler;
            }

            public override void DidDetectBarcodes(SBSDKBarcodeScannerViewController controller, SBSDKBarcodeScannerResult[] codes)
            {
                codes = codes ?? Array.Empty<SBSDKBarcodeScannerResult>();
                barcodeCameraViewHandler.VirtualView.OnBarcodeScanResult(new BarcodeResultBundle()
                {
                    Barcodes = codes.Select(b => new Barcode
                    {
                        Text = b.RawTextString,
                        Format = b.Type.ToMaui(),
                        RawBytes = b.RawBytes.ToArray()
                    }).ToList()
                });
            }

            public override bool ShouldDetectBarcodes(SBSDKBarcodeScannerViewController controller)
            {
                if (ScanbotSDK.MAUI.ScanbotSDK.SDKService.IsLicenseValid)
                {
                    return true;
                }
                else
                {
                    if (!alertShown)
                    {
                        ViewUtils.ShowAlert("License Expired!", "Ok");
                        alertShown = true;
                    }

                    return false;
                }
            }
        }
    }
}

