using System.Text.Json.Serialization;
using RatingService.Application.Models;

namespace RatingService.Application.Commands;

public readonly struct PostUsersCommand
{
    [JsonPropertyName("items")]
    public User[] Items { get; init; }
}
