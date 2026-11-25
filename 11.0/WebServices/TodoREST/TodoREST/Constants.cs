namespace TodoREST
{
    public static class Constants
    {
        // URL of REST service
        //public static string RestUrl = "https://dotnetmauitodorest.azurewebsites.net/api/todoitems/{0}";

        // URL of REST service (Android does not use localhost)
        // Use http cleartext for local deployment. Change to https for production
        public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string Scheme = "https"; // or http
        public static string Port = "5001";
        public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/api/todoitems/{{0}}";
    }
}
