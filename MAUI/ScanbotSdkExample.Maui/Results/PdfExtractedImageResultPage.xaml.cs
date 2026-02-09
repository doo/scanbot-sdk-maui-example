using System.Collections.ObjectModel;
using ScanbotSDK.MAUI.Image;

namespace ScanbotSdkExample.Maui.Results;

public class ExtractedPdfImage
{
    public ExtractedPdfImage(ImageSource imageSource)
    {
        _extractedImageSource = imageSource;
    }

    public ExtractedPdfImage(string path)
    {
        _extractedImageSource = ImageSource.FromFile(path);
    }
    
    private ImageSource _extractedImageSource;
    public ImageSource ExtractedImageSource => _extractedImageSource;
}

public partial class PdfExtractedImageResultPage : ContentPage
{
    private List<ExtractedPdfImage> _imageList;
    public ObservableCollection<ExtractedPdfImage> ImageList { get; set; }
    public PdfExtractedImageResultPage(string[] imagePaths)
    {
        InitializeComponent();
        BindingContext = this;
        
        _imageList = new List<ExtractedPdfImage>();
        foreach (var imagePath in imagePaths)
        {
            _imageList.Add(new ExtractedPdfImage(imagePath));
        }
    }
    
    public PdfExtractedImageResultPage(List<ImageSource> sources)
    {
        InitializeComponent();
        BindingContext = this;
        
        _imageList = new List<ExtractedPdfImage>();
        foreach (var source in sources)
        {
            _imageList.Add(new ExtractedPdfImage(source));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CollectionViewImages.ItemsSource = new ObservableCollection<ExtractedPdfImage>(_imageList);
    }
}