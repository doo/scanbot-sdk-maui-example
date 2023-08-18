namespace ReadyToUseUI.iOS.Models
{
    public class DocumentScanner
    {
        public static DocumentScanner Instance = new DocumentScanner();

        public string Title { get => "DOCUMENT SCANNER";  }

        public List<ListItem> Items = new List<ListItem>
        {
            new ListItem { Title = "Scan Document", Code = ListItemCode.ScanDocument },
            new ListItem { Title = "Scan Document with Finder", Code = ListItemCode.ScanDocumentWithFinder },
            new ListItem { Title = "Import Image", Code = ListItemCode.ImportImage },
            new ListItem { Title = "View Images", Code = ListItemCode.ViewImages }
        };
    }
}
