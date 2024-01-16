using System;
using Android.Graphics;
using IO.Scanbot.Sdk.Barcode.Entity;

namespace ReadyToUseUI.Droid.Model
{
    public class BarcodeResultBundle
    {
        private static readonly Lazy<BarcodeResultBundle> lazyInstance = new Lazy<BarcodeResultBundle>(() => new BarcodeResultBundle());

        public static BarcodeResultBundle Instance => lazyInstance.Value;

        public BarcodeScanningResult ScanningResult { get; set; }

        public string ImagePath { get; set; }

        public string PreviewPath { get; set; }

        public Bitmap ResultBitmap { get; set; }
    }
}