using System.Text.Json.Serialization;

namespace AsyncApiBindingsGenerator
{
    internal class Body
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}