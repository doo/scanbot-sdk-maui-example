// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ClassicComponent.iOS
{
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		UIKit.UIImageView imageViewDocument { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint imageViewHiddenConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint imageViewVisibleConstraint { get; set; }

		[Outlet]
		UIKit.UILabel lblHint { get; set; }

		[Outlet]
		UIKit.UITableView tableViewSDKFeatures { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imageViewDocument != null) {
				imageViewDocument.Dispose ();
				imageViewDocument = null;
			}

			if (lblHint != null) {
				lblHint.Dispose ();
				lblHint = null;
			}

			if (tableViewSDKFeatures != null) {
				tableViewSDKFeatures.Dispose ();
				tableViewSDKFeatures = null;
			}

			if (imageViewHiddenConstraint != null) {
				imageViewHiddenConstraint.Dispose ();
				imageViewHiddenConstraint = null;
			}

			if (imageViewVisibleConstraint != null) {
				imageViewVisibleConstraint.Dispose ();
				imageViewVisibleConstraint = null;
			}
		}
	}
}
