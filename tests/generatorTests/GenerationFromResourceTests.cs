using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using csly.generator.sourceGenerator;
using NFluent;
using Xunit;

namespace generatorTests;


public static class DiagnosticExtensions
{
    public static (int line, int column) GetPosition(this Diagnostic diagnostic)
    {
        var span = diagnostic.Location.GetLineSpan();

        var line = span.StartLinePosition.Line + 1;
        var column = span.StartLinePosition.Character + 1;

        return (line, column);
    }

    public static string GetPositionedMessage(this Diagnostic diagnostic)
    {
        var position = diagnostic.GetPosition();
        return $"{position.line}:{position.column} {diagnostic.GetMessage()}";
    }
}

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

    [Fact]
    public void TestContextTypeError()
    {
        var source = GetEmbeddedGrammar("contextTypeError");
        var result = GenerateSource(source, "ExprParser");
         var contextError= result.Diagnostics.FirstOrDefault(d => d.Id == CslyGeneratorErrors.TOO_MANY_CONTEXT_TYPES);
         Assert.NotNull(contextError);
         string message = contextError.GetMessage();
         Check.That(message).Contains("Dictionary<string, object>");
         Check.That(message).Contains("Dictionary<string, int>");
         Check.That(message).Contains("Dictionary<string, double>");
    }
    
    [Fact]
    public void TestTypeCheckErrors()
    {
        var source = GetEmbeddedGrammar("typechecking");
        var result = GenerateSource(source, "DummyParser");
        var contextErrors= result.Diagnostics.Where(d => d.Id == CslyGeneratorErrors.PARAMETER_TYPE_ERROR).ToList();
        Check.That(contextErrors).Not.IsNullOrEmpty();
        Check.That(contextErrors).CountIs(6);
        var errors = contextErrors.Select(x => x.GetPositionedMessage()).ToList();
        
        Check.That(errors).Contains("32:30 Parameter 'a' has type 'csly.generator.model.lexer.Token<TypeCheck.ABCToken>', but convention expects type 'List<Group<ABCToken,string>>'");
        Check.That(errors).Contains("38:33 Parameter 'test' has type 'string', but convention expects type 'List<Token<ABCToken>>'");
        Check.That(errors).Contains("44:36 Parameter 'test' has type 'string', but convention expects type 'List<string>'");
        Check.That(errors).Contains("50:26 Parameter 'a' has type 'string', but convention expects type 'ValueOption<string>'");
        Check.That(errors).Contains("56:34 Parameter 'a' has type 'object', but convention expects type 'Token<ABCToken>'");
        Check.That(errors).Contains("62:37 Parameter 'a' has type 'object', but convention expects type 'string'");
        


    }
}
