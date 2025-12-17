using System.Reflection;
using System.Text;
using AsyncApiApplicationSupportGenerator;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using TestProject.Commands;
using TestProject.Handlers;
using TestProject.Models;
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
        var generatedInterfaceGetUsers = await ReadCSharpFile<IGetUsersHandler>(true);
        var generatedInterfacePostUsers = await ReadCSharpFile<IPostUsersHandler>(true);
        var generatedModelGetUsersQueryResponse = await ReadCSharpFile<GetUsersQueryResponse>(true);
        var generatedModelPostUsersCommand = await ReadCSharpFile<PostUsersCommand>(true);
        var generatedModelUser = await ReadCSharpFile<User>(true);

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
                    (typeof(Main), "IGetUsersHandler.generated.cs", SourceText.From(generatedInterfaceGetUsers, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "IPostUsersHandler.generated.cs", SourceText.From(generatedInterfacePostUsers, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "GetUsersQueryResponse.generated.cs", SourceText.From(generatedModelGetUsersQueryResponse, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "PostUsersCommand.generated.cs", SourceText.From(generatedModelPostUsersCommand, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    (typeof(Main), "User.generated.cs", SourceText.From(generatedModelUser, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
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