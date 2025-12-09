using System;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using ByteBard.AsyncAPI.Models;
using ByteBard.AsyncAPI.Readers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

            var mainHandler = context.SyntaxProvider.ForAttributeWithMetadataName(
                $"AsyncApiBindingsGenerator.{nameof(AsyncApiBindingsMainAttribute)}",
                predicate: IsSyntaxTargetForGeneration,
                transform: GetSemanticTargetForGeneration
            )
                .Collect();

            var apiSpecsWithMainHandler = apiSpecs.Combine(mainHandler);

            context.RegisterSourceOutput(apiSpecsWithMainHandler, Execute);
        }

        private bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        private INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            return context.TargetSymbol as INamedTypeSymbol;
        }

        private static void Execute(SourceProductionContext context, (AsyncApiDocument asyncApiDocument, ImmutableArray<INamedTypeSymbol> symbols) info)
        {
            var symbol = info.symbols[0];
            var (classSource, className) = OutputGenerator.GenerateSpecOutputs(info.asyncApiDocument, symbol);

            context.AddSource($"{className}.generated.cs", SourceText.From(classSource, Encoding.UTF8, SourceHashAlgorithm.Sha256));
        }
    }
}