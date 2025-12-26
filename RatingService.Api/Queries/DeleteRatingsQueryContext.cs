using System.Text.Json.Serialization;
using RatingService.Application.Queries;

namespace RatingService.Api.Queries;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DeleteRatingsQuery))]
internal partial class DeleteRatingsQueryContext : JsonSerializerContext
{
}
