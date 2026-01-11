using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Service.Api.Common.DistributedCacheServices;
using Service.Libraries.Redis;
using StackExchange.Redis;

namespace Service.Api.Common;

internal partial class DistributedCacheService(IRedisConnection redisConnection, ILogger<DistributedCacheService> logger) : IDistributedCacheService
{
    public async Task<bool> CheckDistributedCache(StringValues messageId, bool isInLocalCache)
    {
        if (isInLocalCache)
        {
            return true;
        }

        var value = await redisConnection.Database.StringGetAsync(messageId.ToString());

        var messageAlreadyReceived = value.HasValue;

        if (!messageAlreadyReceived)
        {
            await redisConnection.Database.StringSetAsync(
                messageId.ToString(),
                "PENDING",
                expiry: TimeSpan.FromSeconds(2),
                When.NotExists);
        }
        else
        {
            LogIgnoreDuplicateMessageDistributed(messageId);
        }

        return messageAlreadyReceived;
    }

    [LoggerMessage(LogLevel.Information, Message = "Ignored duplicate message with id {messageId} because it is in distributed cache.")]
    private partial void LogIgnoreDuplicateMessageDistributed(string messageId);
}
