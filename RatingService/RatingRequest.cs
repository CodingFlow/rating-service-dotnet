using System.Text.Json.Serialization;

namespace RatingService;

public class RatingRequest
{
    [JsonPropertyName("originReplyTo")]
    public string OriginReplyTo { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; }
}
