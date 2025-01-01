using NATS.Client.JetStream;
using NATS.Net;
using RatingService;
using System.Text.Json;

Console.WriteLine($"Beginning program");

var host = Environment.GetEnvironmentVariable("NATS_SERVICE_HOST");
var port = Environment.GetEnvironmentVariable("NATS_SERVICE_PORT");
var url = $"nats://{host}:{port}";

var client = new NatsClient(url);

var jetStream = client.CreateJetStreamContext();
var consumer = await jetStream.GetConsumerAsync("mystream", "my-pull-consumer");

Console.WriteLine("Ready to process messages");

await foreach (var message in consumer.ConsumeAsync<Request<string>>())
{
    Console.WriteLine("processing message");
    var splitSubject = ExtractHttpMethod(message);

    Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");

    HandleRequest(splitSubject, message);

    await message.AckAsync();
}

void HandleRequest<T>((string httpMethod, string pathPart) splitSubject, NatsJSMsg<Request<T>> message)
{
    Console.WriteLine($"httpMethod: {splitSubject.httpMethod} -- pathPart: {splitSubject.pathPart}");
    switch (splitSubject)
    {
        case ("get", "users"):
            _ = handleGetUsersAsync(message);
            break;
    };

}

async Task handleGetUsersAsync<T>(NatsJSMsg<Request<T>> message)
{
    await client.PublishAsync(message.Data.OriginReplyTo, new Response<User[]>
    {
        StatusCode = 200,
        Body = [
            new User { Username = "katz", Id = 27 },
            new User { Username = "dogs are here", Id = 3 }
        ],
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