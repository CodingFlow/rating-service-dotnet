using System.Text.Json;
using System.Text.Json.Nodes;
using NATS.Client.JetStream;
using NATS.Net;
using RatingService.Handlers;

namespace RatingService;

internal class Main(IGetUsersHandler getUsersHandler, IPostUsersHandler postUsersHandler) : IMain
{
    public async Task Run()
    {
        var host = Environment.GetEnvironmentVariable("NATS_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable("NATS_SERVICE_PORT");
        var url = $"nats://{host}:{port}";

        var client = new NatsClient(url);

        var jetStream = client.CreateJetStreamContext();
        var consumer = await jetStream.GetConsumerAsync("mystream", "my-pull-consumer");

        Console.WriteLine("Ready to process messages");

        var messages = consumer.ConsumeAsync<Request<JsonNode>>();

        await Parallel.ForEachAsync(messages, async (message, cancellationToken) => await HandleMessage(client, message, cancellationToken));
    }

    private async ValueTask HandleMessage(NatsClient client, NatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        Console.WriteLine("processing message");
        var splitSubject = ExtractHttpMethod(message);

        Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");

        _ = HandleRequestAsync(client, splitSubject, message, cancellationToken);

        await message.AckAsync(cancellationToken: cancellationToken);
    }

    private async Task HandleRequestAsync(NatsClient client, (string httpMethod, string pathPart) splitSubject, NatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"httpMethod: {splitSubject.httpMethod} -- pathPart: {splitSubject.pathPart}");
        switch (splitSubject)
        {
            case ("get", "users"):
                var responseBody = getUsersHandler.HandleGetUsers();
                await client.PublishAsync(message.Data.OriginReplyTo, new Response<User[]>
                {
                    StatusCode = 200,
                    Body = responseBody,
                    Headers = []
                },
                cancellationToken: cancellationToken);
                break;
            case ("post", "users"):
                Console.WriteLine($"Received request body: {message.Data.Body}");
                var requestBody = message.Data.Body.Deserialize<User>();
                var request = new Request<User>
                {
                    Headers = message.Data.Headers,
                    OriginReplyTo = message.Data.OriginReplyTo,
                    Body = requestBody
                };
                Console.WriteLine($"Post request body username: {request.Body.Username}");
                var responseBodyPost = postUsersHandler.HandlePostUsersAsync(requestBody);

                await client.PublishAsync(message.Data.OriginReplyTo, new Response<string>
                {
                    StatusCode = 201,
                    Body = responseBodyPost,
                    Headers = []
                },
                cancellationToken: cancellationToken);
                break;
        }
    }

    private static (string httpMethod, string pathPart) ExtractHttpMethod<T>(NatsJSMsg<Request<T>> message)
    {
        var firstPartIndex = message.Subject.IndexOf('.');
        var secondPartIndex = message.Subject.IndexOf('.', firstPartIndex + 1);

        if (secondPartIndex == -1)
        {
            secondPartIndex = message.Subject.Length;
        }

        Console.WriteLine($"first index: {firstPartIndex} -- second index: {secondPartIndex}");

        return (message.Subject[..firstPartIndex], message.Subject[(firstPartIndex + 1)..secondPartIndex]);
    }
}
