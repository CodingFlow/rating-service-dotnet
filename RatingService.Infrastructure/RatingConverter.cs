using RatingService.Domain;
using Vogen;

namespace RatingService.Infrastructure;

[EfCoreConverter<RatingId>]
[EfCoreConverter<UserId>]
[EfCoreConverter<ServiceId>]
[EfCoreConverter<Score>]
public partial class RatingConverter
{
}

