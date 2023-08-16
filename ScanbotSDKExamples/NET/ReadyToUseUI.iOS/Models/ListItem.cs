namespace ReadyToUseUI.iOS.Models
{
    public enum ListItemCode
    {
        ScanDocument,
        ScanDocumentWithFinder,
        ImportImage,
        ViewImages,

        ScannerMRZ,
        ScannerEHIC,
        
        ScannerBarcode,
        ScannerBatchBarcode,
        ScannerImportBarcode,
        ScannerImportImagesFromBarcode,

        GenericDocumentRecognizer,
        CheckRecognizer
    }

    public class ListItem
    {
        public string Title { get; set; }

        public ListItemCode Code { get; set; }
    }
}
