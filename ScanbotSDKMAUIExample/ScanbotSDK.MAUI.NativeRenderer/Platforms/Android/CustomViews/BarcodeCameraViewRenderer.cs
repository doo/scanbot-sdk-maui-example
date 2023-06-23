using System;
using System.ComponentModel;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using BarcodeSDK.MAUI.Models;
using DocumentSDK.MAUI.Droid;
using DocumentSDK.MAUI.Droid.Utils;
using IO.Scanbot.Sdk;
using IO.Scanbot.Sdk.Barcode;
using IO.Scanbot.Sdk.Barcode.Entity;
using IO.Scanbot.Sdk.Barcode.UI;
using IO.Scanbot.Sdk.Camera;
using IO.Scanbot.Sdk.Contourdetector;
using IO.Scanbot.Sdk.UI;
using IO.Scanbot.Sdk.UI.Camera;
using Java.Interop;
using Kotlin.Jvm.Functions;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using ScanbotSDK.MAUI.NativeRenderer.CustomViews;
using static ScanbotSDK.MAUI.NativeRenderer.Platforms.Android.CustomViews.BarcodeDetectorResultHandler;

namespace ScanbotSDK.MAUI.NativeRenderer.Platforms.Android.CustomViews
{
    /*
       By extending 'ViewRenderer' we specify that we want our custom renderer to target 'BarcodeCameraView' and
       override it with our native view, which is a 'FrameLayout' in this case (see layout/barcode_camera_view.xml)
    */
    class AndroidBarcodeCameraRenderer : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<BarcodeCameraView, FrameLayout>, IBarcodeScannerViewCallback //  ICameraOpenCallback
    {
        protected BarcodeCameraView.BarcodeScannerResultHandler HandleScanResult;
        //protected DocumentAutoSnappingController autoSnappingController;
        protected BarcodeDetectorFrameHandler barcodeDetectorFrameHandler;
        protected FrameLayout cameraLayout;
        // Classical component
        protected BarcodeScannerView cameraView;
        private readonly int REQUEST_PERMISSION_CODE = 200;

        public AndroidBarcodeCameraRenderer(Context context) : base(context)
        {
            SetupViews(context);
        }

        private void SetupViews(Context context)
        {

            // We instantiate our views from the layout XML
            cameraLayout = (FrameLayout)LayoutInflater
                .FromContext(context)
                .Inflate(Resource.Layout.barcode_camera_view, null, false);

            // Here we retrieve the Camera View...
            cameraView = cameraLayout.FindViewById<BarcodeScannerView>(Resource.Id.barcode_camera);
        }

        /*
            This is probably the most important method that belongs to a ViewRenderer.
            You must override this in order to actually implement the renderer.
            OnElementChanged is called whenever the View or one of its properties have changed;
            this includes the initialization as well, therefore we initialize our native control here.
         */
        protected override void OnElementChanged(ElementChangedEventArgs<BarcodeCameraView> e)
        {
            // The SetNativeControl method should be used to instantiate the native control,
            // and this method will also assign the control reference to the Control property
            SetNativeControl(cameraLayout);
            base.OnElementChanged(e);
            if (Control != null)
            {
                // The Element object is the instance of BarcodeCameraView as defined in the Forms
                // core project. We've defined some delegates there, and we'll bind to them here so that
                // these native calls will be executed whenever those methods will be called.
                Element.OnResumeHandler = (sender, e) =>
                {
                    cameraView.ViewController.OnResume();
                };

                Element.OnPauseHandler = (sender, e) =>
                {
                    cameraView.ViewController.OnPause();
                };

                Element.StartDetectionHandler = (sender, e) =>
                {
                    StartDetection();
                };

                Element.StopDetectionHandler = (sender, e) =>
                {
                    StopDetection();
                };

                // Similarly, we have defined a delegate in our BarcodeCameraView implementation,
                // so that we can trigger it whenever the Scanner will return a valid result.
                HandleScanResult = Element.OnBarcodeScanResult;

                // In this example we demonstrate how to lock the orientation of the UI (Activity)
                // as well as the orientation of the taken picture to portrait.
                //cameraView.LockToPortrait(true);

                // Here we create the BarcodeDetectorFrameHandler which will take care of detecting
                // barcodes in your video frames
                var detector = new IO.Scanbot.Sdk.ScanbotSDK(Context.GetActivity()).CreateBarcodeDetector();
                detector.ModifyConfig(new Function1Impl<IO.Scanbot.Sdk.Barcode.Entity.BarcodeScannerConfigBuilder>((response) =>
                {
                    response.SetSaveCameraPreviewFrame(false);
                }));

                cameraView.InitCamera(new CameraUiSettings(false));

                BarcodeScannerViewWrapper.InitDetectionBehavior(cameraView, detector, new BarcodeDetectorResultHandler(HandleFrameHandlerResult), null);

                SetSelectionOverlayConfiguration();
                // TODO: Need to fix this from bindings
                //cameraView?.FinderViewController?.SetFinderEnabled(false);


                //barcodeDetectorFrameHandler = BarcodeDetectorFrameHandler.Attach(cameraView, detector);


                if (barcodeDetectorFrameHandler is BarcodeDetectorFrameHandler handler)
                {
                    handler.SetDetectionInterval(0);
                    //BarcodeScannerViewWrapper.InitDetectionBehavior(view, detector, new BarcodeDetectorResultHandler(YOUR_RESULT_HANDLER_FUNCTION), null);
                    //handler.AddResultHandler(new BarcodeDetectorResultHandler((result, errror) => HandleFrameHandlerResult(result)));

                    // Uncomment to enable auto-snapping (eg. single barcode scan)
                    // var barcodeAutoSnappingController = BarcodeAutoSnappingController.Attach(cameraView, handler);
                    // barcodeAutoSnappingController.SetSensitivity(1f);
                }
            }
        }

