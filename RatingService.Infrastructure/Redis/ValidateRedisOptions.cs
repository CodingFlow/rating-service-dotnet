using Microsoft.Extensions.Options;

namespace RatingService.Infrastructure.Redis;

[OptionsValidator]
internal partial class ValidateRedisOptions : IValidateOptions<RedisOptions>
{
}
