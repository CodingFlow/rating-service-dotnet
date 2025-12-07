using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;
using RatingService.Api;
using RatingService.Application.Handlers;

namespace RatingService;

internal partial class MainHandler(IGetUsersHandler getUsersHandler, IPostUsersHandler postUsersHandler)
{
    private async Task DispatchRequest(NatsClient client, (string httpMethod, string pathPart) splitSubject, NatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        switch (splitSubject)
        {
            case ("get", "users"):
                await HandleGet(client, message, getUsersHandler, cancellationToken);
                break;
            case ("post", "users"):
                await HandlePost(client, message, postUsersHandler, cancellationToken);
                break;
        }
    }
}
