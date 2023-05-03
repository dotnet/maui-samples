using System.Text.Json.Serialization;

namespace WeatherTwentyOne.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
public class Minimum
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }
}

public class Maximum
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; }
}

public class Temperature
{
    [JsonPropertyName("minimum")]
    public Minimum Minimum { get; set; }

    [JsonPropertyName("maximum")]
    public Maximum Maximum { get; set; }
}

public class Day
{
    [JsonPropertyName("phrase")]
    public string Phrase { get; set; }
}

public class Night
{
    [JsonPropertyName("phrase")]
    public string Phrase { get; set; }
}

public class Forecast
{
    [JsonPropertyName("dateTime")]
    public DateTime DateTime { get; set; }

    [JsonPropertyName("temperature")]
    public Temperature Temperature { get; set; }

    [JsonPropertyName("day")]
    public Day Day { get; set; }

    [JsonPropertyName("night")]
    public Night Night { get; set; }
}

public class ForecastsPayload
{
    [JsonPropertyName("forecasts")]
    public List<Forecast> Forecasts { get; set; }
}
