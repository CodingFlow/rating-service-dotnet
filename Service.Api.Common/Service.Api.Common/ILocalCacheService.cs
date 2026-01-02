using Microsoft.Extensions.Primitives;

namespace Service.Api.Common;

internal interface ILocalCacheService
{
    bool CheckLocalCache(StringValues messageId);
}