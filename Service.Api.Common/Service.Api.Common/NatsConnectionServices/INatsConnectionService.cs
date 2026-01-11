using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common.NatsConnectionServices;

internal interface INatsConnectionService
{
    NatsClient? Client { get; }

    Task<INatsJSConsumer> ConnectToNats(CancellationToken cancellationToken);
    ValueTask DisposeAsync();
}