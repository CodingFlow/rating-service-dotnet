using Microsoft.Extensions.Primitives;

namespace Service.Api.Common.LocalCacheServices;

internal interface ILocalCacheService
{
    bool CheckLocalCache(StringValues messageId);
}