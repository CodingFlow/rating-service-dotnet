using System.Text.Json.Serialization;
using RatingService.Application.Commands;

namespace RatingService.Api.Commands;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PostRatingsCommand))]
internal partial class PostRatingsCommandContext : JsonSerializerContext
{
}
