using System.Text.Json.Serialization;
using RatingService.Application.Models;

namespace RatingService.Application.QueryResponses;

public readonly struct GetUsersQueryResponse
{
    [JsonPropertyName("items")]
    public User[] Items { get; init; }
}
