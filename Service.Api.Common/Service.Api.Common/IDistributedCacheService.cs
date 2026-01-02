using Microsoft.Extensions.Primitives;

namespace Service.Api.Common;

internal interface IDistributedCacheService
{
    Task<bool> CheckDistributedCache(StringValues messageId, bool isInLocalCache);
}