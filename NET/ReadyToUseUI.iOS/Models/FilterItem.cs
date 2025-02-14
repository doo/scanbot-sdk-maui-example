﻿using ScanbotSDK.iOS;

namespace ReadyToUseUI.iOS.Models
{
    public struct FilterItem
    {
        public string Title { get; set; }

        public bool IsSection { get; set; }

        public Action FilterSelected { get; set; }

        public FilterItem(string sectionTitle, Action filterSelected = null)
        {
            Title = sectionTitle;
            FilterSelected = filterSelected;
            IsSection = filterSelected == null;
        }
    }
}
