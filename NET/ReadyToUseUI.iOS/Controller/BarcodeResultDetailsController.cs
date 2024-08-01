using System.Text;
using ReadyToUseUI.iOS.Utils;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public class BarcodeResultDetailsController : UIViewController
{
    
    private SBSDKUI2BarcodeItem barcode;
    private UITextView textView;
    internal void NavigateData(SBSDKUI2BarcodeItem barcode)
    {
        this.barcode = barcode;
    }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        
        textView = new UITextView();
        textView.TranslatesAutoresizingMaskIntoConstraints = false;
        
        View.AddSubview(textView);
      
        textView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 1).Active = true;
        textView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 1).Active = true;
        textView.TopAnchor.ConstraintEqualTo(View.TopAnchor,1 ).Active = true;
        textView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 1).Active = true;

        textView.Text = GetBarcodeText();
    }

    private string GetBarcodeText()
    {
        var text = "______________________________________\n\n";
        text += "Barcode Type:\n\n";
        text += barcode.Type.ToString()  +"\n\n";

        var documentDetails = ParseData(barcode.ParsedDocument);
        if (!string.IsNullOrEmpty(documentDetails))
        {
            text += "______________________________________\n\n";
            text += "Document Details:\n\n";
            text += documentDetails + "\n\n";
        }

        if (!string.IsNullOrEmpty(barcode.ParsedDocument?.Type?.Name))
        {
            text += "______________________________________\n\n";
            text += "Document Type:\n\n";
            text += barcode.ParsedDocument.Type.Name + "\n\n";
        }

        text += "______________________________________\n\n";
        text += "Raw Text:\n\n";
        text += barcode.TextWithExtension +"\n\n";
        text += "______________________________________\n\n";
        
        return text;
    }
    
    private string ParseData(SBSDKGenericDocument result)
    {
        if (result == null) return string.Empty;
        
        var builder = new StringBuilder();
        var description = string.Join(";\n", result.Fields?
            .Where(field => field != null)
            .Select((field) =>
            {
                string outStr = "";
                if (field.Type?.Name != null)
                {
                    outStr += field.Type.Name + " = ";
                }
                if (field.Value != null && field.Value.Text != null)
                {
                    outStr += field.Value.Text;
                }
                return outStr;
            })
            .ToList()
        );
        return description;
    }
}