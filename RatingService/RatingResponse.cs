using System.Text.Json.Serialization;

namespace RatingService;

public struct RatingResponse
{
    [JsonPropertyName("headers")]
    public required Dictionary<string, string> Headers { get; init; }

    [JsonPropertyName("body")]
    public Rating Body { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

}
