using RatingService.Domain;
using Vogen;

namespace RatingService.Infrastructure;

[EfCoreConverter<RatingId>]
public partial class RatingConverter
{
}
