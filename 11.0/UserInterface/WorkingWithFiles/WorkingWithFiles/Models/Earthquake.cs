namespace WorkingWithFiles.Models;

public class Earthquake
{
    public string eqid { get; set; } = string.Empty;
    public float magnitude { get; set; }
    public float lng { get; set; }
    public string src { get; set; } = string.Empty;
    public string datetime { get; set; } = string.Empty;
    public float depth { get; set; }
    public float lat { get; set; }

    public string Data
    {
        get { return $"{lat}, {lng}, {magnitude}, {depth}"; }
    }

    public string Summary => ToString();

    public override string ToString()
    {
        return $"Date: {datetime[..10]}, Magnitude: {magnitude}";
    }
}

public class Rootobject
{
    public Earthquake[] earthquakes { get; set; } = [];
}
