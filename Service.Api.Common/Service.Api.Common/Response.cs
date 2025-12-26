using System.Text.Json.Serialization;

namespace Service.Api.Common;

public readonly record struct Response<T>
{
    [JsonPropertyName("headers")]
    public required Dictionary<string, string> Headers { get; init; }

    [JsonPropertyName("body")]
    public T Body { get; init; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

}
