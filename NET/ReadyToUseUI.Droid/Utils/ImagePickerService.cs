using Android.Content;
using Android.Graphics;
using Scanbot.ImagePicker.Droid.Utils;

namespace ReadyToUseUI.Droid.Utils;

internal class ImagePickerServiceActivity
{
	internal static async Task<Bitmap> PickImageAsync(Activity activity)
	{ 
		// Define the Intent for getting images
		var intent = new Intent();
		intent.SetType("image/*");
		intent.SetAction(Intent.ActionGetContent);

		var chooser = Intent.CreateChooser(intent, "Gallery");
		var result = await ActivityResultDispatcher.ReceiveProxyActivityResult(activity, chooser);

		if (result.Result != Android.App.Result.Ok || result.Intent == null) return null;
		
		var bitmap = ImageUtils.ProcessGalleryResult(activity, result.Intent);
		return bitmap;
	}
}