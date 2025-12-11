using System.Text.Json.Serialization;

namespace RatingService.Application;

public readonly struct User
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("username")]
    public string Username { get; init; }
}