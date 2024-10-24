using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS;

public static partial class Snippets
{
    public static SBSDKUI2BarcodeScannerConfiguration ItemMapping
    {
        get
        {
            // Create the default configuration object.
            var config = new SBSDKUI2BarcodeScannerConfiguration();
            
            var useCase = new SBSDKUI2SingleScanningMode();

            useCase.ConfirmationSheetEnabled = true;
            useCase.BarcodeInfoMapping = new SBSDKUI2BarcodeInfoMapping()
            {
                BarcodeItemMapper = new CustomMapper()
            };

            // Configure other parameters, pertaining to single-scanning mode as needed.
            config.UseCase = useCase;

            return config;
        }
    }

    public class CustomMapper : SBSDKUI2BarcodeItemMapper
    {
        public override void MapBarcodeItemWithItem(SBSDKUI2BarcodeItem barcodeItem, Action<SBSDKUI2BarcodeMappedData> onResult, Action onError)
        {
            var title = $"Some product {barcodeItem.TextWithExtension}";
            var subTitle = "Subtitle";
            var image = "https://avatars.githubusercontent.com/u/1454920";

            if (barcodeItem.TextWithExtension == "Error occurred!")
            {
                onError();
            }
            else
            {
                onResult(new SBSDKUI2BarcodeMappedData(title: title, subtitle: subTitle, barcodeImage: image));
            }
        }
    }
}