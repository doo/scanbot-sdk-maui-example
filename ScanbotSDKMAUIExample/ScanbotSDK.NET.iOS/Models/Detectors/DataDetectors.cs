using System;
using System.Collections.Generic;

namespace DocumentSDK.MAUI.Example.Native.iOS.Models
{
    public class DataDetectors
    {
        public static DataDetectors Instance = new DataDetectors();

        public string Title { get => "DATA DETECTORS"; }

        public List<ListItem> Items = new List<ListItem>
        {
            new ListItem { Title = "Scan MRZ",                      Code = ListItemCode.ScannerMRZ                  },
            new ListItem { Title = "Scan Health Insurance card",    Code = ListItemCode.ScannerEHIC                 },
            new ListItem { Title = "Generic Document Recognizer",   Code = ListItemCode.GenericDocumentRecognizer   },
            new ListItem { Title = "Check Recognizer",              Code = ListItemCode.CheckRecognizer             }
        };
    }
}
