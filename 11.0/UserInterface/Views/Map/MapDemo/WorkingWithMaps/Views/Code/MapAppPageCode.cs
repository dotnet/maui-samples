namespace WorkingWithMaps.Views.Code;

public class MapAppPageCode : ContentPage
{
    // WARNING: when adding latitude/longitude values be careful of localization.
    // European (and other countries) use a comma as the separator, which will break the request
    public MapAppPageCode()
    {
        Title = "Native map app demo";

        var l = new Label
        {
            Text = "These buttons leave the current app and open the built-in Maps app for the platform"
        };

        var openLocation = new Button
        {
            Text = "Open location using built-in Maps app"
        };
        openLocation.Clicked += async (sender, e) =>
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS || DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
            {
                //https://developer.apple.com/library/ios/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
                await Launcher.OpenAsync("http://maps.apple.com/?q=394+Pacific+Ave+San+Francisco+CA");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // opens the Maps app directly
                await Launcher.OpenAsync("geo:0,0?q=394+Pacific+Ave+San+Francisco+CA");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                await Launcher.OpenAsync("bingmaps:?where=394 Pacific Ave San Francisco CA");
            }
        };

        var openDirections = new Button
        {
            Text = "Get directions using built-in Maps app"
        };
        openDirections.Clicked += async (sender, e) =>
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS || DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst)
            {
                //https://developer.apple.com/library/ios/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
                await Launcher.OpenAsync("http://maps.apple.com/?daddr=San+Francisco,+CA&saddr=cupertino");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // opens the 'task chooser' so the user can pick Maps, Chrome or other mapping app
                await Launcher.OpenAsync("http://maps.google.com/?daddr=San+Francisco,+CA&saddr=Mountain+View");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                await Launcher.OpenAsync("bingmaps:?rtp=adr.394 Pacific Ave San Francisco CA~adr.One Microsoft Way Redmond WA 98052");
            }
        };

        StackLayout sl = new StackLayout 
        { 
            Padding = new Thickness(5, 20, 5, 0) ,
            HorizontalOptions = LayoutOptions.Fill
        };
        sl.Add(l);
        sl.Add(openLocation);
        sl.Add(openDirections);

        Content = sl;
    }
}
