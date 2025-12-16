using System;
using System.Text;
using ByteBard.AsyncAPI.Models;
using ByteBard.AsyncAPI.Readers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AsyncApiApplicationSupportGenerator
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
            var outputs = OutputGenerator.GenerateSpecOutputs(info.asyncApiDocument, info.assemblyName);

            foreach (var (classSource, className) in outputs)
            {
                context.AddSource($"{className}.generated.cs", SourceText.From(classSource, Encoding.UTF8, SourceHashAlgorithm.Sha256));
            }
        }
    }
}