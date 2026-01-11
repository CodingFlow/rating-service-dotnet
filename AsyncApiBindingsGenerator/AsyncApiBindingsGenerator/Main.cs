using System;
using System.Text;
using ByteBard.AsyncAPI.Models;
using ByteBard.AsyncAPI.Readers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AsyncApiBindingsGenerator
{
    [Generator]
    public class Main : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var texts = context.AdditionalTextsProvider
                .Where(text => text.Path.EndsWith("asyncapi.yaml"))
                .Select((text, cancellationToken) => text.GetText(cancellationToken));

            var apiSpecs = texts.Select((text, cancellationToken) =>
            {
                var asyncApiStringReader = new AsyncApiStringReader();
                var input = text.ToString();
                var asyncApiDocument = asyncApiStringReader.Read(input, out var diagnostic);
                return asyncApiDocument;
            });

            var assemblyNameProvider = context.CompilationProvider.Select((compilation, _ ) => compilation.Assembly.Name);

            var apiSpecsWithAssemblyNames = apiSpecs.Combine(assemblyNameProvider);

            context.RegisterSourceOutput(apiSpecsWithAssemblyNames, Execute);
        }

        private static void Execute(SourceProductionContext context, (AsyncApiDocument asyncApiDocument, string assemblyName) info)
        {
            var (dispatcherSource, dispatcherName) = OutputGenerator.GenerateDispatcher(info.asyncApiDocument, info.assemblyName);
            var (responseStrategiesSource, responseStrategiesName) = OutputGenerator.GenerateDependencyInjectionRegistrations(info.asyncApiDocument, info.assemblyName);
            var validators = ValidatorsGenerator.GenerateValidators(info.asyncApiDocument, info.assemblyName);
            var (validationExtensionsSource, validationExtensionsName) = ValidationExtensionsGenerator.Generate(info.asyncApiDocument, info.assemblyName);

            AddSource(context, dispatcherSource, dispatcherName);
            AddSource(context, responseStrategiesSource, responseStrategiesName);

            foreach (var (validatorSource, validatorClassName) in validators)
            {
                AddSource(context, validatorSource, validatorClassName);
            }

            AddSource(context, validationExtensionsSource, validationExtensionsName);
        }

        private static void AddSource(SourceProductionContext context, string source, string className)
        {
            context.AddSource($"{className}.generated.cs", SourceText.From(source, Encoding.UTF8, SourceHashAlgorithm.Sha256));
        }
    }
}
