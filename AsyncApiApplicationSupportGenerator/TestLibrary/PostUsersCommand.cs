using System.Text.Json.Serialization;
using TestProject.Models;

namespace TestProject.Commands;

public readonly struct PostUsersCommand
{
    [JsonPropertyName("items")]
    public User[] Items { get; init; }
}
