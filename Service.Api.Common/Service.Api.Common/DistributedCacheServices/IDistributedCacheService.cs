using Microsoft.Extensions.Primitives;

namespace Service.Api.Common.DistributedCacheServices;

internal interface IDistributedCacheService
{
    Task<bool> CheckDistributedCache(StringValues messageId, bool isInLocalCache);
}