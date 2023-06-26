using BarcodeSDK.MAUI.Models;
using DocumentSDK.MAUI.iOS;
using Microsoft.Maui.Handlers;
using ScanbotSDK.iOS;
using ScanbotSDK.MAUI.NativeRenderer.Platforms.iOS;
using ScanbotSDK.MAUI.NativeRenderer.Platforms.iOS.CustomViews;
using UIKit;

namespace ScanbotSDK.MAUI.NativeRenderer.CustomViews
{
    public partial class BarcodeCameraViewHandler : ViewHandler<BarcodeCameraView, BarcodeCameraView_iOS>
    {
        BarcodeCameraView_iOS cameraView_iOS;

        #region Handler Overrides

        protected override BarcodeCameraView_iOS CreatePlatformView() => new BarcodeCameraView_iOS(ViewController);

        protected override void ConnectHandler(BarcodeCameraView_iOS platformView)
        {
            base.ConnectHandler(platformView);
            cameraView_iOS = platformView;
            platformView.ConnectHandler(VirtualView, this);
            var container = VirtualView.Handler.ContainerView;
        }

        protected override void DisconnectHandler(BarcodeCameraView_iOS platformView)
        {
            base.DisconnectHandler(platformView);
        }

        protected override void SetupContainer()
        {
            base.SetupContainer();
        }

        protected override void RemoveContainer()
        {
            base.RemoveContainer();
        }


        #endregion

        #region Properties Implementation

        public static void MapOverlayConfiguration(BarcodeCameraViewHandler current, BarcodeCameraView commonView)
        {
            current.cameraView_iOS.MapOverlayConfiguration(commonView);
        }

        public static void MapIsFlashEnabled(BarcodeCameraViewHandler current, BarcodeCameraView commonView)
        {
            current.cameraView_iOS.MapIsFlashEnabled(commonView.IsFlashEnabled);
        }

        #endregion

        #region Event Handlers Implementation

        public static void MapStartDetectionHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
            current.cameraView_iOS.MapStartDetectionHandler();
        }

        public static void MapStopDetectionHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
            current.cameraView_iOS.MapStopDetectionHandler();
        }

        public static void MapOnPauseHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
          
        }

        public static void MapOnResumeHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
          
        }

        private void CheckPermissions()
        {
          
        }

        #endregion

        #region Support Methods

        

        #endregion
    }
}

