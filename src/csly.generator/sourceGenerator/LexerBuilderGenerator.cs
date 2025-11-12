using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace csly.generator.sourceGenerator;

public class LexerBuilderGenerator
{

    public List<string> Tokens { get; set; } = new List<string>();
    
    public string GenerateLexer(EnumDeclarationSyntax enumDeclarationSyntax, string outputType,
        Dictionary<string, SyntaxNode> declarationsByName)
    {
        string name = enumDeclarationSyntax.Identifier.ToString();
        StringBuilder builder = new();
        builder.AppendLine($"public IFluentLexerBuilder<{name}> GetLexer() {{");

        builder.AppendLine($"var builder = FluentLexerBuilder<{name}>.NewBuilder()");
        
        LexerSyntaxWalker walker = new(builder, name, declarationsByName, this);
        walker.Visit(enumDeclarationSyntax);
        

        builder.AppendLine(".UseLexerPostProcessor(UseTokenPostProcessor())");
        builder.AppendLine(".UseExtensionBuilder(UseTokenExtensions())");
        builder.AppendLine(";");
        builder.AppendLine("return builder;");
        
        builder.AppendLine($"}}");
        return builder.ToString();
    }
}