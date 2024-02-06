using Android;
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

using IO.Scanbot.Sdk.Barcode;
using IO.Scanbot.Sdk.Barcode.Entity;
using IO.Scanbot.Sdk.Barcode.UI;
using IO.Scanbot.Sdk.UI.Camera;

using ScanbotSDK.MAUI.Models;
using ScanbotSDK.MAUI.Droid;
using ScanbotSDK.MAUI.Droid.Utils;

namespace ClassicComponent.Maui.CustomViews
{
    public partial class BarcodeCameraViewHandler : ViewHandler<BarcodeCameraView, FrameLayout>
    {
        // Classical component
        protected BarcodeScannerView cameraViewDroid;
        private readonly int REQUEST_PERMISSION_CODE = 200;
        private bool toastShown = false;

        protected override FrameLayout CreatePlatformView()
        {
            var cameraLayout = (FrameLayout)LayoutInflater
                      .FromContext(Context)
                      .Inflate(Resource.Layout.barcode_camera_view, null, false);


            // Here we retrieve the Camera View...
            cameraViewDroid = cameraLayout.FindViewById<BarcodeScannerView>(Resource.Id.barcode_camera);
            InstallHierarchyFitter(cameraViewDroid);
            return cameraLayout;
        }

        private void OnCameraOpen()
        {
            cameraViewDroid.ViewController.UseFlash(VirtualView.IsFlashEnabled);
        }

        private void OnSelectionOverlayBarcodeClicked(IO.Scanbot.Sdk.Barcode.Entity.BarcodeItem barcodeItem)
        {
            var outResult = new BarcodeResultBundle
            {
                Barcodes = new List<Barcode>() { barcodeItem.ToMaui() },
                Image = barcodeItem.Image.ToImageSource()
            };

            VirtualView.OnBarcodeScanResult.Invoke(outResult);
        }

        protected override void ConnectHandler(FrameLayout platformView)
        {
            base.ConnectHandler(platformView);

            var detector = new IO.Scanbot.Sdk.ScanbotSDK(Context.GetActivity()).CreateBarcodeDetector();
            detector.ModifyConfig((response) =>
            {
                response.SetSaveCameraPreviewFrame(false);
            });

            cameraViewDroid.InitCamera(new CameraUiSettings(useCameraX: false));
            cameraViewDroid.InitDetectionBehavior(detector, HandleFrameHandlerResult,
                (onCameraOpen: OnCameraOpen,
                onPictureTaken: (_, _) => { },
                onSelectionOverlayBarcodeClicked: OnSelectionOverlayBarcodeClicked
            ));
        }

        protected override void DisconnectHandler(FrameLayout platformView)
        {
            base.DisconnectHandler(platformView);
        }

        protected override void RemoveContainer()
        {
            base.RemoveContainer();
        }

        protected override void SetupContainer()
        {
            base.SetupContainer();
        }

        public static void MapOverlayConfiguration(BarcodeCameraViewHandler current, BarcodeCameraView commonView)
        {
            current.SetSelectionOverlayConfiguration(commonView);
        }

        public static void MapIsFlashEnabled(BarcodeCameraViewHandler current, BarcodeCameraView commonView)
        {
            current.cameraViewDroid.ViewController.UseFlash(commonView.IsFlashEnabled);
        }

        public static void MapStartDetectionHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
            current?.CheckPermissions();
            current?.cameraViewDroid?.ViewController.StartPreview();
            current?.cameraViewDroid?.ViewController?.OnResume();
        }

        public static void MapOnPauseHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
            current.cameraViewDroid.ViewController.OnPause();
        }

        public static void MapOnResumeHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
            current.cameraViewDroid.ViewController.OnResume();
        }

        public static void MapStopDetectionHandler(BarcodeCameraViewHandler current, BarcodeCameraView commonView, object arg3)
        {
            current.cameraViewDroid.ViewController.StopPreview();
        }

        private void CheckPermissions()
        {
            if (Context == null || Context.GetActivity() == null)
            {
                return;
            }

            var activity = Context.GetActivity();

            if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.Camera) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(activity, new string[] { Manifest.Permission.Camera }, REQUEST_PERMISSION_CODE);
            }
        }

        public void SetSelectionOverlayConfiguration(BarcodeCameraView commonView)
        {
            if (commonView.OverlayConfiguration?.Enabled == true)
            {
                var config = commonView.OverlayConfiguration;
                cameraViewDroid.SelectionOverlayController.SetEnabled(config.Enabled);
                cameraViewDroid.SelectionOverlayController.SetBarcodeAppearanceDelegate(
                (getPolygonStyle: (defaultStyle, _) => defaultStyle.Copy(
                                                    fillColor: config.PolygonColor.ToPlatform(),
                                                    fillHighlightedColor: config.HighlightedPolygonColor?.ToPlatform()),
                getTextViewStyle: (defaultStyle, _) => defaultStyle.Copy(
                            textFormat: config.OverlayTextFormat.ToNative(),
                            textColor: config.TextColor.ToPlatform(),
                            textContainerColor: config.TextContainerColor.ToPlatform(),
                            textHighlightedColor: config.HighlightedTextColor?.ToPlatform(),
                            textContainerHighlightedColor: config.HighlightedTextContainerColor?.ToPlatform()
                            )));
            }
        }

        private bool HandleFrameHandlerResult(BarcodeScanningResult result, IO.Scanbot.Sdk.SdkLicenseError error)
        {
            if (!ScanbotSDK.MAUI.ScanbotSDK.SDKService.IsLicenseValid)
            {
                if (!toastShown)
                {
                    cameraViewDroid.Post(() => Toast.MakeText(Context.GetActivity(), "License has expired!", ToastLength.Long).Show());
                    toastShown = true;
                }

                return false;
            }

            if (result == null)
            {
                return false;
            }

            var overlayEnabled = VirtualView.OverlayConfiguration?.Enabled ?? false;
            if (overlayEnabled == false || VirtualView.OverlayConfiguration?.AutomaticSelectionEnabled == true)
            {
                var outResult = new BarcodeResultBundle
                {
                    Barcodes = result.BarcodeItems.Select(b => b.ToMaui()).ToList(),
                    Image = result.PreviewFrame.ToImageSource()
                };

                VirtualView.OnBarcodeScanResult?.Invoke(outResult);
            }
            return false;
        }

        private static void InstallHierarchyFitter(ViewGroup viewGroup)
        {
            viewGroup.SetOnHierarchyChangeListener(new HierarchyChangeListener(viewGroup));
        }
    }

    internal class HierarchyChangeListener : Java.Lang.Object, ViewGroup.IOnHierarchyChangeListener
    {
        private ViewGroup viewGroup;

        public HierarchyChangeListener(ViewGroup viewGroup)
        {
            this.viewGroup = viewGroup;
        }

        public void OnChildViewAdded(Android.Views.View parent, Android.Views.View child)
        {
            if (parent == null) { return; }
            parent.Measure(
                Android.Views.View.MeasureSpec.MakeMeasureSpec(viewGroup.MeasuredWidth, MeasureSpecMode.Exactly),
                Android.Views.View.MeasureSpec.MakeMeasureSpec(viewGroup.MeasuredHeight, MeasureSpecMode.Exactly)
            );
            parent.Layout(0, 0, parent.MeasuredWidth, parent.MeasuredHeight);
        }

        public void OnChildViewRemoved(Android.Views.View parent, Android.Views.View child)
        {

        }
    }
}