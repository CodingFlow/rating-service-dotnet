using System.Text.Json.Serialization;
using RatingService.Application;

namespace RatingService.Api;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(User[]))]
internal partial class SourceGenerationContextPostUsersHandler : JsonSerializerContext
{
}
