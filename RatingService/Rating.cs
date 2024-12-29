using System.Text.Json.Serialization;

namespace RatingService;

public struct Rating
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
}