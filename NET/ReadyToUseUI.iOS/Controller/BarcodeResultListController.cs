using ReadyToUseUI.iOS.Utils;
using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Controller;

public interface IBarcodeResultSourceDelegate
{
    List<SBSDKUI2BarcodeItem> Barcodes { get; }
    void BarcodeItemSelected(int row);
}

[Register("BarcodeResultController")]
public class BarcodeResultListController : UIViewController, IBarcodeResultSourceDelegate
{
    private List<SBSDKUI2BarcodeItem> barcodes;
    private UITableView tableView;
    private IBarcodeResultSourceDelegate tableViewInteraction;
    
    public List<SBSDKUI2BarcodeItem> Barcodes => barcodes;
    internal void NavigateData(List<SBSDKUI2BarcodeItem> barcodes)
    {
        this.barcodes = barcodes;
    }
    
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        
        tableView = new UITableView();
        tableView.TableFooterView = new UIView();
        tableView.RowHeight = UITableView.AutomaticDimension;
        tableView.EstimatedRowHeight = 90;
        tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
        tableView.Source = new BarcodeResultSource(this);
        tableView.TranslatesAutoresizingMaskIntoConstraints = false;
        
        View.AddSubview(tableView);
      
        tableView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 1).Active = true;
        tableView.RightAnchor.ConstraintEqualTo(View.RightAnchor, 1).Active = true;
        tableView.TopAnchor.ConstraintEqualTo(View.TopAnchor,1 ).Active = true;
        tableView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, 1).Active = true;
    }
    
    public void BarcodeItemSelected(int row)
    {
        var detailsController = new BarcodeResultDetailsController();
        detailsController.NavigateData(barcodes[row]);
        NavigationController.PushViewController(detailsController, true);
    }
}

public class BarcodeResultSource : UITableViewSource
{
    private IBarcodeResultSourceDelegate interaction;
    public BarcodeResultSource(IBarcodeResultSourceDelegate interaction)
    {
        this.interaction = interaction;
    }
    
    public override IntPtr RowsInSection(UITableView tableView, IntPtr section) => interaction?.Barcodes?.Count ?? 0;
    
    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = new UITableViewCell(UITableViewCellStyle.Default, "barcodeCell");
        cell.SelectionStyle = UITableViewCellSelectionStyle.None;
        var barcode = interaction?.Barcodes[indexPath.Row];
        if (barcode != null)
        {
            ConfigureDefaultCell(cell,barcode.Type?.ToBarcodeFormatName() , barcode.Text);
        }
        return cell;
    }

    void ConfigureDefaultCell(UITableViewCell cell, string text, string subtitle = "", UIImage image = null)
    {
        if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
        {
            var config = UIListContentConfiguration.CellConfiguration;
            config.Text = text;
            config.TextProperties.Color = UIColor.DarkGray;
            config.TextProperties.Font = UIFont.FromName("Helvetica", 16.0f);
            config.TextProperties.Alignment = UIListContentTextAlignment.Natural;

            config.SecondaryText = subtitle;
            config.TextProperties.Color = UIColor.Gray;
            config.SecondaryTextProperties.Font = UIFont.FromName("Helvetica", 14.0f);
            config.SecondaryTextProperties.Alignment = UIListContentTextAlignment.Natural;
            config.SecondaryTextProperties.NumberOfLines = 2;
            config.SecondaryTextProperties.LineBreakMode = UILineBreakMode.TailTruncation;

            config.Image = image;
            cell.ContentConfiguration = config;
        }
        else
        {
            cell.TextLabel.Text = text;
            cell.TextLabel.TextColor = UIColor.DarkGray;
            cell.TextLabel.Font = UIFont.FromName("Helvetica", 16.0f);
            cell.TextLabel.TextAlignment = UITextAlignment.Left;
            
            cell.DetailTextLabel.Text = subtitle;
            cell.DetailTextLabel.TextColor = UIColor.Gray;
            cell.DetailTextLabel.Font = UIFont.FromName("Helvetica", 14.0f);
            cell.DetailTextLabel.TextAlignment = UITextAlignment.Left;
            cell.DetailTextLabel.Lines = 2;
            cell.DetailTextLabel.LineBreakMode = UILineBreakMode.TailTruncation;

            cell.ImageView.Image = image;
        }
    }

    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        interaction?.BarcodeItemSelected(indexPath.Row);
    }
}