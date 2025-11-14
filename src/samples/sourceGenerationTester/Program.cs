// See https://aka.ms/new-console-template for more information


//using csly.generator.sourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpFileSystem.FileSystems;
//using sourceGenerationTester.expressionParser;
//using sourceGenerationTester.visitor;
using System;
//using csly.models;
using System.IO;
using System.Linq;
using csly.generator.sourceGenerator;


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

        var contents = result.GeneratedTrees.ToList().ToDictionary(x => x.FilePath, x => x.ToString());
        var generatedFiles = result.GeneratedTrees.Select(x => new FileInfo(x.FilePath).Name);

        foreach (var file in contents)
        {
            File.WriteAllText(Path.Combine("c:/tmp/generation/", "SimpleParser.cs"), file.Value);
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
    //    var parser = new StaticExpressionParser();
        
    //    while (true)
    //    {
    //        var choice = Console.ReadLine();
    //        if (string.IsNullOrEmpty(choice) || choice == "q" || choice == "quit")
    //        {
    //            Environment.Exit(0);
    //        }
    //        StaticExpressionToken scanner = new StaticExpressionToken();
    //        var lexerResult = scanner.Scan(choice.AsSpan());
                        
    //        if (lexerResult.IsError)
    //        {
    //            Console.WriteLine($"Lexing failed: {lexerResult.Error}");
    //            return;
    //        }

    //        var result = parser.ParseNonTerminal_expression(lexerResult.Tokens, 0);
    //        if (result.IsOk)
    //        {
    //            Console.WriteLine("Parse succeeded");
    //            Console.WriteLine(result.Root.Dump("  "));
    //            StaticVisitor visitor = new StaticVisitor(new ExpressionParser());
    //            if (result.Root is SyntaxNode<ExpressionToken, int> root)
    //            {
    //                var value = visitor.VisitExpression(root);
    //                Console.WriteLine($"{choice} = {value}");

    //            }
                
    //        }
    //        else
    //        {
    //            Console.WriteLine("Parse failed");
    //            foreach (var err in result.Errors)
    //            {
    //                Console.WriteLine(err.ErrorMessage);
    //            }
    //        }
    //    }
    //}
}