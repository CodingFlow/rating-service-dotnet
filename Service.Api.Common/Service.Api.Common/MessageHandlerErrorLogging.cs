using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NATS.Client.JetStream;
using NATS.Net;

namespace Service.Api.Common;

internal partial class MessageHandlerErrorLogging(IMessageHandler messageHandler, IMemoryCache memoryCache, ILogger<MessageHandlerErrorLogging> logger) : MessageHandlerDecorator(messageHandler)
{
    public override async ValueTask HandleMessage(NatsClient client, INatsJSMsg<Request<JsonNode>> message, CancellationToken cancellationToken)
    {
        try
        {
            LogStartedProcessingMessage();
            LogMessageHeaders(message.Headers);
            LogMessageBody(message.Data);

            await base.HandleMessage(client, message, cancellationToken);
        }
        catch (Exception ex)
        {
            LogMessageProcessingError(ex);

            message.Headers.TryGetValue("Nats-Msg-Id", out var messageId);
            memoryCache.Remove(messageId);

            await message.NakAsync(cancellationToken: cancellationToken);
        }
    }

    [LoggerMessage(LogLevel.Information, Message = "Processing message")]
    private partial void LogStartedProcessingMessage();

    [LoggerMessage(LogLevel.Debug, Message = "Request message headers: {headers}")]
    private partial void LogMessageHeaders(object headers);

    [LoggerMessage(LogLevel.Debug, Message = "Request message body: {body}")]
    private partial void LogMessageBody(object body);

    [LoggerMessage(LogLevel.Warning, Message = "Error processing message")]
    private partial void LogMessageProcessingError(Exception ex);
}
