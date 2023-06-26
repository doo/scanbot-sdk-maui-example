using System;
using BarcodeSDK.MAUI.Models;
using DocumentSDK.MAUI.iOS;
using ScanbotSDK.iOS;
using ScanbotSDK.MAUI.NativeRenderer.CustomViews;
using UIKit;

namespace ScanbotSDK.MAUI.NativeRenderer.Platforms.iOS.CustomViews
{
	public class BarcodeCameraView_iOS : UIView
	{
        BarcodeCameraViewHandler barcodeCameraViewHandler;
        SBSDKBarcodeScannerViewController cameraViewController;
        private BarcodeCameraView.BarcodeScannerResultHandler handleBarcodeScanResults;

        public BarcodeCameraView_iOS(UIViewController viewController)
		{
           
        }

        internal void ConnectHandler(BarcodeCameraView commonView, BarcodeCameraViewHandler barcodeCameraViewHandler)
        {
            this.barcodeCameraViewHandler = barcodeCameraViewHandler;
            cameraViewController = new SBSDKBarcodeScannerViewController(AppDelegate.CurrentViewController, this);
            cameraViewController.Delegate = new BarcodeScannerDelegate
            {
                OnDetect = HandleBarcodeScannerResults
            };
            commonView.OnBarcodeScanResult = handleBarcodeScanResults;
            cameraViewController.BarcodeImageGenerationType = SBSDKBarcodeImageGenerationType.None;
        }
        #region Properties Implementation

        internal void MapIsFlashEnabled(bool isFlashEnabled)
        {
            cameraViewController.FlashLightEnabled = isFlashEnabled;
        }

        // -----------------------------------------
        // Selection Overlay Config binding
        // -----------------------------------------
        internal void MapOverlayConfiguration(BarcodeCameraView commonView)
        {
            var config = commonView.OverlayConfiguration;
            if (config.Enabled == true)
            {
                //cameraViewController.SelectionOverlayEnabled = true;

                //cameraViewController.SelectionPolygonColor = config.PolygonColor.ToNative();
                //cameraViewController.SelectionTextColor = config.TextColor.ToNative();
                //cameraViewController.SelectionTextContainerColor = config.TextContainerColor.ToNative();
                //if (config.HighlightedPolygonColor != null)
                //{
                //    cameraViewController.SelectionHighlightedPolygonColor = config.HighlightedPolygonColor?.ToNative();
                //}

                //if (config.HighlightedTextColor != null)
                //{
                //    cameraViewController.SelectionHighlightedTextColor = config.HighlightedTextColor?.ToNative();
                //}

                //if (config.HighlightedTextContainerColor != null)
                //{
                //    cameraViewController.SelectionHighlightedTextContainerColor = config.HighlightedTextContainerColor?.ToNative();
                //}
            }
        }

        #endregion

        #region Event Handlers Implementation

        internal void MapStartDetectionHandler()
        {
            cameraViewController.RecognitionEnabled = true;
        }

        internal void MapStopDetectionHandler()
        {
            cameraViewController.RecognitionEnabled = false;
        }

        // -----------------------------------------
        // Invokes results for the Common MAUI side
        // -----------------------------------------
        internal void HandleBarcodeScannerResults(SBSDKBarcodeScannerResult[] codes)
        {
            handleBarcodeScanResults(new BarcodeResultBundle()
            {
                Barcodes = codes.ToFormsBarcodes()
            });
        }

        #endregion

        //// Find the View from Navigation heirarchy and initialise it.
        //private UIViewController FindAndInitialiseView()
        //{
        //    var viewController = CurrentViewController?.ChildViewControllers?.First();

        //    // If application has a Navigation Controller
        //    if (viewController is UINavigationController navigationController)
        //    {
        //        return navigationController.VisibleViewController;
        //    }
        //    else if (viewController is UITabBarController tabBarController)
        //    {
        //        // It is itself a Page renderer.
        //        return tabBarController.SelectedViewController;
        //    }
        //    else
        //    {   // If application has no Navigation Controller OR TabBarController
        //        return viewController;
        //    }
        //}

        /// Initialise the Camera View.
        //private UIViewController InitialiseView(UIViewController visibleViewController)
        //{
            //PageRenderer pageRendererViewController = null;
            //if (visibleViewController is PageRenderer) // In case of TabBedPage ViewController
            //{
            //    pageRendererViewController = visibleViewController as PageRenderer;
            //}
            //else if (visibleViewController?.ChildViewControllers?.First() is PageRenderer) // Navigation/Single page
            //{
            //    pageRendererViewController = visibleViewController.ChildViewControllers.First() as PageRenderer;
            //}

            //if (pageRendererViewController != null)
            //{
            //    cameraViewController.Initialize(pageRendererViewController);
            //    cameraViewController.ScannerDelegate.OnDetect = HandleBarcodeScannerResults;
            //    barcodeScannerResultHandler = Element.OnBarcodeScanResult;
            //    isInitialized = true;
            //}
        //}
    }

    // Since we cannot directly inherit from SBSDKBarcodeScannerViewControllerDelegate in our ViewRenderer,
    // we have created this wrapper class to allow binding to its events through the use of delegates
    class BarcodeScannerDelegate : SBSDKBarcodeScannerViewControllerDelegate
    {
        public delegate void OnDetectHandler(SBSDKBarcodeScannerResult[] codes);
        public OnDetectHandler OnDetect;

        public override void DidDetectBarcodes(SBSDKBarcodeScannerViewController controller, SBSDKBarcodeScannerResult[] codes)
        {
            OnDetect?.Invoke(codes);
        }

        public override bool BarcodeScannerControllerShouldDetectBarcodes(SBSDKBarcodeScannerViewController controller)
        {
            if (DocumentSDK.MAUI.ScanbotSDK.SDKService.IsLicenseValid)
            {
                return true;
            }
            else
            {
                (UIApplication.SharedApplication.Delegate as AppDelegate)?.ShowAlert("License Expired!", "Ok");
                return false;
            }
        }
    }
}

