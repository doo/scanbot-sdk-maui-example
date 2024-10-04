# Scanbot Barcode & Document Scanning Example App for MAUI

This example app shows how to integrate the [Scanbot Barcode Scanner SDK](https://scanbot.io/products/barcode-software/barcode-sdk/), [Scanbot Document Scanner SDK](https://scanbot.io/products/document-scanning/document-scanner-sdk/), and [Scanbot Data Capture SDK](https://scanbot.io/products/data-capture/data-capture-sdk/) for MAUI and .NET(Native iOS and Android platforms).

The Scanbot SDK are available as a NuGet package for the MAUI and .NET platforms:
[ScanbotSDK.MAUI](https://www.nuget.org/packages/ScanbotSDK.MAUI)
[ScanbotSDK.NET](https://www.nuget.org/packages/ScanbotSDK.NET)

## What is the Scanbot SDK?

The Scanbot SDK lets you integrate barcode & document scanning, as well as data extraction functionalities, into your mobile apps and website. It contains different modules that are licensable for an annual fixed price. For more details, visit our website https://scanbot.io.


## Trial License

The Scanbot SDK will run without a license for one minute per session!

After the trial period has expired, all SDK functions and UI components will stop working. You have to restart the app to get another one-minute trial period.

To try the Scanbot SDK without a one-minute limit, you can get a free “no-strings-attached” trial license. Please submit the [Trial License Form](https://scanbot.io/trial/) on our website.

## Free Developer Support

We provide free "no-strings-attached" developer support for the implementation & testing of the Scanbot SDK.
If you encounter technical issues with integrating the Scanbot SDK or need advice on choosing the appropriate
framework or features, please visit our [Support Page](https://docs.scanbot.io/support/).

## Documentation
👉 [Scanbot SDK documentation](https://docs.scanbot.io/document-scanner-sdk/maui/introduction/)

## Requirements
[Microsoft Visual Studio](https://www.visualstudio.com)
Developing native, cross-platform .NET Multi-platform App UI (.NET MAUI) apps requires Visual Studio 2022 17.3 or greater, or Visual Studio 2022 for Mac 17.4 or greater.


## Documentation
The documentation of the current Scanbot SDK MAUI release can be found [here](https://docs.scanbot.io/document-scanner-sdk/maui/introduction/)

### Build Instructions

Assuming you already have your development machine setup, the following commands will help you build and debug our projects:

#### .NET
##### iOS

To build the iOS example project for both net7.0-ios and net8.0-ios, forcing packages to be restored and everything to be compiled from scratch:

```dotnet build NET/ReadyToUseUI.iOS --force --no-incremental -r ios-arm64```

To run the project on a real device, specify a target framework with `-f net8.0-ios` and the Run target via `-t:Run`, yielding the following:

```dotnet build NET/ReadyToUseUI.iOS -r ios-arm64 -f net8.0-ios -t:Run --force --no-incremental```

The Classic Component has its own project. Run it with:

```dotnet build NET/ClassicComponent.iOS -r ios-arm64 -f net8.0-ios -t:Run --force --no-incremental```

##### Android
To build the Android example project for both net7.0-android and net8.0-android, forcing packages to be restored and everything to be compiled from scratch:

```dotnet build NET/ReadyToUseUI.Droid --force --no-incremental```

To run the project on a real device, specify a target framework with `-f net8.0-android` and the Run target via `-t:Run`, yielding the following:

```dotnet build NET/ReadyToUseUI.Droid -f net8.0-android -t:Run --force --no-incremental```

The Classic Component has its own project. Run it with:

```dotnet build NET/ClassicComponent.Droid -f net8.0-android -t:Run --force --no-incremental```

#### MAUI

To build the MAUI example project for all supported target frameworks (net7.0-android, net7.0-ios, net8.0-android and net8.0-ios) and forcing packages to be restored and everything to be compiled from scratch, execute:

```dotnet build MAUI/ReadyToUseUI.Maui --force --no-incremental```

To run the project on a real iOS device, specify a target framework with `-f net8.0-ios` and the Run target via `-t:Run`, yielding the following:

```dotnet build MAUI/ReadyToUseUI.Maui -f net8.0-ios -t:Run --force --no-incremental```


To run the project on a real Android device, specify a target framework with `-f net8.0-android` and the Run target via `-t:Run`, yielding the following:

```dotnet build MAUI/ReadyToUseUI.Maui -f net8.0-android -t:Run --force --no-incremental```

## Please note

The Scanbot SDK will run without a license for one minute per session!

After the trial period has expired all Scanbot SDK functions as well as the UI components (like the Document Scanner UI) will stop working or may be terminated.
You have to restart the app to get another trial period.

To get a free "no-strings-attached" trial license, please submit the [Trial License Form](https://scanbot.io/trial/) on our website.
