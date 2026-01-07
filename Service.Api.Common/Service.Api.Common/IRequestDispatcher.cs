using System.Text.Json.Nodes;

namespace Service.Api.Common;

public interface IRequestDispatcher
{
    Task DispatchRequest(string[] pathParts, Request<JsonNode> requestData, CancellationToken cancellationToken);
}