using PlatformIntegrationDemo.Views;

namespace PlatformIntegrationDemo.Models;

// The demo pages shown on the home page, grouped by category.
public static class SampleCatalog
{
    public static IReadOnlyList<SampleCategory> Categories { get; }
    public static IReadOnlyList<SampleItem> Samples { get; }

    static SampleCatalog()
    {
        var appModel = new SampleCategory("App model", AccentColor("Violet"));
        var communication = new SampleCategory("Communication", AccentColor("Blue"));
        var sharing = new SampleCategory("Sharing", AccentColor("Sky"));
        var device = new SampleCategory("Device", AccentColor("Teal"));
        var location = new SampleCategory("Location", AccentColor("Green"));
        var sensors = new SampleCategory("Sensors", AccentColor("Orange"));
        var media = new SampleCategory("Media", AccentColor("Pink"));
        var storage = new SampleCategory("Storage", AccentColor("Rose"));

        Categories = [appModel, communication, sharing, device, location, sensors, media, storage];

        Samples =
        [
            new(MaterialIcons.Info, "App Info", typeof(AppInfoPage), "Read the app name, version, and theme", appModel, "app", "info", "version"),
            new(MaterialIcons.Public, "Browser", typeof(BrowserPage), "Open a website in the system or in-app browser", appModel, "browser", "web", "internet"),
            new(MaterialIcons.OpenInNew, "Launcher", typeof(LauncherPage), "Open another app or a file from a URI", appModel, "launcher", "app", "run", "uri"),
            new(MaterialIcons.AdminPanelSettings, "Permissions", typeof(PermissionsPage), "Check and request runtime permissions", appModel, "permissions"),
            new(MaterialIcons.RecordVoiceOver, "Semantic Screen Reader", typeof(SemanticScreenReaderPage), "Announce text with the screen reader", appModel, "accessibility", "a11y", "screen reader", "semantic"),

            new(MaterialIcons.Contacts, "Contacts", typeof(ContactsPage), "Pick a contact or read all contacts", communication, "contacts", "people", "device"),
            new(MaterialIcons.Email, "Email", typeof(EmailPage), "Compose an email with attachments", communication, "email", "message", "attachment", "share", "communication"),
            new(MaterialIcons.Dialpad, "Phone Dialer", typeof(PhoneDialerPage), "Open the dialer with a phone number", communication, "phone", "dialer", "call", "communication"),
            new(MaterialIcons.Sms, "SMS", typeof(SMSPage), "Compose a text message", communication, "sms", "message", "text", "communication", "share"),
            new(MaterialIcons.Wifi, "Connectivity", typeof(ConnectivityPage), "Check network access and connection profiles", communication, "connectivity", "network", "internet", "wifi"),
            new(MaterialIcons.Login, "Web Authenticator", typeof(WebAuthenticatorPage), "Sign in with a browser-based flow", communication, "auth", "authenticate", "authenticator", "oauth", "web", "webauth"),

            new(MaterialIcons.ContentPaste, "Clipboard", typeof(ClipboardPage), "Copy text to and paste from the clipboard", sharing, "clipboard", "copy", "paste"),
            new(MaterialIcons.Share, "Share", typeof(SharePage), "Share text, links, and files with other apps", sharing, "share", "data", "transfer", "file", "communication"),

            new(MaterialIcons.BatteryChargingFull, "Battery", typeof(BatteryPage), "Read the charge level, state, and power source", device, "battery", "power", "energy", "hardware"),
            new(MaterialIcons.Smartphone, "Device Info", typeof(DeviceInfoPage), "Read the model, platform, and display metrics", device, "device", "info", "display", "screen", "orientation", "rotation", "hardware"),
            new(MaterialIcons.FlashlightOn, "Flashlight", typeof(FlashlightPage), "Turn the camera flash on and off", device, "flashlight", "torch", "flash", "hardware", "device"),
            new(MaterialIcons.TouchApp, "Haptic Feedback", typeof(HapticFeedbackPage), "Play click and long-press haptics", device, "haptic", "feedback", "hardware", "device"),
            new(MaterialIcons.LightMode, "Keep Screen On", typeof(KeepScreenOnPage), "Stop the screen from going to sleep", device, "screen", "awake", "sleep", "display"),
            new(MaterialIcons.Vibration, "Vibration", typeof(VibrationPage), "Vibrate the device for a set duration", device, "vibration", "vibrate", "hardware", "device"),

            new(MaterialIcons.PinDrop, "Geocoding", typeof(GeocodingPage), "Turn addresses into coordinates and back", location, "geocoding", "address", "placemark", "geolocation", "position", "mapping"),
            new(MaterialIcons.MyLocation, "Geolocation", typeof(GeolocationPage), "Get the last known and current location", location, "geolocation", "location", "gps", "position", "address", "mapping"),
            new(MaterialIcons.Map, "Maps", typeof(MapsPage), "Open the maps app at coordinates or an address", location, "maps", "navigation", "route", "address", "geocoding", "geolocation", "position", "mapping"),

            new(MaterialIcons.Speed, "Accelerometer", typeof(AccelerometerPage), "Read acceleration and detect a shake", sensors, "accelerometer", "shake", "motion", "sensors", "hardware", "device"),
            new(MaterialIcons.Sensors, "All Sensors", typeof(AllSensorsPage), "Run every motion and environment sensor at once", sensors, "accelerometer", "barometer", "compass", "gyroscope", "magnetometer", "orientation", "sensors", "hardware", "device"),
            new(MaterialIcons.Compress, "Barometer", typeof(BarometerPage), "Read the air pressure", sensors, "barometer", "pressure", "sensor", "hardware", "device"),
            new(MaterialIcons.Explore, "Compass", typeof(CompassPage), "Read the heading relative to magnetic north", sensors, "compass", "heading", "north", "sensors", "hardware", "device"),
            new(MaterialIcons.ThreeDRotation, "Gyroscope", typeof(GyroscopePage), "Read the rotation around each axis", sensors, "gyroscope", "rotation", "sensors", "hardware", "device"),
            new(MaterialIcons.CompassCalibration, "Magnetometer", typeof(MagnetometerPage), "Read the magnetic field strength", sensors, "magnetometer", "magnetic", "compass", "sensors", "hardware", "device"),
            new(MaterialIcons.ScreenRotation, "Orientation Sensor", typeof(OrientationSensorPage), "Read the device orientation in 3D space", sensors, "orientation", "quaternion", "sensors", "hardware", "device"),

            new(MaterialIcons.PhotoLibrary, "Media Picker", typeof(MediaPickerPage), "Pick or capture a photo or video", media, "media", "photo", "video", "camera", "image", "picker", "picture", "movie"),
            new(MaterialIcons.Screenshot, "Screenshot", typeof(ScreenshotPage), "Capture the current screen", media, "screenshot", "capture", "image", "picture", "media", "display"),
            new(MaterialIcons.VolumeUp, "Text to Speech", typeof(TextToSpeechPage), "Speak text with a chosen voice", media, "text", "speech", "tts", "voice", "message", "communication"),
            new(MaterialIcons.Straighten, "Unit Converters", typeof(UnitConvertersPage), "Convert temperature and distance units", media, "units", "converters", "temperature", "distance", "calculations"),
            new(MaterialIcons.Palette, "Color Converters", typeof(ColorConvertersPage), "Parse and adjust colors", media, "color", "hex", "hue", "graphics", "drawing", "style"),

            new(MaterialIcons.FileOpen, "File Picker", typeof(FilePickerPage), "Pick one or more files", storage, "files", "picker", "document", "picking", "filesystem", "storage"),
            new(MaterialIcons.Folder, "File System", typeof(FileSystemPage), "Read and write files in app storage", storage, "files", "directory", "cache", "filesystem", "storage"),
            new(MaterialIcons.Tune, "Preferences", typeof(PreferencesPage), "Persist simple settings", storage, "settings", "preferences", "prefs", "storage"),
            new(MaterialIcons.Lock, "Secure Storage", typeof(SecureStoragePage), "Store key-value pairs encrypted", storage, "secure", "security", "keychain", "keystore", "settings", "preferences", "prefs", "storage"),
        ];
    }

    static Color AccentColor(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color
            : Colors.Gray;
}
