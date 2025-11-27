// See https://aka.ms/new-console-template for more information


using csly.generator.sourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpFileSystem.FileSystems;
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
        var parser = fs.ReadAllText("/samples/toky.gram");

        var result = GenerateSource(parser, "SimpleParser");

        var contents = result.GeneratedTrees.ToList().ToDictionary(x => x.FilePath, x => x.ToString());
        var generatedFiles = result.GeneratedTrees.Select(x => new FileInfo(x.FilePath).Name);

        string path = "c:/tmp/generation/";

        File.WriteAllText(Path.Combine(path, "Toky.cs"), parser);

        foreach (var file in contents)
        {
            FileInfo fileInfo = new FileInfo(file.Key);

            string fileName = Path.Combine(path, fileInfo.Name);

            if (fileInfo.Name.StartsWith("lexer."))
            {
                fileName = Path.Combine(path, "models", "lexer" + fileInfo.Name);
            }
            if (fileInfo.Name.StartsWith("parser."))
            {
                fileName = Path.Combine(path, "models", "parser" + fileInfo.Name);
            }

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            FileInfo fi = new FileInfo(fileName);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            File.WriteAllText(fileName, file.Value);
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



    //private static void Run()
    //{
    //var parser = new expressionParser.ExpressionParser();
    //var main = new sourceGenerationTester.expressionParser.ExpressionParserMain();
    //var entryPoint = new expressionParser.ExpressionParserMain(parser);



    //while (true)
    //{
    //    var choice = Console.ReadLine();
    //    if (string.IsNullOrEmpty(choice) || choice == "q" || choice == "quit")
    //    {
    //        Environment.Exit(0);
    //    }
    //    var r = entryPoint.Parse(choice);
    //    if (r.IsOk)
    //    {
    //        Console.WriteLine($"{choice} = {r.Result}");
    //        Console.WriteLine(r.SyntaxTree.Dump("  "));
    //    }
    //    else
    //    {
    //        Console.WriteLine("Parse failed");
    //        foreach (var err in r.Errors)
    //        {
    //            Console.WriteLine(err.ErrorMessage);
    //        }
    //    }
    //}
}
