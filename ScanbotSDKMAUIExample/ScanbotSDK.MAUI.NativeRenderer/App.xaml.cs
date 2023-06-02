namespace ScanbotSDK.MAUI.NativeRenderer;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}

