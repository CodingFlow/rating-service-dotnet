using System.Text.Json.Serialization;

namespace Service.Api.Common;

public readonly record struct Request<T>
{
    [JsonPropertyName("originReplyTo")]
    public string OriginReplyTo { get; init; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; init; }

    [JsonPropertyName("queryParameters")]
    public Dictionary<string, string> QueryParameters { get; init; }

    [JsonPropertyName("body")]
    public T Body { get; init; }
}
