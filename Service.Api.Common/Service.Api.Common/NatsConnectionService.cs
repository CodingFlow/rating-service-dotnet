using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal partial class NatsConnectionService(
    IOptions<NatsServiceOptions> natsServiceOptions,
    IOptions<ServiceStreamConsumerOptions> serviceStreamConsumerOptions,
    ILogger<NatsConnectionService> logger) : INatsConnectionService, IAsyncDisposable
{
    public NatsClient? Client { get; private set; }

    private readonly NatsServiceOptions natsServiceSettings = natsServiceOptions.Value;
    private readonly ServiceStreamConsumerOptions serviceStreamConsumerSettings = serviceStreamConsumerOptions.Value;

    public async Task<INatsJSConsumer> ConnectToNats(CancellationToken cancellationToken)
    {
        var host = natsServiceSettings.ServiceHost;
        var port = natsServiceSettings.Port;

        LogConnectedToNats(host, port);

        var url = $"nats://{host}:{port}";

        Client = new NatsClient(url);

        var jetStream = Client.CreateJetStreamContext();
        var consumer = await jetStream.GetConsumerAsync(serviceStreamConsumerSettings.StreamName, serviceStreamConsumerSettings.ConsumerName, cancellationToken);

        return consumer;
    }

    public async ValueTask DisposeAsync()
    {
        await Client!.DisposeAsync();
    }

    [LoggerMessage(LogLevel.Information, Message = "Connecting to NATS at {host}:{port}")]
    private partial void LogConnectedToNats(string host, string port);
}
