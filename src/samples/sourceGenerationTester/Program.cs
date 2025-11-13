// See https://aka.ms/new-console-template for more information

using csly.generator.model.lexer;
using csly.generator.sourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpFileSystem.FileSystems;
using sourceGenerationTester.expressionParser;
using sourceGenerationTester.staticlexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sourceGenerationTester;


using csly.generator.model.lexer;
using csly.generator.model.parser;
using csly.generator.model.parser.tree;
using csly.generator.sourceGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;



public partial class Program
{
    public static void Main(string[] args)
    {
        Generate();
        Run();
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
        var parser = new StaticExpressionParser();
        var lexer = new StaticLexer();
        while (true)
        {
            var choice = Console.ReadLine();
            if (string.IsNullOrEmpty(choice) || choice == "q" || choice == "quit")
            {
                Environment.Exit(0);
            }



            //var tok = (ExpressionToken id, string value, int position) => new Token<ExpressionToken>(id, value, new LexerPosition(position,0,position));


            //var tokens = new List<Token<ExpressionToken>>() { tok(ExpressionToken.INT,"2",0), tok(ExpressionToken.PLUS, "+", 1), tok(ExpressionToken.INT, "2", 2), new Token<ExpressionToken>() { IsEOS = true } };



            var lexerResult = lexer.Scan(choice.AsSpan());
            if (lexerResult.IsError)
            {
                Console.WriteLine($"Lexing failed: {lexerResult.Error}");
                return;

            }

            var result = parser.ParseNonTerminal_expression(lexerResult.Tokens, 0);
            if (result.IsOk)
            {
                Console.WriteLine("Parse succeeded");
                Console.WriteLine(result.Root.Dump("  "));
            }
            else
            {
                Console.WriteLine("Parse failed");
                foreach (var err in result.Errors)
                {
                    Console.WriteLine(err.ErrorMessage);
                }
            }
        }
    }
}