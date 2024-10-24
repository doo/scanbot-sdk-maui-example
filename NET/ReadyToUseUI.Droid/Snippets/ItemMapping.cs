using IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration;
using IO.Scanbot.Sdk.Ui_v2.Common;

namespace ReadyToUseUI.Droid;

public static partial class Snippets
{
    public static BarcodeScannerConfiguration ItemMapping
    {
        get
        {
            // Create the default configuration object.
            var config = new BarcodeScannerConfiguration();
            
            var useCase = new SingleScanningMode();

            useCase.ConfirmationSheetEnabled = true;
            useCase.BarcodeInfoMapping = new BarcodeInfoMapping()
            {
                BarcodeItemMapper = new CustomMapper()
            };

            // Configure other parameters, pertaining to single-scanning mode as needed.
            config.UseCase = useCase;

            return config;
        }
    }

    public class CustomMapper : global::Java.Lang.Object, global::Java.IO.ISerializable, IO.Scanbot.Sdk.Ui_v2.Barcode.Configuration.IBarcodeItemMapper
    {
        public void MapBarcodeItem(BarcodeItem barcodeItem, IBarcodeMappingResult result)
        {
            var title = $"Some product {barcodeItem.TextWithExtension}";
            var subTitle = barcodeItem.Type.Name();
            var image = "https://avatars.githubusercontent.com/u/1454920";

            if (barcodeItem.TextWithExtension == "Error occurred!")
            {
                result.OnError();
            }
            else
            {
                result.OnResult(new BarcodeMappedData(title: title, subtitle: subTitle, barcodeImage: image));
            }
        }
    }
}