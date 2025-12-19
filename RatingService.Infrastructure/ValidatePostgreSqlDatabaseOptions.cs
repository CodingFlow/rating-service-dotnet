using Microsoft.Extensions.Options;

namespace RatingService.Infrastructure;

[OptionsValidator]
internal partial class ValidatePostgreSqlDatabaseOptions : IValidateOptions<PostgreSqlDatabaseOptions>
{
}
