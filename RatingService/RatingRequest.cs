using System.Text.Json.Serialization;

namespace RatingService;

public struct RatingRequest<T>
{
    [JsonPropertyName("originReplyTo")]
    public string OriginReplyTo { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; }

    [JsonPropertyName("body")]
    public T Body { get; set; }
}
