// See https://aka.ms/new-console-template for more information

using csly.generator.sourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpFileSystem.FileSystems;
using sourceGenerationTester.expressionParser;
using System;
using System.IO;
using System.Linq;

namespace sourceGenerationTester;
public partial class Program
{
    public static void Main(string[] args)
    {
        Generate();
        //Run();
        /*GoStatic();
        Run();*/
    }


    private static void Generate()
    {
        EmbeddedResourceFileSystem fs = new EmbeddedResourceFileSystem(typeof(Program).Assembly);
        var parser = fs.ReadAllText("/samples/expression.gram");
        
        var result = GenerateSource(parser, "SimpleParser");
        
        var contents = result.GeneratedTrees.ToList().ToDictionary(x => x.FilePath,x => x.ToString());
        var generatedFiles = result.GeneratedTrees.Select(x => new FileInfo(x.FilePath).Name);

        foreach (var file in contents)
        {
            File.WriteAllText(Path.Combine("c:/tmp/generation/","SimpleParser.cs"), file.Value);
        }
        
    }
    
    private static GeneratorDriverRunResult GenerateSource(string source, string className)
    {
        // Create an instance of the source generator.
        var generator = new CslyParserGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(new[] { generator });

        // To run generators, we can use an empty compilation.
        var compilation = CSharpCompilation.Create(className,
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        // Run generators. Don't forget to use the new compilation rather than the previous one.
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        return runResult;
    }



    private static void Run()
    {
        while (true)
        {
            var choice = Console.ReadLine();
            if (string.IsNullOrEmpty(choice) || choice == "q" || choice == "quit")
            {
                Environment.Exit(0);
            }
            ExpressionGenerator parser = new ExpressionGenerator();
            


        }
    }
}