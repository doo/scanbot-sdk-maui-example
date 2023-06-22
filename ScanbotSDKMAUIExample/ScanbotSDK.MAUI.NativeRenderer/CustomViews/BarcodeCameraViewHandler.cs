using System;
using Microsoft.Maui.Handlers;
using static Android.Provider.MediaStore;

namespace ScanbotSDK.MAUI.NativeRenderer.CustomViews
{
    public partial class BarcodeCameraViewHandler
    {
        public static PropertyMapper<BarcodeCameraView, BarcodeCameraViewHandler> PropertyMapper = new PropertyMapper<BarcodeCameraView, BarcodeCameraViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(BarcodeCameraView.OverlayConfiguration)] = MapOverlayConfiguration,
            [nameof(BarcodeCameraView.IsFlashEnabled)] = MapIsFlashEnabled
        };

        public static CommandMapper<BarcodeCameraView, BarcodeCameraViewHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(BarcodeCameraView.StartDetectionHandler)] = MapStartDetectionHandler,
            [nameof(BarcodeCameraView.StopDetectionHandler)] = MapStopDetectionHandler,
            [nameof(BarcodeCameraView.OnResumeHandler)] = MapOnResumeHandler,
            [nameof(BarcodeCameraView.OnPauseHandler)] = MapOnPauseHandler
        };

        public BarcodeCameraViewHandler() : base(PropertyMapper, CommandMapper)
        {

        }
    }
}