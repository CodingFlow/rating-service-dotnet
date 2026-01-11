using ByteBard.AsyncAPI.Models;

namespace AsyncApiBindingsGenerator.TraversalStrategies
{
    internal class ValidationModelsInput : AbstractInput
    {
        public string ServiceNamespacePart { get; set; }
        public string RequestNamespacePart { get; set; }
        public string RequestTypeName { get; set; }
        public string ClassName { get; set; }
    }
}
