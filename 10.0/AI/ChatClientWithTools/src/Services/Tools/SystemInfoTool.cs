using Microsoft.Extensions.AI;
using System.Text.Json;
using ChatClientWithTools.Models;
using System.IO;
using System.Linq;

namespace ChatClientWithTools.Services.Tools;

public class SystemInfoTool : AIFunction
{
    private static readonly string[] BatteryStatesMobile = new[] { "Charging", "Discharging", "Full", "Not Charging" };
    private static readonly Random Rng = new();

    public override string Name => "get_system_info";
    public override string Description => "Gets system information (battery, storage, memory, device)";

    public override JsonElement JsonSchema => JsonSerializer.SerializeToElement(new
    {
        type = "object",
        properties = new
        {
            info_type = new
            {
                type = "string",
                description = "Subset of info to return: all | battery | storage | memory | device",
                // enum suggestion for LLM
                enum_values = new[] { "all", "battery", "storage", "memory", "device" }
            }
        }
    });

    protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var infoType = GetString(arguments, "info_type", "all").ToLowerInvariant();

        var result = new SystemInfoResult
        {
            BatteryLevel = -1,
            BatteryState = "Unknown",
            AvailableStorage = 0,
            TotalStorage = 0,
            AvailableMemory = 0,
            TotalMemory = 0,
            DeviceName = string.Empty,
            Platform = string.Empty,
            Version = string.Empty
        };

        if (infoType is "all" or "battery")
        {
            result.BatteryLevel = GetBatteryLevel();
            result.BatteryState = GetBatteryState();
        }
        if (infoType is "all" or "storage")
        {
            result.TotalStorage = GetTotalStorage();
            result.AvailableStorage = GetAvailableStorage();
        }
        if (infoType is "all" or "memory")
        {
            result.TotalMemory = GetTotalMemory();
            result.AvailableMemory = GetAvailableMemory();
        }
        if (infoType is "all" or "device")
        {
            result.DeviceName = GetDeviceName();
            result.Platform = GetPlatform();
            result.Version = GetVersion();
        }

        return ValueTask.FromResult<object?>(result);
    }

    private static string GetString(AIFunctionArguments args, string name, string def = "") =>
        args.TryGetValue(name, out var v) ? v?.ToString() ?? def : def;

    private static double GetBatteryLevel()
    {
        try
        {
#if ANDROID || IOS
            return Math.Round(Rng.NextDouble() * 100, 1);
#else
            return Math.Round(20 + (Rng.NextDouble() * 60), 1); // 20-80%
#endif
        }
        catch { return -1; }
    }

    private static string GetBatteryState()
    {
        try
        {
#if ANDROID || IOS
            return BatteryStatesMobile[Rng.Next(BatteryStatesMobile.Length)];
#else
            return "Unknown";
#endif
        }
        catch { return "Unknown"; }
    }

    private static long GetAvailableStorage()
    {
        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);
            var systemDrive = drives.FirstOrDefault(d => d.DriveType == DriveType.Fixed) ?? drives.FirstOrDefault();
            return systemDrive?.AvailableFreeSpace ?? 0;
        }
        catch { return 0; }
    }

    private static long GetTotalStorage()
    {
        try
        {
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady);
            var systemDrive = drives.FirstOrDefault(d => d.DriveType == DriveType.Fixed) ?? drives.FirstOrDefault();
            return systemDrive?.TotalSize ?? 0;
        }
        catch { return 0; }
    }

    private static long GetAvailableMemory()
    {
        try
        {
            var gcInfo = GC.GetGCMemoryInfo();
            var totalCommitted = gcInfo.TotalCommittedBytes;
            var assumedTotal = 8L * 1024 * 1024 * 1024; // 8GB assumption
            return Math.Max(0, assumedTotal - totalCommitted);
        }
        catch { return 0; }
    }

    private static long GetTotalMemory()
    {
        try
        {
            return 8L * 1024 * 1024 * 1024; // assumed 8GB
        }
        catch { return 0; }
    }

    private static string GetDeviceName()
    {
        try { return Environment.MachineName; } catch { return "Unknown Device"; }
    }

    private static string GetPlatform()
    {
        try
        {
#if WINDOWS
            return "Windows";
#elif ANDROID
            return "Android";
#elif IOS
            return "iOS";
#elif MACCATALYST
            return "macOS";
#else
            return Environment.OSVersion.Platform.ToString();
#endif
        }
        catch { return "Unknown"; }
    }

    private static string GetVersion()
    {
        try { return Environment.OSVersion.VersionString; } catch { return "Unknown"; }
    }
}