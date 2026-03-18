using System.Diagnostics;
using ScanbotSDK.MAUI.Document.ClassicComponent;
using ScanbotSdkExample.Maui.ClassicUI.MVVM.ViewModels;

namespace ScanbotSdkExample.Maui.ClassicUI.MVVM.Views;

public partial class ClassicDocumentScannerView : ContentPage
{
    private ClassicDocumentScannerViewModel ViewModel { get; }

    public ClassicDocumentScannerView()
    {
        BindingContext = ViewModel = new ClassicDocumentScannerViewModel();
        InitializeComponent();
        
        ViewModel.SnapDocumentCommand = new Command(() =>
        {
            DocumentScannerView?.SnapDocument();
        });
    }
}