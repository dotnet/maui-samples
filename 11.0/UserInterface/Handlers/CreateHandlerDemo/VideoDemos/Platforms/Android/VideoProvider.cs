using Android.Content;
using Android.Content.Res;
using Android.Database;
using Debug = System.Diagnostics.Debug;
using Uri = Android.Net.Uri;

namespace VideoDemos.Platforms.Android
{
    [ContentProvider(new string[] { "com.companyname.videodemos" })]
    public class VideoProvider : ContentProvider
    {
        public override AssetFileDescriptor OpenAssetFile(Uri uri, string mode)
        {
            var assets = Context.Assets;
            string fileName = uri.LastPathSegment;
            if (fileName == null)
                throw new FileNotFoundException();

            AssetFileDescriptor afd = null;
            try
            {
                afd = assets.OpenFd(fileName);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
            }
            return afd;
        }

        public override int Delete(Uri uri, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }

        public override string GetType(Uri uri)
        {
            throw new NotImplementedException();
        }

        public override Uri Insert(Uri uri, ContentValues values)
        {
            throw new NotImplementedException();
        }

        public override bool OnCreate()
        {
            return false;
        }

        public override ICursor Query(Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            throw new NotImplementedException();
        }

        public override int Update(Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }
    }
}
