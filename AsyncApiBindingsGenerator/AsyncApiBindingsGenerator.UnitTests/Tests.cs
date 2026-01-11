using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using TestProject;
using TestProject.Api.Validators;
using VerifyCS = AsyncApiBindingsGenerator.UnitTests.CSharpSourceGeneratorVerifier<AsyncApiBindingsGenerator.Main>;

namespace AsyncApiBindingsGenerator.UnitTests;

public class Tests
{
    private Assembly implementationAssembly;

    [SetUp]
    public void Setup()
    {
        implementationAssembly = GetAssembly("AsyncApiBindingsGenerator");
    }

    [Test]
    public async Task SimpleGetPost()
    {
        var generatedDispatcher = await ReadCSharpFile<RequestDispatcher>(true);
        
        var generatedResponseStrategies = await ReadCSharpFileByName(true, "ResponseStrategiesExtensions");
        
        var generatedGetRatingsQueryValidator = await ReadCSharpFile<GetRatingsQueryValidator>(true);
        var generatedPostRatingsCommandValidator = await ReadCSharpFile<PostRatingsCommandValidator>(true);
        var generatedRatingValidator = await ReadCSharpFile<RatingValidator>(true);
        var generatedDeleteRatingsQueryValidator = await ReadCSharpFile<DeleteRatingsQueryValidator>(true);

        var generatedValidationExtensions = await ReadCSharpFileByName(true, "ValidationExtensions");

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
                    (typeof(Main), "RequestDispatcher.generated.cs", SourceText.From(generatedDispatcher, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "ResponseStrategiesExtensions.generated.cs", SourceText.From(generatedResponseStrategies, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "GetRatingsQueryValidator.generated.cs", SourceText.From(generatedGetRatingsQueryValidator, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "PostRatingsCommandValidator.generated.cs", SourceText.From(generatedPostRatingsCommandValidator, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "RatingValidator.generated.cs", SourceText.From(generatedRatingValidator, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "DeleteRatingsQueryValidator.generated.cs", SourceText.From(generatedDeleteRatingsQueryValidator, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "ValidationExtensions.generated.cs", SourceText.From(generatedValidationExtensions, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
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