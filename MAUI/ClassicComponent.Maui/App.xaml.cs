namespace ClassicComponent.Maui
{
    public partial class App : Application
    {
        public const string LICENSE_KEY = "";

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new HomePage());
        }
    }
}
