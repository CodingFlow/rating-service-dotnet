using System.Text.Json.Serialization;

namespace TestProject;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(User[]))]
internal partial class SourceGenerationContextPostUsersHandler : JsonSerializerContext { }
