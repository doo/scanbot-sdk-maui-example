using ScanbotSDK.MAUI.Constants;

namespace ReadyToUseUI.Maui.Models
{
    public class DocumentTypes
    {
        public static DocumentTypes Instance { get; private set; } = new DocumentTypes();

        public Dictionary<BarcodeDocumentFormat, bool> formats;
        public bool IsFilteringEnabled { get; set; } = false;

        private DocumentTypes()
        {
            formats = All.Aggregate(new Dictionary<BarcodeDocumentFormat, bool>(), (dict, format) =>
            {
                dict.Add(format, true);
                return dict;
            });
        }

        public List<BarcodeDocumentFormat> AcceptedTypes
        {
            get
            {
                if (!IsFilteringEnabled)
                {
                    return new List<BarcodeDocumentFormat>();
                }

                return formats.Where(i => i.Value)
                           .Select(i => i.Key)
                           .ToList();
            }
        }

        public bool IsChecked(BarcodeDocumentFormat lastCheckedFormat)
        {
           if (formats.TryGetValue(lastCheckedFormat, out var isChecked))
           {
                return isChecked;
           }

           return false;
        }

        public List<BarcodeDocumentFormat> All => Enum.GetValues<BarcodeDocumentFormat>().ToList();

        public void Update(BarcodeDocumentFormat type, bool value)
        {
            formats[type] = value;
        }
    }
}
