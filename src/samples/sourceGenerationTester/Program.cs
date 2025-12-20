// See https://aka.ms/new-console-template for more information


using csly.generator.sourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpFileSystem.FileSystems;
using System;
using System.IO;
using System.Linq;

namespace sourceGenerationTester;

public static class Ext
{
    public static string Capitalize(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;
        if (s.Length == 1)
            return s.ToUpper();
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}

public  class Program
{
    public static void Main(string[] args)
    {
        var who = args[0];
        var where = args.Length > 1 ? args[1] : "c:/tmp/generation/";
        Generate(who, where);
        //Run();
        /*GoStatic();
        Run();*/
    }


    private static void Generate(string who, string where)
    {

        EmbeddedResourceFileSystem fs = new EmbeddedResourceFileSystem(typeof(Program).Assembly);
        var parser = fs.ReadAllText($"/samples/{who}.gram");

        var result = GenerateSource(parser, who);

        var contents = result.GeneratedTrees.ToList().ToDictionary(x => x.FilePath, x => x.ToString());
        var generatedFiles = result.GeneratedTrees.Select(x => new FileInfo(x.FilePath).Name);

        
        Directory.CreateDirectory(where);

        File.WriteAllText(Path.Combine(where, $"{who.Capitalize()}.cs"), parser);

        foreach (var file in contents)
        {
            FileInfo fileInfo = new FileInfo(file.Key);

            string fileName = Path.Combine(where, fileInfo.Name);

            if (fileInfo.Name.StartsWith("lexer."))
            {
                fileName = Path.Combine(where, "models", "lexer" + fileInfo.Name);
            }
            if (fileInfo.Name.StartsWith("parser."))
            {
                fileName = Path.Combine(where, "models", "parser" + fileInfo.Name);
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



    
}
