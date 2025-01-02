using NATS.Client.JetStream;
using NATS.Net;
using RatingService;
using System.Text.Json;
using System.Text.Json.Nodes;

Console.WriteLine($"Beginning program");

var host = Environment.GetEnvironmentVariable("NATS_SERVICE_HOST");
var port = Environment.GetEnvironmentVariable("NATS_SERVICE_PORT");
var url = $"nats://{host}:{port}";

var client = new NatsClient(url);

var jetStream = client.CreateJetStreamContext();
var consumer = await jetStream.GetConsumerAsync("mystream", "my-pull-consumer");

Console.WriteLine("Ready to process messages");

await foreach (var message in consumer.ConsumeAsync<Request<JsonNode>>())
{
    Console.WriteLine("processing message");
    var splitSubject = ExtractHttpMethod(message);

    Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");

    HandleRequest(splitSubject, message);

    await message.AckAsync();
}

void HandleRequest((string httpMethod, string pathPart) splitSubject, NatsJSMsg<Request<JsonNode>> message)
{
    Console.WriteLine($"httpMethod: {splitSubject.httpMethod} -- pathPart: {splitSubject.pathPart}");
    switch (splitSubject)
    {
        case ("get", "users"):
            _ = handleGetUsersAsync(message);
            break;
        case ("post", "users"):
            Console.WriteLine($"Received request body: {message.Data.Body}");
            var body = message.Data.Body.Deserialize<User>();
            var request = new Request<User>
            {
                Headers = message.Data.Headers,
                OriginReplyTo = message.Data.OriginReplyTo,
                Body = body
            };
            Console.WriteLine($"Post request body username: {request.Body.Username}");
            _ = handlePostUsersAsync(request);
            break;
    };

}

async Task handleGetUsersAsync<T>(NatsJSMsg<Request<T>> message)
{
    await client.PublishAsync(message.Data.OriginReplyTo, new Response<User[]>
    {
        StatusCode = 200,
        Body = [
            new User { Username = "lionel57", Id = 27 },
            new User { Username = "catmaster", Id = 3 }
        ],
        Headers = new Dictionary<string, string>()
    });
}

async Task handlePostUsersAsync(Request<User> request)
{
    await client.PublishAsync(request.OriginReplyTo, new Response<string>
    {
        StatusCode = 201,
        Body = string.Empty,
        Headers = new Dictionary<string, string>()
    });
}

static (string httpMethod, string pathPart) ExtractHttpMethod<T>(NatsJSMsg<Request<T>> message)
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