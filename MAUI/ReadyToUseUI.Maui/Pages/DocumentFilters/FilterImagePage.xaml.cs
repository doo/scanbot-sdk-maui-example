using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadyToUseUI.Maui.Pages;

public partial class FilteredImagePage : ContentPage
{
    public ImageSource FilteredImage {
        get;
        set;
    }
    
    public FilteredImagePage()
    {
        InitializeComponent();
        this.BindingContext = this;
    }
    
    internal void NavigateData(ImageSource source)
    {
        FilteredImage = source;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        filteredImage.Source = FilteredImage;
    }

    private void OpenFilters_OnClicked(object sender, EventArgs e)
    {
        // todo: show picker with filter options for user selection. 
    }
}