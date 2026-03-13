using System.ComponentModel;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithTools.Services.Tools;

public class CalculatorTool
{
    public static AIFunction CreateAIFunction(IServiceProvider services)
        => AIFunctionFactory.Create(
            services.GetRequiredService<CalculatorTool>().Calculate,
            name: "calculate",
            serializerOptions: ToolJsonContext.Default.Options);

    [Description("Performs mathematical calculations. Takes a mathematical expression using operators like +, -, *, / and returns the result.")]
    public CalculationResult Calculate(
        [Description("A mathematical expression using symbols, not words. Use +, -, *, / operators. Examples: '2+2', '100/3', '3.5*12', '25% of 100', 'sqrt(16)', '2^3'")] string expression)
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
            if (expression.Contains("% of", StringComparison.OrdinalIgnoreCase))
                return CalculatePercentage(expression);

            expression = PreprocessExpression(expression);

            // DataTable.Compute uses the current thread culture for parsing
            // numeric literals, so we temporarily switch to InvariantCulture
            // to ensure "3.5" is always parsed as a decimal, not locale-dependent.
            var previousCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var table = new DataTable();
                var result = table.Compute(expression, null);
                var resultString = result?.ToString() ?? "0";
                var numericResult = double.Parse(resultString, CultureInfo.InvariantCulture);

                if (double.IsInfinity(numericResult) || double.IsNaN(numericResult))
                    throw new DivideByZeroException("Division by zero");

                var formattedResult = FormatResult(numericResult);

                return new CalculationResult
                {
                    Expression = expression,
                    Result = formattedResult,
                    Steps = $"Evaluated: {expression} = {formattedResult}"
                };
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previousCulture;
            }
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

        if (!double.TryParse(percentStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var percent) ||
            !double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
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
        // Normalize unicode math symbols to ASCII equivalents
        expression = expression
            .Replace("×", "*").Replace("÷", "/")
            .Replace("\u2212", "-")  // − unicode minus
            .Replace("\u22C5", "*")  // ⋅ dot operator
            .Replace("\u00B7", "*")  // · middle dot
            .Replace("\u2217", "*")  // ∗ asterisk operator
            .Replace("\u00B2", "^2") // ² superscript 2
            .Replace("\u00B3", "^3") // ³ superscript 3
            .Replace("\u221A", "sqrt") // √ square root symbol
            .Replace("\u03C0", "pi");  // π greek pi

        // Normalize natural language operators that the AI may send
        // (e.g., "100 divided by 3" → "100/3", "3.5 times 12" → "3.5*12")
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\s+divided\s+by\s+", "/",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\s+times\s+", "*",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\s+plus\s+", "+",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\s+minus\s+", "-",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\s+multiplied\s+by\s+", "*",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Replace math constants with their numeric values (word-boundary safe)
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\bpi\b", Math.PI.ToString("R", CultureInfo.InvariantCulture),
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        expression = System.Text.RegularExpressions.Regex.Replace(
            expression, @"\be\b", Math.E.ToString("R", CultureInfo.InvariantCulture),
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Evaluate math functions that DataTable.Compute doesn't support
        expression = EvaluateMathFunctions(expression, "sqrt", Math.Sqrt);
        expression = EvaluateMathFunctions(expression, "sin", Math.Sin);
        expression = EvaluateMathFunctions(expression, "cos", Math.Cos);
        expression = EvaluateMathFunctions(expression, "tan", Math.Tan);

        // Replace ^ with Power evaluation since DataTable doesn't support it
        expression = EvaluatePowerOperator(expression);

        return expression;
    }

    private static string EvaluateMathFunctions(string expression, string funcName, Func<double, double> func)
    {
        while (true)
        {
            var idx = expression.IndexOf(funcName + "(", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) break;

            var openParen = idx + funcName.Length;
            var depth = 1;
            var pos = openParen + 1;
            while (pos < expression.Length && depth > 0)
            {
                if (expression[pos] == '(') depth++;
                else if (expression[pos] == ')') depth--;
                pos++;
            }

            if (depth != 0) break;

            var inner = expression[(openParen + 1)..(pos - 1)];
            // Recursively preprocess the inner expression, then evaluate via DataTable
            var innerProcessed = PreprocessExpression(inner);
            var table = new DataTable();
            var innerResult = table.Compute(innerProcessed, null);
            var innerValue = Convert.ToDouble(innerResult, CultureInfo.InvariantCulture);
            var result = func(innerValue);

            expression = string.Concat(
                expression.AsSpan(0, idx),
                result.ToString("R", CultureInfo.InvariantCulture),
                expression.AsSpan(pos));
        }

        return expression;
    }

    private static string EvaluatePowerOperator(string expression)
    {
        // Handle base^exponent by finding ^ and evaluating Math.Pow
        while (expression.Contains('^'))
        {
            var caretIdx = expression.IndexOf('^');

            // Find the base (number or closing paren before ^)
            var baseEnd = caretIdx;
            var baseStart = caretIdx - 1;
            while (baseStart >= 0 && (char.IsDigit(expression[baseStart]) || expression[baseStart] == '.'))
                baseStart--;
            baseStart++;

            // Find the exponent (number or opening paren after ^)
            var expStart = caretIdx + 1;
            var expEnd = expStart;
            if (expEnd < expression.Length && expression[expEnd] == '-') expEnd++;
            while (expEnd < expression.Length && (char.IsDigit(expression[expEnd]) || expression[expEnd] == '.'))
                expEnd++;

            if (baseStart >= baseEnd || expStart >= expEnd) break;

            var baseStr = expression[baseStart..baseEnd];
            var expStr = expression[expStart..expEnd];

            if (!double.TryParse(baseStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var baseVal) ||
                !double.TryParse(expStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var expVal))
                break;

            var result = Math.Pow(baseVal, expVal);
            expression = string.Concat(
                expression.AsSpan(0, baseStart),
                result.ToString("R", CultureInfo.InvariantCulture),
                expression.AsSpan(expEnd));
        }

        return expression;
    }

    private static string FormatResult(double value)
    {
        if (Math.Abs(value - Math.Round(value)) < 0.0000001)
            return Math.Round(value).ToString("N0", CultureInfo.InvariantCulture);
        else if (Math.Abs(value) >= 1000000)
            return value.ToString("N2", CultureInfo.InvariantCulture);
        else
            return Math.Round(value, 4).ToString("G", CultureInfo.InvariantCulture);
    }

    public record CalculationResult
    {
        public required string Expression { get; init; }
        public required string Result { get; init; }
        public string? Steps { get; init; }
    }
}
