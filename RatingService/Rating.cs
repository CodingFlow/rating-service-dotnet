using System.Text.Json.Serialization;

namespace RatingService;

public class Rating
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; }
}