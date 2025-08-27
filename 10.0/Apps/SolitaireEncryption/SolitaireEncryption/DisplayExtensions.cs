using System.Text;

namespace SolitaireEncryption;

public static class DisplayExtensions
{
    public static string Pad5(this string input)
    {
        var isExact = input.Length % 5 == 0;
        var pad = "";

        if (!isExact)
        {
            input = input.PadRight(input.Length + (5 - input.Length % 5), 'X');
        }

        var output = new StringBuilder();
        var chunks = input.Length / 5;

        for (var chunk = 0; chunk < chunks; chunk++)
        {
            output.Append(string.Concat(input.AsSpan(chunk * 5, 5), " "));
        }

        return output.ToString().TrimEnd(' ') + pad;
    }

    public static string PadRemove(this string input)
        => input.Replace(" ", "");
}

