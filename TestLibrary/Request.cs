using System.Text.Json.Serialization;

namespace TestLibrary;

public struct Request<T>
{
    [JsonPropertyName("originReplyTo")]
    public string OriginReplyTo { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; }

    [JsonPropertyName("body")]
    public T Body { get; set; }
}
