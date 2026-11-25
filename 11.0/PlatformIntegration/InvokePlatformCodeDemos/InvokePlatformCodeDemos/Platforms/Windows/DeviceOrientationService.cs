namespace InvokePlatformCodeDemos.Services.PartialMethods;

public partial class DeviceOrientationService
{
    public partial DeviceOrientation GetOrientation()
    {
        return DeviceOrientation.Undefined;
        //throw new PlatformNotSupportedException("GetOrientation is only supported on Android and iOS.");
    }
}
