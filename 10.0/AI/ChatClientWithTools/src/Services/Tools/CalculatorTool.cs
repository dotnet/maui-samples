using Microsoft.Extensions.AI;
using System.Data;
using System.Text.Json;
using ChatClientWithTools.Models;

namespace ChatClientWithTools.Services.Tools;

public class CalculatorTool : AIFunction
{
    // AIFunction now uses parameterless base; expose metadata via overrides.
    public override string Name => "calculate";
    public override string Description => "Performs mathematical calculations and evaluates expressions";

    public override JsonElement JsonSchema => JsonSerializer.SerializeToElement(new
    {
        type = "object",
        properties = new
        {
            expression = new
            {
                type = "string",
                description = "The mathematical expression to evaluate (e.g., '2+2', '25% of 100', 'sqrt(16)')"
            }
        },
        required = new[] { "expression" }
    });

    protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var expression = GetStringArgument(arguments, "expression");

        if (string.IsNullOrWhiteSpace(expression))
        {
            return ValueTask.FromResult<object?>(new CalculationResult
            {
                Expression = expression,
                Result = "(empty)",
                Steps = "Provide an expression to evaluate."
            });
        }

        try
        {
            // Handle percentage calculations
            if (expression.Contains("% of"))
            {
                return ValueTask.FromResult<object?>(CalculatePercentage(expression));
            }

            // Handle common functions
            expression = PreprocessExpression(expression);

            // Use DataTable.Compute for basic arithmetic
            var table = new DataTable();
            var result = table.Compute(expression, null);

            var resultString = result.ToString() ?? "0";
            var formattedResult = FormatResult(double.Parse(resultString));

            return ValueTask.FromResult<object?>(new CalculationResult
            {
                Expression = expression,
                Result = formattedResult,
                Steps = $"Evaluated: {expression} = {formattedResult}"
            });
        }
        catch (Exception ex)
        {
            return ValueTask.FromResult<object?>(new CalculationResult
            {
                Expression = expression,
                Result = "error",
                Steps = $"Error: {ex.Message}"
            });
        }
    }

    private string GetStringArgument(AIFunctionArguments arguments, string name)
    {
        return arguments.TryGetValue(name, out var value) ? value?.ToString() ?? "" : "";
    }

    private CalculationResult CalculatePercentage(string expression)
    {
        try
        {
            // Parse "X% of Y" format
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
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error calculating percentage: {ex.Message}");
        }
    }

    private string PreprocessExpression(string expression)
    {
        // Replace common mathematical functions with C# equivalents
        expression = expression.Replace("sqrt(", "Math.Sqrt(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("sin(", "Math.Sin(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("cos(", "Math.Cos(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("tan(", "Math.Tan(", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("pi", "Math.PI", StringComparison.OrdinalIgnoreCase);
        expression = expression.Replace("e", "Math.E", StringComparison.OrdinalIgnoreCase);

        // Handle power operator
        expression = expression.Replace("^", "**");

        return expression;
    }

    private string FormatResult(double value)
    {
        // Format the result appropriately
        if (Math.Abs(value - Math.Round(value)) < 0.0000001)
        {
            // It's essentially an integer
            return Math.Round(value).ToString("N0");
        }
        else if (Math.Abs(value) >= 1000000)
        {
            // Large numbers - use scientific notation or shortened form
            return value.ToString("N2");
        }
        else
        {
            // Regular decimal
            return Math.Round(value, 4).ToString("G");
        }
    }
}