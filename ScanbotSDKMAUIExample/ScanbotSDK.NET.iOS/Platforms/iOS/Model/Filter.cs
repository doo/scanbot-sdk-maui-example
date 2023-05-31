﻿using System;
using ScanbotSDK.iOS;

namespace DocumentSDK.MAUI.Example.Native.iOS.Model
{
    public class Filter
    {
        public Filter()
        {

        }

        public Filter(string title, SBSDKImageFilterType type)
        {
            Title = title;
            Type = type;
        }

        public string Title { get; set; }

        public SBSDKImageFilterType Type { get; set; }
    }
}
