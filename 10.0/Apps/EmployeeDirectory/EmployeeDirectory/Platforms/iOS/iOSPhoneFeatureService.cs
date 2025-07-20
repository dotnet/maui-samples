using System;
using System.Collections.Generic;
using Foundation;
using MessageUI;
using UIKit;
using EmployeeDirectory;

namespace EmployeeDirectory.Platforms.iOS
{
	public class iOSPhoneFeatureService : IPhoneFeatureService
	{
		public UIViewController RootViewController { get; private set; }

		public iOSPhoneFeatureService (UIViewController rootViewController)
		{
			RootViewController = rootViewController;
		}

		public bool Email (string emailAddress)
		{
			if (MFMailComposeViewController.CanSendMail) {
				var composer = new MFMailComposeViewController ();
				composer.SetToRecipients (new string[] { emailAddress });
				composer.SetSubject ("Hello from EmployeeDirectory!");

				composer.Finished += (sender, e) => RootViewController.DismissViewController (true, null);
				RootViewController.PresentViewController (composer, true, null);
				return true;
			} else {
				return false;
			}
		}

		public bool Browse (string websiteUrl)
		{
			var url = websiteUrl.ToUpperInvariant().StartsWith("HTTP") ?
				websiteUrl :
				"http://" + websiteUrl;

			UIApplication.SharedApplication.OpenUrl (NSUrl.FromString (url));
			return true;
		}

		public bool Tweet (string twitterName)
		{
			string messageText = string.Format ("Let me introduce to you, " +
			                     "the one and only {0} using #xamarin EmployeeDirectory!", twitterName);

			var url = NSUrl.FromString ($"https://twitter.com/intent/tweet?text={Uri.EscapeDataString(messageText)}");
			UIApplication.SharedApplication.OpenUrl (url);
			return true;
		}

		public bool Call (string phoneNumber)
		{
			var url = NSUrl.FromString ("tel:" + Uri.EscapeDataString (phoneNumber));

			if (UIApplication.SharedApplication.CanOpenUrl (url)) {
				UIApplication.SharedApplication.OpenUrl (url);
				return true;
			} else {
				return false;
			}
		}

		public bool Map (string address)
		{
			UIApplication.SharedApplication.OpenUrl (
				NSUrl.FromString ("http://maps.google.com/maps?q=" + Uri.EscapeDataString (address)));

			return true;
		}
	}
}

