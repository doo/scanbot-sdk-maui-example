// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System.CodeDom.Compiler;

namespace ClassicComponent.iOS
{
    [Register ("ThumbnailCollectionViewCell")]
    partial class ThumbnailCollectionViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView thumbnailImage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (thumbnailImage != null) {
                thumbnailImage.Dispose ();
                thumbnailImage = null;
            }
        }
    }
}