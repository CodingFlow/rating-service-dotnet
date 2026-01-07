using System.Diagnostics;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NATS.Client.Core;
using NATS.Client.JetStream;
using OpenTelemetry.Context.Propagation;
using Service.Abstractions;

namespace Service.Api.Common;

internal partial class Main(
    INatsConnectionService natsConnectionService,
    ILocalCacheService localCacheService,
    IDistributedCacheService distributedCacheService,
    IServiceScopeFactory serviceScopeFactory,
    IEnumerable<IStartupService> startupServices,
    ILogger<Main> logger) : IHostedService
{
    private static readonly ActivitySource tracer = new(nameof(Main));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var startupTasks = startupServices.Select(service => service.Startup());

        var consumer = await natsConnectionService.ConnectToNats(cancellationToken);

        await Task.WhenAll(startupTasks);

        LogReadyMessages();

        var messages = consumer.ConsumeAsync<Request<JsonNode>>(cancellationToken: cancellationToken);

        await Parallel.ForEachAsync(messages, async (message, cancellationToken) =>
        {
            try
            {
                message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);

                using var loggerScope = logger.BeginScope(CreateState(messageId));
                using var tracerScope = tracer.StartActivity("Request", ActivityKind.Server, GetParentScope(message, messageId));

                var isInLocalCache = localCacheService.CheckLocalCache(messageId);
                var isInDistributedCache = await distributedCacheService.CheckDistributedCache(messageId, isInLocalCache);

                if (!isInLocalCache && !isInDistributedCache)
                {
                    await BeginRequestScope(message, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LogBeginProcessingMessageError(ex);
            }
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await natsConnectionService.DisposeAsync();
    }

    private static Dictionary<string, object> CreateState(StringValues messageId)
    {
        return new Dictionary<string, object>() {
            { "NatsMsgId", messageId }
        };
    }

    private static ActivityContext GetParentScope(INatsJSMsg<Request<JsonNode>> message, StringValues messageId)
    {
        return Propagators.DefaultTextMapPropagator.Extract(default, message.Headers, GetCorrelationId).ActivityContext;
    }

    private static IEnumerable<string> GetCorrelationId(NatsHeaders headers, string name)
    {
        headers.TryGetValue(name, out var values);
        return values.Where(value => value != null).ToList()! ?? Enumerable.Empty<string>();
    }

    private async Task BeginRequestScope(INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var messageHandler = scope.ServiceProvider.GetService<IMessageHandler>();

        await messageHandler.HandleMessage(message, cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, Message = "Ready to process messages")]
    private partial void LogReadyMessages();

    [LoggerMessage(LogLevel.Warning, Message = "Failed to begin processing message.")]
    private partial void LogBeginProcessingMessageError(Exception ex);
}
