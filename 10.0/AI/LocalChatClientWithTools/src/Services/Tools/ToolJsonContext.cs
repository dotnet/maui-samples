using System.Text.Json;
using System.Text.Json.Serialization;

namespace LocalChatClientWithTools.Services.Tools;

[JsonSourceGenerationOptions(
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true)]
// Primitives and framework types needed by AIFunctionFactory for schema generation
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(JsonElement))]
// Tool result types
[JsonSerializable(typeof(CalculatorTool.CalculationResult))]
[JsonSerializable(typeof(WeatherTool.WeatherResult))]
[JsonSerializable(typeof(WeatherTool.TemperatureUnit))]
[JsonSerializable(typeof(FileOperationsTool.FileListResult))]
[JsonSerializable(typeof(SystemInfoTool.SystemInfoResult))]
[JsonSerializable(typeof(SystemInfoTool.InfoCategory))]
[JsonSerializable(typeof(TimerTool.TimerResult))]
internal partial class ToolJsonContext : JsonSerializerContext;
