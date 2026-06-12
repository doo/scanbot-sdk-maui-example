using System.Collections.ObjectModel;

namespace ScanbotSdkExample.Maui.Results;

public class PreviewImage(ImageSource _image)
{
    public PreviewImage(string path) : this(ImageSource.FromFile(path))
    {
    }

    public ImageSource Image => _image;
}

public partial class PreviewImagesPage : ContentPage
{
    private List<PreviewImage> _imageList;
    public ObservableCollection<PreviewImage> ImageList { get; set; }
    public PreviewImagesPage(Uri[] imagePaths)
    {
        InitializeComponent();
        BindingContext = this;
        
        _imageList = new List<PreviewImage>();
        foreach (var imagePath in imagePaths)
        {
            _imageList.Add(new PreviewImage(imagePath.LocalPath));
        }
    }
    
    public PreviewImagesPage(List<ImageSource> sources)
    {
        InitializeComponent();
        BindingContext = this;
        
        _imageList = new List<PreviewImage>();
        foreach (var source in sources)
        {
            _imageList.Add(new PreviewImage(source));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CollectionViewImages.ItemsSource = new ObservableCollection<PreviewImage>(_imageList);
    }
}