        private void StartDetection()
        {
            cameraView.ViewController?.OnResume();
            //barcodeDetectorFrameHandler.Enabled = true;
            CheckPermissions();
        }

        private void StopDetection()
        {
            cameraView.ViewController?.StopPreview();
            //barcodeDetectorFrameHandler.Enabled = false;
        }

        private void SetSelectionOverlayConfiguration()
        {
            if (Element?.OverlayConfiguration?.Enabled == true)
            {
                cameraView.SelectionOverlayController.SetEnabled(Element.OverlayConfiguration.Enabled);
                cameraView.SelectionOverlayController.SetPolygonColor(Element.OverlayConfiguration.PolygonColor.ToArgb());
                cameraView.SelectionOverlayController.SetTextColor(Element.OverlayConfiguration.TextColor.ToArgb());
                cameraView.SelectionOverlayController.SetTextContainerColor(Element.OverlayConfiguration.TextContainerColor.ToArgb());

                if (Element.OverlayConfiguration.HighlightedPolygonColor != null)
                {
                    cameraView.SelectionOverlayController.SetPolygonHighlightedColor(Element.OverlayConfiguration.HighlightedPolygonColor.ToArgb());
                }

                if (Element.OverlayConfiguration.HighlightedTextColor != null)
                {
                    cameraView.SelectionOverlayController.SetTextHighlightedColor(Element.OverlayConfiguration.HighlightedTextColor.ToArgb());
                }

                if (Element.OverlayConfiguration.HighlightedTextContainerColor != null)
                {
                    cameraView.SelectionOverlayController.SetTextContainerHighlightedColor(Element.OverlayConfiguration.HighlightedTextContainerColor.ToArgb());
                }
            }
        }

        private bool HandleSuccess(IO.Scanbot.Sdk.Barcode.Entity.BarcodeScanningResult result)
        {
            if (result == null) { return false; }

            BarcodeResultBundle outResult = new BarcodeResultBundle
            {
                Barcodes = result.BarcodeItems.ToFormsBarcodeList(),
                Image = result.PreviewFrame.ToImageSource()
            };

            HandleScanResult?.Invoke(outResult);
            return true;
        }

        private bool HandleFrameHandlerResult(BarcodeScanningResult result, IO.Scanbot.Sdk.SdkLicenseError error)
        {
            if (result != null)
            {
                HandleSuccess(result);
            }
            else
            {
                cameraView.Post(() => Toast.MakeText(Context.GetActivity(), "License has expired!", ToastLength.Long).Show());
            }

            return false;
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

        public void OnSelectionOverlayBarcodeClicked(BarcodeItem barcodeItem)
        {

        }

        public void OnCameraOpen()
        {
            //cameraView.PostDelayed(() =>
            //{

            //    // Disable auto-focus sound:
            //    cameraView.SetAutoFocusSound(false);

            //    // Uncomment to disable shutter sound (supported since Android 4.2+):
            //    // Please note that some devices may not allow disabling the camera shutter sound. 
            //    // If the shutter sound state cannot be set to the desired value, this method will be ignored.
            //    cameraView.SetShutterSound(false);

            //    // Enable ContinuousFocus mode:
            //    cameraView.ContinuousFocus();
            //}, 500);

        }

        public void OnPictureTaken(byte[] image, CaptureInfo captureInfo)
        {

        }
    }

    // Here we define a custom BarcodeDetectorResultHandler. Whenever a result is ready, the frame handler
    // will call the Handle method on this object. To make this more flexible, we allow to
    // specify a delegate through the constructor.
    // Here we define a custom BarcodeDetectorResultHandler. Whenever a result is ready, the frame handler
    // will call the Handle method on this object. To make this more flexible, we allow to
    // specify a delegate through the constructor.
    class BarcodeDetectorResultHandler : IO.Scanbot.Sdk.Barcode.BarcodeDetectorResultHandlerWrapper
    {
        public delegate bool HandleResultFunction(BarcodeScanningResult result, IO.Scanbot.Sdk.SdkLicenseError error);
        public readonly HandleResultFunction handleResultFunc;

        public BarcodeDetectorResultHandler(HandleResultFunction handleResultFunc)
        {
            this.handleResultFunc = handleResultFunc;
        }

        public override bool HandleResult(BarcodeScanningResult result, IO.Scanbot.Sdk.SdkLicenseError error)
        {
            handleResultFunc(result, error);
            return false;
        }
    }


    /**
   * Snippet from: 
   * https://stackoverflow.com/questions/64013415/pass-lambda-function-to-c-sharp-generated-code-of-kotlin-in-xamarin-android-bind
   */
    class Function1Impl<T> : Java.Lang.Object, IFunction1 where T : Java.Lang.Object
    {
        private readonly Action<T> OnInvoked;

        public Function1Impl(Action<T> onInvoked)
        {
            this.OnInvoked = onInvoked;
        }

        public Java.Lang.Object Invoke(Java.Lang.Object objParameter)
        {
            try
            {
                T parameter = (T)objParameter;
                OnInvoked?.Invoke(parameter);
                return null;
            }
            catch (Exception ex)
            {
                // Exception handling, if needed
            }

            return null;
        }
    }
}

