using System;
using Android.Content;
using EmployeeDirectory;
using Android;

namespace EmployeeDirectory.Platforms.Android
{
    public class AndroidPhoneFeatureService : IPhoneFeatureService
    {
        MainActivity owner;

        public AndroidPhoneFeatureService(MainActivity owner)
        {
            this.owner = owner;
        }

        public bool Email(string emailAddress)
        {
            try
            {
                var intent = new Intent(Intent.ActionSend);
                intent.SetType("message/rfc822");
                intent.PutExtra(Intent.ExtraEmail, new[] { emailAddress });
                if (owner != null)
                {
                    owner.StartActivity(Intent.CreateChooser(intent, "Send email"));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Browse(string websiteUrl)
        {
            try
            {
                var url = websiteUrl.ToUpperInvariant().StartsWith("HTTP") ?
                    websiteUrl :
                    "http://" + websiteUrl;

                var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(url));
                if (owner != null)
                {
                    owner.StartActivity(intent);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Tweet(string twitterName)
        {
            try
            {
                var username = twitterName.Trim();
                if (username.StartsWith("@"))
                    username = username.Substring(1);

                var url = "https://twitter.com/" + username;
                var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(url));
                if (owner != null)
                {
                    owner.StartActivity(intent);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Call(string phoneNumber)
        {
            try
            {
                var intent = new Intent(Intent.ActionCall, global::Android.Net.Uri.Parse(
                                 "tel:" + Uri.EscapeDataString(phoneNumber)));
                if (owner != null)
                {
                    owner.StartActivity(intent);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Map(string address)
        {
            try
            {
                var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(
                    "geo:0,0?q=" + Uri.EscapeDataString(address)));
                if (owner != null)
                {
                    owner.StartActivity(intent);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}

