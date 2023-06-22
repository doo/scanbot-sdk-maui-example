using BarcodeSDK.MAUI.Models;
using Xamarin.Google.Crypto.Tink.Proto;

namespace ScanbotSDK.MAUI.NativeRenderer;
/// <summary>
/// Type of application.
/// </summary>
enum ApplicationType
{
    SinglePage,
    NavigationPage,
    TabbedPage
}

public partial class App : Application
{
    public const string LICENSE_KEY = "kFnbPhgSwGpVGyyqAquL2qyDt7Iq+0" +
  "tTEVyIcO6fw7X6bKOU2y3iTZ+aY7KT" +
  "dT64j1TMtW1NNd4Wd2GtDRhvKwpEDf" +
  "W4jAcH6m91EO7DdrT6RS3E05sEIc2i" +
  "iNDoQ7o3y8JOomcyuExbyEEgicgSh6" +
  "sC/xYfuRnAmm08RjypAymRHAF5Pqjb" +
  "vZ8ZgGsoA5M+vDdDAKy+vq8CCkSEe8" +
  "z4a7MjfZMn7TmcxTE+vKfjn9fSZ9vw" +
  "N+D5CNgLCUnk4QGNb8UgQL2AaeDHil" +
  "jwtXDeOOioqx+cw7PJHMGh4T3crGEp" +
  "UUC6XbpwGLs0lfP8s1Q6MuidASmYg3" +
  "bkpHEQFqDPMA==\nU2NhbmJvdFNESw" +
  "ppby5zY2FuYm90LmV4YW1wbGV8aW8u" +
  "c2NhbmJvdC5leGFtcGxlLmZsdXR0ZX" +
  "J8aW8uc2NhbmJvdC5leGFtcGxlLnNk" +
  "ay5hbmRyb2lkfGlvLnNjYW5ib3QuZX" +
  "hhbXBsZS5zZGsuYmFyY29kZS5hbmRy" +
  "b2lkfGlvLnNjYW5ib3QuZXhhbXBsZS" +
  "5zZGsuYmFyY29kZS5mbHV0dGVyfGlv" +
  "LnNjYW5ib3QuZXhhbXBsZS5zZGsuYm" +
  "FyY29kZS5pb25pY3xpby5zY2FuYm90" +
  "LmV4YW1wbGUuc2RrLmJhcmNvZGUucm" +
  "VhY3RuYXRpdmV8aW8uc2NhbmJvdC5l" +
  "eGFtcGxlLnNkay5iYXJjb2RlLndpbm" +
  "Rvd3N8aW8uc2NhbmJvdC5leGFtcGxl" +
  "LnNkay5iYXJjb2RlLnhhbWFyaW58aW" +
  "8uc2NhbmJvdC5leGFtcGxlLnNkay5i" +
  "YXJjb2RlLnhhbWFyaW4uZm9ybXN8aW" +
  "8uc2NhbmJvdC5leGFtcGxlLnNkay5j" +
  "YXBhY2l0b3IuaW9uaWN8aW8uc2Nhbm" +
  "JvdC5leGFtcGxlLnNkay5jb3Jkb3Zh" +
  "LmlvbmljfGlvLnNjYW5ib3QuZXhhbX" +
  "BsZS5zZGsuZmx1dHRlcnxpby5zY2Fu" +
  "Ym90LmV4YW1wbGUuc2RrLmlvcy5iYX" +
  "Jjb2RlfGlvLnNjYW5ib3QuZXhhbXBs" +
  "ZS5zZGsuaW9zLmNsYXNzaWN8aW8uc2" +
  "NhbmJvdC5leGFtcGxlLnNkay5pb3Mu" +
  "cnR1dWl8aW8uc2NhbmJvdC5leGFtcG" +
  "xlLnNkay5yZWFjdG5hdGl2ZXxpby5z" +
  "Y2FuYm90LmV4YW1wbGUuc2RrLnJlYW" +
  "N0Lm5hdGl2ZXxpby5zY2FuYm90LmV4" +
  "YW1wbGUuc2RrLnJ0dS5hbmRyb2lkfG" +
  "lvLnNjYW5ib3QuZXhhbXBsZS5zZGsu" +
  "eGFtYXJpbnxpby5zY2FuYm90LmV4YW" +
  "1wbGUuc2RrLnhhbWFyaW4uZm9ybXN8" +
  "aW8uc2NhbmJvdC5leGFtcGxlLnNkay" +
  "54YW1hcmluLnJ0dXxpby5zY2FuYm90" +
  "Lm5hdGl2ZWJhcmNvZGVzZGtyZW5kZX" +
  "Jlcnxpby5zY2FuYm90LnNkay5pbnRl" +
  "cm5hbGRlbW98bG9jYWxob3N0fHNjYW" +
  "5ib3RzZGstd2FzbS1kZWJ1Z2hvc3Qu" +
  "czMtZXUtd2VzdC0xLmFtYXpvbmF3cy" +
  "5jb218d2Vic2RrLWRlbW8taW50ZXJu" +
  "YWwuc2NhbmJvdC5pbwoxNjkzNjEyNz" +
  "k5CjgzODg2MDcKMzE=\n";

    // JUST FOR DEBUG PURPOSES.
    // Using this constant we can switch between different application
    // structures (SinglePage, NavigationPage, TabbedPage) to make
    // sure everything works under every circumstance.
    const ApplicationType APPLICATION_TYPE = ApplicationType.SinglePage;

    public App()
    {
        InitializeComponent();
        MainPage = GetMainPage(applicationType: APPLICATION_TYPE);
    }

    protected override void OnStart()
    {
    }

    protected override void OnSleep()
    {
    }

    protected override void OnResume()
    {
    }

    /// <summary>
    /// Get the Application Main Page by selecting the structure mentioned.
    /// </summary>
    /// <param name="applicationType">Specify structure of the application</param>
    /// <returns>Returns the page object</returns>
    private Page GetMainPage(ApplicationType applicationType)
    {
        switch (applicationType)
        {
            case ApplicationType.SinglePage:
                return new MainPage();
            case ApplicationType.NavigationPage:
                return new NavigationPage(new MainPage());
            case ApplicationType.TabbedPage:
                var tabbedPage = new TabbedPage();
                tabbedPage.Children.Add(new MainPage { Title = "1" });
                tabbedPage.Children.Add(new SecondPage { Title = "2" });
                tabbedPage.Children.Add(new ThirdPage { Title = "3" });
                return tabbedPage;
            default:
                return new MainPage();

        }
    }

    public class SecondPage : ContentPage { }

    public class ThirdPage : ContentPage { }
}

