using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithTools.Services.Tools;

public class SystemInfoTool
{
    public static AIFunction CreateAIFunction(IServiceProvider services)
        => AIFunctionFactory.Create(
            services.GetRequiredService<SystemInfoTool>().GetSystemInfo,
            name: "get_system_info",
            serializerOptions: ToolJsonContext.Default.Options);

    [Description("Gets real device system information (battery, storage, device details)")]
    public SystemInfoResult GetSystemInfo(
        [Description("Subset of info to return")] InfoCategory infoType = InfoCategory.All)
    {
        var result = new SystemInfoResult();

        if (infoType is InfoCategory.All or InfoCategory.Battery)
        {
            result.BatteryLevel = GetBatteryLevel();
            result.BatteryState = GetBatteryState();
        }

        if (infoType is InfoCategory.All or InfoCategory.Storage)
        {
            var (total, available) = GetStorageInfo();
            result.TotalStorage = FormatBytes(total);
            result.AvailableStorage = FormatBytes(available);
        }

        if (infoType is InfoCategory.All or InfoCategory.Device)
        {
            result.DeviceName = DeviceInfo.Current.Name;
            result.DeviceModel = DeviceInfo.Current.Model;
            result.DeviceManufacturer = DeviceInfo.Current.Manufacturer;
            result.Platform = DeviceInfo.Current.Platform.ToString();
            result.Version = DeviceInfo.Current.VersionString;
        }

        return result;
    }

    private static double? GetBatteryLevel()
    {
        try
        {
            var level = Battery.Default.ChargeLevel;
            return level >= 0 ? Math.Round(level * 100, 1) : null;
        }
        catch { return null; }
    }

    private static string? GetBatteryState()
    {
        try
        {
            return Battery.Default.State.ToString();
        }
        catch { return null; }
    }

    private static string? FormatBytes(long? bytes)
    {
        if (bytes is null) return null;
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double size = bytes.Value;
        int i = 0;
        while (size >= 1024 && i < units.Length - 1) { size /= 1024; i++; }
        return $"{size:0.#} {units[i]}";
    }

    private static (long? Total, long? Available) GetStorageInfo()
    {
        try
        {
            var appDataPath = FileSystem.Current.AppDataDirectory;
            var driveInfo = new DriveInfo(appDataPath);
            return (driveInfo.TotalSize, driveInfo.AvailableFreeSpace);
        }
        catch { return (null, null); }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<InfoCategory>))]
    public enum InfoCategory
    {
        All,
        Battery,
        Storage,
        Device
    }

    public record SystemInfoResult
    {
        public double? BatteryLevel { get; set; }
        public string? BatteryState { get; set; }
        public string? AvailableStorage { get; set; }
        public string? TotalStorage { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceManufacturer { get; set; }
        public string? Platform { get; set; }
        public string? Version { get; set; }
    }
}
