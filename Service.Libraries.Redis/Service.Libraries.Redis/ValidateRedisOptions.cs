using Microsoft.Extensions.Options;

namespace Service.Libraries.Redis;

[OptionsValidator]
internal partial class ValidateRedisOptions : IValidateOptions<RedisOptions>
{
}
