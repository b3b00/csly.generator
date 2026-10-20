using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using csly.generator.sourceGenerator;
using Xunit;

namespace generatorTests;

public class GenerationFromResourceTests
{
    private static readonly Assembly CslyGeneratorAssembly =
        typeof(CslyParserGenerator).Assembly;

    private static readonly MetadataReference[] References =
    {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(CslyGeneratorAssembly.Location)
    };

    private static string GetEmbeddedGrammar(string name)
    {
        var assembly = typeof(GenerationFromResourceTests).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(x => x.EndsWith($".grammars.{name}.gram", System.StringComparison.Ordinal));
        Assert.NotNull(resourceName);
        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static GeneratorDriverRunResult GenerateSource(string source, string className)
    {
        var generator = new CslyParserGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator });

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            className,
            new[] { syntaxTree },
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return driver.RunGenerators(compilation).GetRunResult();
    }

    private static bool HasLeftRecursionError(GeneratorDriverRunResult result)
    {
        return result.Diagnostics.Any(d => d.Id == CslyGeneratorErrors.LEFT_RECURSION_DETECTED);
    }

    public static IEnumerable<object[]> LeftRecursionGrammarData =>
        new[]
        {
            new object[] { "LeftRecursive", "direct left recursion : expression : expression PLUS expression" },
            new object[] { "IndirectLeftRecursive", "indirect left recursion : expression -> term -> expression" },
            new object[] { "LeftRecursiveOption", "left recursion through nullable Option : expression : expression? A" },
            new object[] { "LeftRecursiveZeroOrMore", "left recursion through nullable ZeroOrMore : expression : expression* B" },
            new object[] { "LeftRecursionInAlternateInGroup", "left recursion through alternate in group : leftrec: ([leftrec | right])" },
        };

    [Theory]
    [MemberData(nameof(LeftRecursionGrammarData))]
    public void LeftRecursiveGrammarFailsGeneration(string grammarName, string description)
    {
        var source = GetEmbeddedGrammar(grammarName);
        var result = GenerateSource(source, grammarName);

        Assert.True(HasLeftRecursionError(result),
            $"Expected left recursion error (CSLY0007) for {description} ({grammarName}), but got: " +
            string.Join("; ", result.Diagnostics.Select(d => d.ToString())));

        Assert.True(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
            $"Expected an error severity diagnostic for {description} ({grammarName})");
    }

    [Fact]
    public void NonLeftRecursiveGrammarSucceeds()
    {
        var source = GetEmbeddedGrammar("NoLeftRecursive");
        var result = GenerateSource(source, "NoLeftRecursive");

        Assert.False(HasLeftRecursionError(result),
            $"Unexpected left recursion error: {string.Join("; ", result.Diagnostics.Where(d => d.Id == CslyGeneratorErrors.LEFT_RECURSION_DETECTED).Select(d => d.ToString()))}");
    }

    [Fact]
    public void GeneratedOutputContainsParserForNonLeftRecursiveGrammar()
    {
        var source = GetEmbeddedGrammar("NoLeftRecursive");
        var result = GenerateSource(source, "NoLeftRecursive");
        

        Assert.False(HasLeftRecursionError(result));
        Assert.NotEmpty(result.GeneratedTrees);
        Assert.True(result.GeneratedTrees.Any(t => t.FilePath.EndsWith(".g.cs")));
    }
}
