// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

namespace ClassicComponent.iOS
{
    [Register ("VINScannerViewController")]
	partial class VINScannerViewController
	{
		[Outlet]
		UIKit.UIView containerView { get; set; }

		[Outlet]
		UIKit.UILabel lblResult { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (containerView != null) {
				containerView.Dispose ();
				containerView = null;
			}

			if (lblResult != null) {
				lblResult.Dispose ();
				lblResult = null;
			}
		}
	}
}
