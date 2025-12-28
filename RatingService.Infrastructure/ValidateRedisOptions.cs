using Microsoft.Extensions.Options;

namespace RatingService.Infrastructure;

[OptionsValidator]
internal partial class ValidateRedisOptions : IValidateOptions<RedisOptions>
{
}
