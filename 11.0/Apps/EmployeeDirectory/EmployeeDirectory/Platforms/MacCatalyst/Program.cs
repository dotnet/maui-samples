using ObjCRuntime;
using UIKit;

namespace EmployeeDirectory.Platforms.MacCatalyst
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
