namespace ClassicComponent.iOS.Models
{
    /// <summary>
    /// SDK Service model class. Used for binding data to the UITableView.
    /// </summary>
    public class SdkService
    {
        public string Title { get; set; }
        public bool ShowService { get; set; } = true;
        public Action ServiceAction { get; set; }
    }

    /// <summary>
    /// Service Title Constants
    /// </summary>
    public abstract class SdkServiceTitle
    {
        public const string SCANNING_UI = "Scanning UI";
        public const string CROPPING_UI = "Cropping UI";
        public const string IMPORT_IMAGE_FROM_LIBRARY = "Import Image from Photo Library";
        public const string APPLY_IMAGE_FILTER = "Apply Image Filter";
        public const string CREATE_TIFF = "Create TIFF";
        public const string CREATE_PDF = "Create PDF";
        public const string PERFORM_OCR = "Perform OCR";
        public const string GENERIC_DOCUMENT_RECOGNIZER = "Generic Document Recognizer";
        public const string CHECK_RECOGNIZER = "Check Recognizer";
        public const string BARCODE_SCAN_AND_COUNT = "Barcode Scan and Count";
        public const string VIN_SCANNER = "VIN Scanner";
    }
}

