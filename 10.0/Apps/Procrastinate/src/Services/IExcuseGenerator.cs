namespace procrastinate.Services;

public record ExcuseResult(
    string Excuse,
    string GeneratorName,
    TimeSpan Duration,
    int? TokenCount = null,
    string? Model = null
);

public interface IExcuseGenerator
{
    string Name { get; }
    Task<ExcuseResult> GenerateExcuseAsync(string language);
    bool IsAvailable { get; }
}
