using ScanbotSDK.MAUI.Barcode;

namespace ReadyToUseUI.Maui.Pages;

public partial class BarcodeResultDetailPage : ContentPage
{
    private BarcodeItem barcodeItem;
    public BarcodeResultDetailPage()
    {
        InitializeComponent();
    }

    internal void NavigateData(BarcodeItem barcodeItem)
    {
        this.barcodeItem = barcodeItem;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        labelBarcodeFormatValue.Text = barcodeItem.Type.Value.ToString();
        labelBarcodeTextValue.Text = barcodeItem.Text;
        
        if (string.IsNullOrEmpty(barcodeItem.TextWithExtension))
        {
            labelBarcodeTextWithExtensionCaption.IsVisible = false;
            labelBarcodeTextWithExtensionValue.IsVisible = false;
        }
        else
        {
            labelBarcodeTextWithExtensionCaption.IsVisible = true;
            labelBarcodeTextWithExtensionValue.IsVisible = true;
            labelBarcodeTextWithExtensionValue.Text = barcodeItem.TextWithExtension;
        }

        if (barcodeItem.RawBytes != null && barcodeItem.RawBytes.Length > 1)
        {
            labelBarcodeRawBytesCaption.IsVisible = true;
            labelBarcodeRawBytesValue.IsVisible = true;
            labelBarcodeRawBytesValue.Text = System.Text.Encoding.Default.GetString(barcodeItem.RawBytes);
        }
        else
        {
            labelBarcodeRawBytesCaption.IsVisible = false;
            labelBarcodeRawBytesValue.IsVisible = false;
        }
    }
}