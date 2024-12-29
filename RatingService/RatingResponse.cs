using System.Text.Json.Serialization;

namespace RatingService;

public class RatingResponse
{
    [JsonPropertyName("headers")]
    public required Dictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

    [JsonPropertyName("body")]
    public Rating Body { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

}
