using System.Text.Json.Serialization;

namespace RatingService;

public class RatingResponse
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
}
