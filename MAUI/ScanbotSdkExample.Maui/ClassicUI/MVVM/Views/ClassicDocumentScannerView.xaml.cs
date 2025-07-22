using ScanbotSdkExample.Maui.ClassicUI.MVVM.ViewModels;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.Views;

public partial class ClassicDocumentScannerView : ContentPage
{
    public ClassicDocumentScannerView()
    {
        BindingContext = new ClassicDocumentScannerViewModel();
        InitializeComponent();
    }
}