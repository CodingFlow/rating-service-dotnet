using System.Text.Json.Serialization;
using TestProject.Application.Models;

namespace TestProject.Application.Commands;

public readonly record struct PostRatingsCommand()
{
    [JsonPropertyName("items")]
    public IEnumerable<Rating> Items { get; init; } = new List<Rating>();
}