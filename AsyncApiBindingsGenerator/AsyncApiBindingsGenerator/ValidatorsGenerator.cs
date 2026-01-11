using System;
using System.Collections.Generic;
using System.Linq;
using AsyncApiBindingsGenerator.TraversalStrategies;
using AsyncApiBindingsGenerator.Utils;
using ByteBard.AsyncAPI.Models;
using Microsoft.CodeAnalysis;
using static AsyncApiBindingsGenerator.Utils.StringUtils;

namespace AsyncApiBindingsGenerator
{
    internal static class ValidatorsGenerator
    {
        public static IEnumerable<(string source, string className)> GenerateValidators(AsyncApiDocument spec, string assemblyName)
        {
            var @namespace = assemblyName;
            var serviceNamespacePart = @namespace.Split('.').First();

            var outputs = spec.Operations.Values
                .SelectMany(operation =>
                {
                    var parts = operation.Channel.Address.Split('.');
                    var (restMethod, pathPart) = (parts.First(), parts.ElementAt(1));

                    var requestNamespacePart = GetRequestNamespacePart(restMethod);

                    var requestTypeName = $"{ToPascalCase(restMethod)}{ToPascalCase(pathPart)}{GetRequestType(restMethod)}";
                    var className = $"{requestTypeName}Validator";

                    var requestSchema = operation.Channel.Messages.First().Value.Payload.Schema.As<AsyncApiJsonSchema>();
                    requestSchema.Properties.TryGetValue("body", out var body);

                    return ProcessingUtils.CreateTraversal(new ValidationModelsInput
                    {
                        Schema = body,
                        ServiceNamespacePart = serviceNamespacePart,
                        RequestNamespacePart = requestNamespacePart,
                        RequestTypeName = requestTypeName,
                        ClassName = className
                    }, new ValidationModelStrategy(), spec);
                });

            return outputs.Distinct();
        }

        private static string GetRequestNamespacePart(string restMethod)
        {
            switch (restMethod)
            {
                case "get":
                    return "Queries";
                case "post":
                    return "Commands";
                case "delete":
                    return "Commands";
                default:
                    throw new NotSupportedException($"Rest method {restMethod} is not supported.");
            }
        }
    }
}