using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Common;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
	private static async Task IntroductionSnippet()
	{
		// Create the default configuration object.
		var configuration = new DocumentScanningFlow();

		// Retrieve the instance of the introduction configuration from the main configuration object.
		var introductionConfiguration = configuration.Screens.Camera.Introduction;

		// Show the introduction screen automatically when the screen appears.
		introductionConfiguration.ShowAutomatically = true;

		// Create a new introduction item.
		var firstExampleEntry = new IntroListEntry();

		// Configure the introduction image to be shown.
		//		firstExampleEntry.Image = IntroImage.ReceiptsIntroImage;

		// Configure the text.
		firstExampleEntry.Text = new StyledText
		{
			Visible = true,
			Text = "Some text explaining how to scan a receipt",
			Color = new ColorValue("#000000"),
			UseShadow = false
		};

		// Create a second introduction item.
		var secondExampleEntry = new IntroListEntry();

		// Configure the introduction image to be shown.
		//secondExampleEntry.Image = IntroImage.CheckIntroImage;

		// Configure the text.
		secondExampleEntry.Text = new StyledText
		{
			Visible = true,
			Text = "Some text explaining how to scan a check",
			Color = new ColorValue("#000000"),
			UseShadow = false
		};

		// Set the items into the configuration.
		introductionConfiguration.Items = [firstExampleEntry, secondExampleEntry];

		// Set a screen title.
		introductionConfiguration.Title = new StyledText
		{
			Visible = true,
			Text = "Introduction",
			Color = new ColorValue("#000000"),
			UseShadow = true
		};

		// Apply the introduction configuration.
		configuration.Screens.Camera.Introduction = introductionConfiguration;

		try
		{
			var document = await ScanbotSDKMain.RTU.DocumentScanner.LaunchAsync(configuration);
			// Handle the document.
		}
		catch (TaskCanceledException)
		{
			// Indicates that the cancel button was tapped.
		}
	}
}