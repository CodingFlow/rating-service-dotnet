using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Service.Api.Common.LocalCacheServices;

namespace Service.Api.Common;

internal partial class LocalCacheService(IMemoryCache memoryCache, ILogger<LocalCacheService> logger) : ILocalCacheService
{
    public bool CheckLocalCache(StringValues messageId)
    {
        var messageAlreadyReceived = memoryCache.TryGetValue(messageId, out _);

        if (!messageAlreadyReceived)
        {
            var memoryCacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(1)
            };

            memoryCache.Set(messageId, true, memoryCacheOptions);
        }
        else
        {
            LogIgnoreDuplicateMessageLocal(messageId);
        }

        return messageAlreadyReceived;
    }

    [LoggerMessage(LogLevel.Information, Message = "Ignored duplicate message with id {messageId} because it is in local cache.")]
    private partial void LogIgnoreDuplicateMessageLocal(string messageId);
}
