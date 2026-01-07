using System;
using System.Collections.Generic;
using System.Linq;
using ByteBard.AsyncAPI.Models;
using Microsoft.CodeAnalysis;

namespace AsyncApiBindingsGenerator
{
    internal static class OutputGenerator
    {
        public static (string source, string className) GenerateSpecOutputs(AsyncApiDocument spec, string assemblyName)
        {
            var @namespace = assemblyName;
            var serviceNamespacePart = @namespace.Split('.').First();
            var channels = spec.Channels;
            var addresses = channels.Select(channel => channel.Value.Address);
            var splitAddresses = addresses.Select(address =>
            {
                var parts = address.Split('.');
                return (parts.First(), parts.ElementAt(1));
            });

            var firstDependency = new List<string>() { "IRestHandler restHandler", "IQueryParameterParser queryParameterParser" };
            var formattedDependencies = firstDependency.Concat(splitAddresses.Select(((string restMethod, string pathPart) addressInfo) =>
            {
                var interfaceName = $"I{ToPascalCase(addressInfo.restMethod)}{ToPascalCase(addressInfo.pathPart)}Handler";
                var variableName = $"{addressInfo.restMethod}{ToPascalCase(addressInfo.pathPart)}Handler";
                
                return $"{interfaceName} {variableName}";
            }));

            var formattedCases = channels.Select(channel =>
            {
                var address = channel.Value.Address;
                var addressParts = address.Split('.');
                var restMethod = addressParts.First();
                var pathPart = addressParts.ElementAt(1);

                var pathPartsFormatted = string.Join(", ", addressParts.Select(p => $@"""{p}"""));

                var variableName = $"{restMethod}{ToPascalCase(pathPart)}Handler";
                var methodName = GetCaseMethodName(restMethod);
                var mergeMethodName = $"merge{ToPascalCase(restMethod)}{ToPascalCase(pathPart)}";
                var messageReference = channel.Value.Messages.First().Value;
                var mergeMethodNameWithComma = IsPayloadPropertyExists(messageReference, "body")
                    ? $"{mergeMethodName}, "
                    : string.Empty;

                return $@"            case [{pathPartsFormatted}]:
                await restHandler.{methodName}(requestData, pathParts, {variableName}, {mergeMethodNameWithComma}cancellationToken);
                break;";
            });

            var formattedMergeMethods = channels.Select(channel =>
            {
                var messageReference = channel.Value.Messages.First().Value;

                string result;

                if (IsPayloadPropertyExists(messageReference, "body"))
                {
                    var address = channel.Value.Address;
                    var parts = address.Split('.');
                    var restMethod = parts.First();
                    var pathPart = parts.ElementAt(1);

                    var requestType = GetRequestType(restMethod);
                    var mergeTypeNamespace = GetMergeTypeNamespace(restMethod, serviceNamespacePart);
                    var mergeType = $"{mergeTypeNamespace}.{ToPascalCase(restMethod)}{ToPascalCase(pathPart)}{requestType}";
                    var mergeMethodName = $"merge{ToPascalCase(restMethod)}{ToPascalCase(pathPart)}";

                    string formattedParsing;

                    if (IsPayloadPropertyExists(messageReference, "queryParameters"))
                    {
                        var parsing = GetPayloadSchemaEntry(messageReference, "queryParameters").Value.Properties.Select(entry =>
                        {
                            var requestPropertyKey = ToPascalCase(entry.Key);
                            var queryParameterKey = entry.Key;

                            var matchingBodyProperty = GetPayloadSchemaEntry(messageReference, "body").Value.Properties[queryParameterKey];

                            var propertyToParse = matchingBodyProperty.Type == SchemaType.Array
                                ? matchingBodyProperty.Items
                                : matchingBodyProperty;

                            var parseMethod = GetParseMethod(propertyToParse);

                            return $@"        var ({queryParameterKey}, {queryParameterKey}Errors) = queryParameterParser.{parseMethod}(original.{requestPropertyKey}, queryParameters, ""{queryParameterKey}"");";
                        });

                        formattedParsing = $@"

{string.Join(@"
", parsing)}";
                    } else
                    {
                        var parsing = Enumerable.Empty<string>();
                        formattedParsing = string.Empty;
                    }

                    string mergeBlock;

                    if (IsPayloadPropertyExists(messageReference, "queryParameters"))
                    {
                        var anyErrors = GetPayloadSchemaEntry(messageReference, "queryParameters").Value.Properties.Select(entry =>
                        {
                            var requestPropertyKey = ToPascalCase(entry.Key);
                            var queryParameterKey = entry.Key;

                            return $@"!{queryParameterKey}Errors.Any()";
                        });

                        var withBlockAssignments = CreateWithBlockAssignments(messageReference);
                        var errorsConcat = GetPayloadSchemaEntry(messageReference, "queryParameters").Value.Properties.Select(entry =>
                        {
                            var queryParameterKey = entry.Key;

                            return $@"                .Concat({queryParameterKey}Errors)";
                        });
                        var errorsConcatFormatted = string.Join(@"
", errorsConcat);

                        mergeBlock = $@"

        if ({string.Join(" && ", anyErrors)})
        {{
            merged = original with
            {{
{withBlockAssignments}
            }};
        }}
        else
        {{
            errors = errors
{errorsConcatFormatted};
        }}";
                    } else
                    {
                        mergeBlock = string.Empty;
                    }

                        result = $@"    private ({mergeType}, IEnumerable<ValidationError>) {mergeMethodName}({mergeType} original, Dictionary<string, string> queryParameters, string[] pathParts)
    {{
        var errors = Enumerable.Empty<ValidationError>();
        var merged = original;{formattedParsing}{mergeBlock}

        return (merged, errors);
    }}";
                }
                else
                {
                    result = string.Empty;
                }

                return result;
            })
                .Where(m => m != string.Empty);

            var source =
$@"// <auto-generated/>
#nullable restore

using System.Text.Json.Nodes;
using Service.Api.Common;
using {serviceNamespacePart}.Application.Handlers;

namespace {@namespace};

public class RequestDispatcher({string.Join(", ", formattedDependencies)}) : IRequestDispatcher
{{
    public async Task DispatchRequest(string[] pathParts, Request<JsonNode> requestData, CancellationToken cancellationToken)
    {{
        switch (pathParts)
        {{
{string.Join(@"
", formattedCases)}
        }}
    }}

{string.Join(@"

", formattedMergeMethods)}
}}
";

            return (source, "RequestDispatcher");
        }

        private static string CreateWithBlockAssignments(AsyncApiMessage messageReference)
        {
            var formattedWithAssignments = GetPayloadSchemaEntry(messageReference, "queryParameters").Value.Properties.Select(entry =>
            {
                var requestPropertyKey = ToPascalCase(entry.Key);
                var queryParameterKey = entry.Key;

                return $@"                {requestPropertyKey} = {queryParameterKey},";
            });

            return string.Join(@"
", formattedWithAssignments);
        }

        private static string GetParseMethod(AsyncApiJsonSchema schema)
        {
            switch (schema.Type)
            {
                case SchemaType.Integer:
                    return "ParseInt";
                case SchemaType.String:
                    return schema.Format == "uuid"
                        ? "ParseGuid"
                        : "ParseString";
                default:
                    return string.Empty;
            }
        }

        private static bool IsPayloadPropertyExists(AsyncApiMessage messageReference, string propertyName)
        {
            var entry = GetPayloadSchemaEntry(messageReference, propertyName);

            return !entry.Equals(default(KeyValuePair<string, AsyncApiJsonSchema>));
        }

        private static KeyValuePair<string, AsyncApiJsonSchema> GetPayloadSchemaEntry(AsyncApiMessage messageReference, string propertyName)
        {
            return messageReference
                .Payload.Schema.As<AsyncApiJsonSchema>()
                .Properties.FirstOrDefault(prop => prop.Key == propertyName);
        }

        private static string GetRequestType(string restMethod)
        {
            switch (restMethod)
            {
                case "get":
                    return "Query";
                case "delete":
                    return "Query";
                case "post":
                    return "Command";
                default:
                    throw new ArgumentException($"Rest method '{restMethod}' not supported.");
            }
        }

        private static string GetCaseMethodName(string restMethod)
        {
            switch (restMethod.ToLower())
            {
                case "get":
                    return "HandleGet";
                case "post":
                    return "HandlePost";
                case "put":
                    return "HandlePut";
                case "delete":
                    return "HandleDelete";
                case "patch":
                    return "HandlePatch";
                default:
                    throw new ArgumentException($"Unsupported REST method: {restMethod}");
            }
        }

        private static string GetMergeTypeNamespace(string restMethod, string assemblyName)
        {
            switch (restMethod)
            {
                case "get":
                    return $"{assemblyName}.Application.Queries";
                case "delete":
                    return $"{assemblyName}.Application.Queries";
                case "post":
                    return $"{assemblyName}.Application.Commands";
                default:
                    throw new ArgumentException($"Rest method '{restMethod}' not supported.");
            }
        }

        private static string ToPascalCase(string input)
        {
            return $"{char.ToUpper(input[0])}{input.Substring(1)}";
        }
    }
}