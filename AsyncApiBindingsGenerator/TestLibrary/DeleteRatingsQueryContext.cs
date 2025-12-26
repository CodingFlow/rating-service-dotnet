using System.Text.Json.Serialization;
using TestProject.Application.Queries;

namespace TestProject.Api.Queries;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DeleteRatingsQuery))]
internal partial class DeleteRatingsQueryContext : JsonSerializerContext
{
}