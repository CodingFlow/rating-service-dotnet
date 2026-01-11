using System.Text.Json.Serialization;

namespace TestProject.Application.Models;

public readonly record struct Rating()
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }

    [JsonPropertyName("serviceId")]
    public Guid ServiceId { get; init; }

    [JsonPropertyName("score")]
    public int Score { get; init; }
}
