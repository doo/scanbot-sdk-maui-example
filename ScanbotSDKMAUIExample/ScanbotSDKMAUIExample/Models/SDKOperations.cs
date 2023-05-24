using System;
namespace DocumentSDK.MAUI.Example.Models
{
	/// <summary>
	/// Model class to binding the ListView with all the services available in SDK.
	/// </summary>
	public class SDKService
	{
		public string Title { get; set; }
        public bool ShowSection { get; set; }
        public bool ShowService { get; set; }
    }

    /// <summary>
    /// Service Title Constants
    /// </summary>
    public class SDKServiceTitle
    {
        // DOCUMENT SCANNER
        public const string ScanDocument = "Scan Document";
        public const string ImportImageAndDetectDoc = "Import image & Detect Document";
        public const string ViewImageResults = "View Image Results";

        // BARCODE DETECTOR
        public const string ScanQRAndBarcodes = "Scan QR & Barcodes";
        public const string ScanMultipleQRAndBarcodes = "Scan Multiple QR & Barcodes";
        public const string ImportImageAndDetectBarcode = "Import Image & Detect Barcodes";

        // DATA DETECTORS
        public const string MRZScanner = "MRZ Scanner";
        public const string EHICScanner = "EHIC Scanner";
        public const string GenericDocRecognizer = "Generic Document Recognizer";
        public const string CheckRecognizer = "Check Recognizer";

        // MISCELLANEOUS
        public const string ViewLicenseInfo = "View License Info";
        public const string LearnMore = "Learn more about Scanbot SDK";
    }
}

