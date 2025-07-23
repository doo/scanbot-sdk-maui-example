using ScanbotSdkExample.Maui.ClassicUI.MVVM.ViewModels;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.Views;

public partial class ClassicDocumentScannerView : ContentPage
{
    public ClassicDocumentScannerView()
    {
        var vm = new ClassicDocumentScannerViewModel
        {
            SnapDocumentImageCommand = new Command(() => DocumentScannerView.SnapDocumentImage())
        };
        BindingContext = vm;
        InitializeComponent();
    }
}