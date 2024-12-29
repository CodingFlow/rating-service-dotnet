using NATS.Net;
using RatingService;
using System.Text.Json;

Console.WriteLine($"cats ~~~~ ~~ @@");

var host = Environment.GetEnvironmentVariable("NATS_SERVICE_HOST");
var port = Environment.GetEnvironmentVariable("NATS_SERVICE_PORT");
var url = $"nats://{host}:{port}";

var client = new NatsClient(url);

var jetStream = client.CreateJetStreamContext();
var consumer = await jetStream.GetConsumerAsync("mystream", "my-pull-consumer");

await foreach (var message in consumer.ConsumeAsync<RatingRequest<RequestBody>>())
{
    Console.WriteLine($"Processed: {message.Data}");

    Console.WriteLine($"ReplyTo field: {message.ReplyTo}");

    Console.WriteLine($"Data: {JsonSerializer.Serialize(message.Data)}");


    _ = client.PublishAsync(message.Data.OriginReplyTo, new RatingResponse
    {
        StatusCode = 200,
        Body = new Rating { UserName = message.Data.Body.Animal },
        Headers = new Dictionary<string, string>()
    });

    await message.AckAsync();
}
