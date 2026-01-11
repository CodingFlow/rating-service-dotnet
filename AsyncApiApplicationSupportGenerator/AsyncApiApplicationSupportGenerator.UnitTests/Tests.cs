using System.Reflection;
using System.Text;
using AsyncApiApplicationSupportGenerator;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using TestProject.Commands;
using TestProject.Handlers;
using TestProject.Models;
using TestProject.Queries;
using TestProject.QueryResponses;
using VerifyCS = AsyncApiBindingsGenerator.UnitTests.CSharpSourceGeneratorVerifier<AsyncApiApplicationSupportGenerator.Main>;

namespace AsyncApiBindingsGenerator.UnitTests;

public class Tests
{
    private Assembly implementationAssembly;

    [SetUp]
    public void Setup()
    {
        implementationAssembly = GetAssembly("AsyncApiApplicationSupportGenerator");
    }

    [Test]
    public async Task SimpleGetPost()
    {
        var generatedInterfaceGetRatings = await ReadCSharpFile<IGetRatingsHandler>(true);
        var generatedInterfacePostRatings = await ReadCSharpFile<IPostRatingsHandler>(true);
        var generatedInterfaceDeleteRatings = await ReadCSharpFile<IDeleteRatingsHandler>(true);

        var generatedModelGetRatingsQuery = await ReadCSharpFile<GetRatingsQuery>(true);
        var generatedModelGetRatingsQueryResponse = await ReadCSharpFile<GetRatingsQueryResponse>(true);
        
        var generatedModelPostRatingsCommand = await ReadCSharpFile<PostRatingsCommand>(true);
        
        var generatedModelDeleteRatingsQuery = await ReadCSharpFile<DeleteRatingsQuery>(true);

        var generatedModelRating = await ReadCSharpFile<Rating>(true);

        const string asyncApiFilename = "asyncapi.yaml";
        var asyncapiYaml = await ReadFile(true, asyncApiFilename);

        await new VerifyCS.Test
        {
            CompilerDiagnostics = CompilerDiagnostics.None,
            TestState = {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
                AdditionalReferences =
                {
                    implementationAssembly,
                    GetAssembly("TestLibrary")
                },
                AdditionalFiles =
                {
                    (asyncApiFilename, asyncapiYaml)
                },
                

                Sources = {  },
                GeneratedSources =
                {
                    (typeof(Main), "IGetRatingsHandler.generated.cs", SourceText.From(generatedInterfaceGetRatings, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "IPostRatingsHandler.generated.cs", SourceText.From(generatedInterfacePostRatings, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "IDeleteRatingsHandler.generated.cs", SourceText.From(generatedInterfaceDeleteRatings, Encoding.UTF8, SourceHashAlgorithm.Sha256)),

                    (typeof(Main), "GetRatingsQuery.generated.cs", SourceText.From(generatedModelGetRatingsQuery, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "GetRatingsQueryResponse.generated.cs", SourceText.From(generatedModelGetRatingsQueryResponse, Encoding.UTF8, SourceHashAlgorithm.Sha256)),

                    (typeof(Main), "Rating.generated.cs", SourceText.From(generatedModelRating, Encoding.UTF8, SourceHashAlgorithm.Sha256)),

                    (typeof(Main), "PostRatingsCommand.generated.cs", SourceText.From(generatedModelPostRatingsCommand, Encoding.UTF8, SourceHashAlgorithm.Sha256)),

                    (typeof(Main), "DeleteRatingsQuery.generated.cs", SourceText.From(generatedModelDeleteRatingsQuery, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                },
            },
        }.RunAsync();
    }

    private static Assembly GetAssembly(string name)
    {
        var implementationAssemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().First(a => a.FullName.Contains(name));
        return Assembly.Load(implementationAssemblyName);
    }

    private static async Task<string> ReadCSharpFile<T>(bool isTestLibrary)
    {
        var filenameWithoutExtension = typeof(T).Name;
        return await ReadCSharpFileByName(isTestLibrary, filenameWithoutExtension);
    }

    private static async Task<string> ReadCSharpFileByName(bool isTestLibrary, string filenameWithoutExtension)
    {
        var searchPattern = $"{filenameWithoutExtension}*.cs";
        return await ReadFile(isTestLibrary, searchPattern);
    }

    private static async Task<string> ReadFile(bool isTestLibrary, string searchPattern)
    {
        var currentDirectory = GetCurrentDirectory();

        var targetDirectory = isTestLibrary ? GetTestLibraryDirectory(currentDirectory) : currentDirectory;

        var file = targetDirectory.GetFiles(searchPattern).First();

        using var fileReader = new StreamReader(file.OpenRead());
        return await fileReader.ReadToEndAsync();
    }

    private static DirectoryInfo? GetCurrentDirectory()
    {
        return Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName);
    }

    private static DirectoryInfo GetTestLibraryDirectory(DirectoryInfo currentDirectory)
    {
        return currentDirectory.Parent.GetDirectories("TestLibrary").First();
    }
}