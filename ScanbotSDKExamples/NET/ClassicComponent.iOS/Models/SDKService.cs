namespace ClassicComponent.iOS.Models
{
    /// <summary>
    /// SDK Service model class. Used for binding data to the UITableView.
    /// </summary>
    public class SDKService
    {
        public string Title { get; set; }
        public bool ShowService { get; set; } = true;
    }

    /// <summary>
    /// Service Title Constants
    /// </summary>
    public class SDKServiceTitle
    {
        public const string ScanningUI = "Scanning UI";
        public const string CroppingUI = "Cropping UI";
        public const string ImportImageFromLibrary = "Import Image from Photo Library";
        public const string ApplyImageFilter = "Apply Image Filter";
        public const string CreateTIFF = "Create TIFF";
        public const string CreatePDF = "Create PDF";
        public const string PerformOCR = "Perform OCR";
        public const string GenericDocumentRecognizer = "Generic Document Recognizer";
        public const string CheckRecognizer = "Check Recognizer";
        public const string BarcodeScanAndCount = "Barcode Scan and Count";
        public const string VINScanner = "VIN Scanner";
    }
}

