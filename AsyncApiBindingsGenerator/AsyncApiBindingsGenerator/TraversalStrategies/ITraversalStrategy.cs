using System.Collections.Generic;
using ByteBard.AsyncAPI.Models;

namespace AsyncApiBindingsGenerator.TraversalStrategies
{
    internal interface ITraversalStrategy<TInput, TOutput>
    {
        TOutput ProcessParent(TInput input, AsyncApiDocument spec);
        TInput ProcessChild(KeyValuePair<string, AsyncApiJsonSchema> propertyEntry, TInput parentInput, AsyncApiDocument spec);
    }
}
