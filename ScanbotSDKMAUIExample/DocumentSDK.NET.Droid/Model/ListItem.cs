﻿using System;
namespace DocumentSDK.NET.Model
{
    public enum ListItemCode
    {
        ScanDocument,
        ImportImage,
        ViewImages,

        ScannerMRZ,
        ScannerEHIC,
        
        ScannerBarcode,
        ScannerBatchBarcode,
        ScannerImportBarcode,
        ScannerImportImagesFromBarcode,


        WorkflowQR,
        WorkflowMRZImage,
        WorkflowMRZFrontBack,
        WorkflowMC,
        WorkflowSEPA,

        GenericDocumentRecognizer,
        CheckRecognizer
    }

    public class ListItem
    {
        public string Title { get; set; }

        public ListItemCode Code { get; set; }
    }
}
