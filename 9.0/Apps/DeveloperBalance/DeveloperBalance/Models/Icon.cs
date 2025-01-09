using System;

namespace DeveloperBalance.Models;

public class Icon
{
    public string Glyph { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;

    public Icon(string glyph, string name)
    {
        Glyph = glyph;
        Name = name;
    }
}
