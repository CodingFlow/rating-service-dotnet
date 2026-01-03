using System.Net;
using System.Text.Json.Serialization;

namespace Service.Api.Common;

public record struct ValidationError
{
    [JsonIgnore]
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("errorCode")]
    public ErrorCode ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

}