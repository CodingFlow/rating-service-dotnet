using System;
using System.Collections.Generic;
using System.Linq;
using AsyncApiBindingsGenerator.TraversalStrategies;
using ByteBard.AsyncAPI.Models;
using Microsoft.CodeAnalysis;

namespace AsyncApiBindingsGenerator.Utils
{
    internal class ProcessingUtils
    {
        public static IEnumerable<TOutput> CreateTraversal<TInput, TOutput>(TInput input, ITraversalStrategy<TInput, TOutput> strategy, AsyncApiDocument spec)
            where TInput : AbstractInput
        {
            var output = strategy.ProcessParent(input, spec);

            var childrenValidations = input.Schema.Properties
                .Where(propertyEntry => propertyEntry.Value.Type == SchemaType.Object
                    || (propertyEntry.Value.Type == SchemaType.Array && propertyEntry.Value.Items.Type == SchemaType.Object))
                .SelectMany(propertyEntry =>
            {
                var childOutput = strategy.ProcessChild(propertyEntry, input, spec);

                return CreateTraversal(childOutput, strategy, spec);
            });

            return new List<TOutput> { output }.Concat(childrenValidations);
        }

        public static bool IsPayloadPropertyExists(AsyncApiMessage messageReference, string propertyName)
        {
            var entry = GetPayloadSchemaEntry(messageReference, propertyName);

            return !entry.Equals(default(KeyValuePair<string, AsyncApiJsonSchema>));
        }
        
        public static KeyValuePair<string, AsyncApiJsonSchema> GetPayloadSchemaEntry(AsyncApiMessage messageReference, string propertyName)
        {
            return messageReference
                .Payload.Schema.As<AsyncApiJsonSchema>()
                .Properties.FirstOrDefault(prop => prop.Key == propertyName);
        }
    }
}
