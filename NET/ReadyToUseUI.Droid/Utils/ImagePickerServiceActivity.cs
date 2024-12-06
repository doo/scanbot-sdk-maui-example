using Android.Content;
using Android.Graphics;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.AppCompat.App;
using ActivityResult = AndroidX.Activity.Result.ActivityResult;

namespace ReadyToUseUI.Droid.Utils;

internal class ImagePickerServiceActivity : Java.Lang.Object, IActivityResultCallback
{
	public static Task<Bitmap> PickImageAsync(AppCompatActivity activity)
	{
		var picker = new ImagePickerServiceActivity();
		picker.activity = activity;
		return picker.PickImage(activity);
	}
	
	private TaskCompletionSource<Bitmap> _taskCompletionSource;
	private ActivityResultLauncher ImagePickerLauncher;
	private AppCompatActivity activity;
        
	private Task<Bitmap> PickImage(AppCompatActivity activity)
	{
		_taskCompletionSource = new TaskCompletionSource<Bitmap>();
		ImagePickerLauncher = activity.RegisterForActivityResult(new ActivityResultContracts.StartActivityForResult(), this);
		
		// Define the Intent for getting images
		var intent = new Intent();
		intent.SetType("image/*");
		intent.SetAction(Intent.ActionGetContent);
		intent.PutExtra(Intent.ExtraLocalOnly, false);
		intent.PutExtra(Intent.ExtraAllowMultiple, false);
		intent.PutExtra(Intent.ExtraMimeTypes, ["image/jpeg", "image/png", "image/webp", "image/heic"]);
		intent.SetAction(Intent.ActionGetContent);
		
		var chooser = Intent.CreateChooser(intent, "Gallery");
		ImagePickerLauncher.Launch(chooser);
		return _taskCompletionSource.Task;
	}

	public void OnActivityResult(Java.Lang.Object result)
	{
		var activityResult = result  as ActivityResult;
		if (activityResult.Data == null ) return;
		
		_taskCompletionSource.SetResult(ImageUtils.ProcessGalleryResult(activity, activityResult.Data));
	}
}