using System.Text.Json.Serialization;

namespace RatingService.Api;

public struct Response<T>
{
    [JsonPropertyName("headers")]
    public required Dictionary<string, string> Headers { get; init; }

    [JsonPropertyName("body")]
    public T Body { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

}
