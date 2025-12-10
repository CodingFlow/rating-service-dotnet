using System.Text.Json.Serialization;

namespace Service.Api.Common;

public struct Request<T>
{
    [JsonPropertyName("originReplyTo")]
    public string OriginReplyTo { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; }

    [JsonPropertyName("body")]
    public T Body { get; set; }
}
