using UIKit;
using CustomRenderer;
using CustomRenderer.iOS;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;

namespace CustomRenderer.iOS
{
	public class MyEntryRenderer : EntryRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (Control != null) {
				// do whatever you want to the UITextField here!
				Control.BackgroundColor = UIColor.FromRGB (204, 153, 255);
				Control.BorderStyle = UITextBorderStyle.Line;
			}
		}
	}
}