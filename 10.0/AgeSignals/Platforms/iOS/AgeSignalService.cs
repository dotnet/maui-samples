using AgeSignals.Services;
using Microsoft.Extensions.Logging;

namespace AgeSignals.Platforms.iOS;

public class AgeSignalService : DeclaredAgeRangeServiceBase
{
    protected override string PlatformName => "iOS";

    public AgeSignalService(ILogger<DeclaredAgeRangeServiceBase> logger) : base(logger)
    {
    }

    public override bool IsSupported()
    {
        return OperatingSystem.IsIOSVersionAtLeast(18, 0);
    }
}
