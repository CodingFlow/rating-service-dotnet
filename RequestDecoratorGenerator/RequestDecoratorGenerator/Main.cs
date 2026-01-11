using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using ByteBard.AsyncAPI.Models;
using ByteBard.AsyncAPI.Readers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace RequestDecoratorGenerator
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

            var types = context.SyntaxProvider.ForAttributeWithMetadataName(
                $"RequestDecoratorGenerator.{nameof(RequestDecoratorAttribute)}",
                predicate: IsSyntaxTargetForGeneration,
                transform: GetSemanticTargetForGeneration
            );

            var allTypes = types.Collect();

            var assemblyNameProvider = context.CompilationProvider.Select((compilation, _) => compilation.Assembly.Name);

            var final = allTypes
                .Combine(
                    apiSpecs.Collect()
                        .Combine(assemblyNameProvider))
                .Select((combined, _) =>
                {
                    var (namedTypes, nested) = combined;
                    var (specs, assemblyName) = nested;

                    return (Types: namedTypes, Specs: specs, AssemblyName: assemblyName);
                }); ;

            context.RegisterSourceOutput(final, Execute);
        }

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode, CancellationToken token)
        {
            return syntaxNode is ClassDeclarationSyntax;
        }

        private static INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            return context.TargetSymbol as INamedTypeSymbol;
        }

        private static void Execute(SourceProductionContext context, (ImmutableArray<INamedTypeSymbol> typeSymbols, ImmutableArray<AsyncApiDocument> asyncApis, string assemblyName) tuple)
        {
            var (typeSymbols, asyncApis, assemblyName) = tuple;

            var asyncApi = asyncApis.First();
            var (source, className) = OutputGenerator.GenerateOutput(typeSymbols, asyncApi, assemblyName);

            context.AddSource($"{className}.generated.cs", SourceText.From(source, Encoding.UTF8, SourceHashAlgorithm.Sha256));
        }
    }
}
