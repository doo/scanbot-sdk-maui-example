namespace ScanbotSdkExample.Droid.Model;

public class PageModel
{
    public string DocumentId { get; set; }
        
    public string PageId { get; set; }
        
    public Android.Net.Uri OriginalPagePreviewUri { get; set; }
        
    public Android.Net.Uri OriginalPageUri { get; set; }
        
    public Android.Net.Uri ScannedPageUri { get; set; }
        
    public Android.Net.Uri ScannedPagePreviewUri { get; set; }
}