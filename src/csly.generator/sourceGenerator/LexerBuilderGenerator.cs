using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace csly.generator.sourceGenerator;

internal class LexerBuilderGenerator
{

    public List<string> Tokens { get; set; } = new List<string>();
    
    public string GenerateLexer(EnumDeclarationSyntax enumDeclarationSyntax, string outputType,
        Dictionary<string, SyntaxNode> declarationsByName, StaticLexerBuilder staticLexerBuilder)
    {
        string name = enumDeclarationSyntax.Identifier.ToString();
        StringBuilder builder = new();
        builder.AppendLine($"public IFluentLexerBuilder<{name}> GetLexer() {{");

        builder.AppendLine($"var builder = FluentLexerBuilder<{name}>.NewBuilder()");

        LexerSyntaxWalker walker = new(builder, name, declarationsByName, this, staticLexerBuilder);
        walker.Visit(enumDeclarationSyntax);
        

        builder.AppendLine(".UseLexerPostProcessor(UseTokenPostProcessor())");
        builder.AppendLine(".UseExtensionBuilder(UseTokenExtensions())");
        builder.AppendLine(";");
        builder.AppendLine("return builder;");
        
        builder.AppendLine($"}}");
        return builder.ToString();
    }
}