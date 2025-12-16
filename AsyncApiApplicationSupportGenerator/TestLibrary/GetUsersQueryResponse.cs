using System.Text.Json.Serialization;
using TestProject.Models;

namespace TestProject.QueryResponses;

public readonly struct GetUsersQueryResponse
{
    [JsonPropertyName("items")]
    public User[] Items { get; init; }
}
