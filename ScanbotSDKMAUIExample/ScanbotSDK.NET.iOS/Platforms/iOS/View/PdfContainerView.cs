﻿
using System;
using UIKit;
using PdfKit;
using Foundation;
using CoreGraphics;
using DocumentSDK.MAUI.Example.Native.iOS.Models;
using System.Runtime.InteropServices;
using SBSDK = DocumentSDK.MAUI.Native.iOS.ScanbotSDK;

namespace DocumentSDK.MAUI.Example.Native.iOS.View
{
    public class PdfContainerView : UIView
    {
        UILabel title;
        PdfView content;

        public PdfContainerView(NSUrl uri, bool ocr)
        {
            BackgroundColor = UIColor.White;

            title = new UILabel();
            title.TextAlignment = UITextAlignment.Center;
            title.TextColor = Models.Colors.DarkGray;
            title.Font = UIFont.FromName("HelveticaNeue-Bold", 13f);
            title.Lines = 0;

            AddSubview(title);

            content = new PdfView();
            content.DisplayMode = PdfDisplayMode.SinglePageContinuous;
            content.AutoScales = true;

            var data = NSData.FromFile(uri.Path);
            // If data is encrypted, SBSDK.Encrypter will be evaluated.
            // In that case, use it to decrypt the data
            if (SBSDK.Encrypter != null)
            {
                data = SBSDK.Encrypter.DecryptData(data);
            }
            content.Document = new PdfDocument(data);
            
            AddSubview(content);

            if (ocr)
            {
                title.Text =
                    "Good job! You created a sandwich .pdf.\n" +
                    "Go ahead, try to select part of the text of your saved file";
            }
            else
            {
                title.Text =
                    "Good job! You saved a plain pdf.\n" +
                    "Try to select part of your text, you won't be able to";
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float padding = 5;

            float x = padding;
            float y = padding;
            float w = (float)Frame.Width - 2 * padding;
            float h = w / 5;

            title.Frame = new CGRect(x, y, w, h);

            y += h + padding;
            h = (float)Frame.Height - (h + 3 * padding);

            content.Frame = new CGRect(x, y, w, h);
        }
    }
}
