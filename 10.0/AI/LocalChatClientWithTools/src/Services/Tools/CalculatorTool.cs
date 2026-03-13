using System.ComponentModel;
using System.Data;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithTools.Services.Tools;

public class CalculatorTool
{
    public static AIFunction CreateAIFunction(IServiceProvider services)
        => AIFunctionFactory.Create(
            services.GetRequiredService<CalculatorTool>().Calculate,
            name: "calculate",
            serializerOptions: ToolJsonContext.Default.Options);

    [Description("Performs mathematical calculations and evaluates expressions")]
    public CalculationResult Calculate(
        [Description("The mathematical expression to evaluate (e.g., '2+2', '25% of 100', 'sqrt(16)')")] string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return new CalculationResult
            {
                Expression = expression ?? string.Empty,
                Result = "(empty)",
                Steps = "Provide an expression to evaluate."
            };
        }

        try
        {
            if (expression.Contains("% of"))
                return CalculatePercentage(expression);

            expression = PreprocessExpression(expression);

            var table = new DataTable();
            var result = table.Compute(expression, null);
            var resultString = result?.ToString() ?? "0";
            var formattedResult = FormatResult(double.Parse(resultString));

            return new CalculationResult
            {
                Expression = expression,
                Result = formattedResult,
                Steps = $"Evaluated: {expression} = {formattedResult}"
            };
        }
        catch (Exception ex)
        {
            return new CalculationResult
            {
                Expression = expression,
                Result = "error",
                Steps = $"Error: {ex.Message}"
            };
        }
    }

    private static CalculationResult CalculatePercentage(string expression)
    {
        var parts = expression.ToLower().Split(" of ");
        if (parts.Length != 2)
            throw new ArgumentException("Invalid percentage format");

        var percentStr = parts[0].Replace("%", "").Trim();
        var valueStr = parts[1].Trim();

        if (!double.TryParse(percentStr, out var percent) || !double.TryParse(valueStr, out var value))
            throw new ArgumentException("Invalid numbers in percentage calculation");

        var result = (percent / 100.0) * value;
        var formattedResult = FormatResult(result);

        return new CalculationResult
        {
            Expression = expression,
            Result = formattedResult,
            Steps = $"{percent}% of {value} = ({percent}/100) × {value} = {formattedResult}"
        };
    }

    private static string PreprocessExpression(string expression)
    {
        expression = expression.Replace("sqrt(", "Math.Sqrt(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("sin(", "Math.Sin(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("cos(", "Math.Cos(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("tan(", "Math.Tan(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("pi", "Math.PI", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("e", "Math.E", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("^", "**");
        return expression;
    }

    private static string FormatResult(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < 0.0000001)
            return Math.Round(value).ToString("N0");
        else if (Math.Abs(value) >= 1000000)
            return value.ToString("N2");
        else
            return Math.Round(value, 4).ToString("G");
    }

    public record CalculationResult
    {
        public required string Expression { get; init; }
        public required string Result { get; init; }
        public string? Steps { get; init; }
    }
}
