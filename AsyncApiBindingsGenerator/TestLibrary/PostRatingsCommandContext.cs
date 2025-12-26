using System.Text.Json.Serialization;
using TestProject.Application.Commands;

namespace TestProject.Api.Commands;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PostRatingsCommand))]
internal partial class PostRatingsCommandContext : JsonSerializerContext
{
}