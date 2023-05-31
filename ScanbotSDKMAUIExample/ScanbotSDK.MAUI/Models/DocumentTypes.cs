using BarcodeSDK.MAUI.Constants;

namespace DocumentSDK.MAUI.Example.Models
{
    public class DocumentTypes
    {

        public static DocumentTypes Instance { get; private set; } = new DocumentTypes();

        public Dictionary<BarcodeDocumentFormat, bool> List { get; private set; } = new Dictionary<BarcodeDocumentFormat, bool>();
        public bool IsFilteringEnabled { get; set; } = false;

        public List<BarcodeDocumentFormat> AcceptedTypes
        {
            get
            {
                var result = new List<BarcodeDocumentFormat>();

                if (!IsFilteringEnabled)
                {
                    return result;
                }

                foreach (var item in List)
                {
                    if (item.Value)
                    {
                        result.Add(item.Key);
                    }
                }

                return result;
            }
        }

        public bool IsChecked(BarcodeDocumentFormat lastCheckedFormat)
        {
            return AcceptedTypes.Contains(lastCheckedFormat);
        }

        public List<BarcodeDocumentFormat> All =>
            Enum.GetValues(typeof(BarcodeDocumentFormat)).Cast<BarcodeDocumentFormat>().ToList();

        private DocumentTypes()
        {
            foreach (BarcodeDocumentFormat format in All)
            {
                List.Add(format, IsEnabledByDefault(format));
            }
        }


        public void Update(BarcodeDocumentFormat type, bool value)
        {
            List[type] = value;
        }

        private bool IsEnabledByDefault(BarcodeDocumentFormat format)
        {
            return true;
        }

    }
}
