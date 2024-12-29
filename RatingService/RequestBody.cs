using System.Text.Json.Serialization;

internal struct RequestBody
{
    [JsonPropertyName("animal")]
    public string Animal { get; set; }
}