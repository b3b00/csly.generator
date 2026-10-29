using System.Collections.Immutable;
using ebnf.grammar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace csly.generator.sourceGenerator;

public partial class ParserSyntaxWalker
{
    
    
    public void ExecuteMethodParamValidation(SourceProductionContext context, Compilation compilation,
        MethodDeclarationSyntax methodSyntax, Rule rule)
    {
        
        
        var diagnosticDescriptor = new DiagnosticDescriptor(
            id: "VAL002",
            title: "Parameter Type Mismatch",
            messageFormat: "Parameter '{0}' has type '{1}', but convention expects type '{2}'",
            category: "TypeChecking",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        
        var semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol methodSymbol) return;

        var realClauses = rule.Clauses.Where(clause => !(clause is TerminalClause terminal && terminal.Discarded)).ToList();
        int paramIndex = 0;
        foreach (var paramSymbol in methodSymbol.Parameters)
        {
            if (paramIndex >= realClauses.Count)
            {
                // the context case
                break;
            }
            var expectedTypeSymbol = GetExpectedType(paramSymbol, compilation, rule, paramIndex);

            if (expectedTypeSymbol != null)
            {
                if (!SymbolEqualityComparer.Default.Equals(paramSymbol.Type, expectedTypeSymbol))
                {
                    var syntaxRef = methodSyntax.ParameterList.Parameters
                        .FirstOrDefault(p => p.Identifier.Text == paramSymbol.Name)?
                        .GetLocation() ?? methodSyntax.GetLocation();

                    context.ReportDiagnostic(Diagnostic.Create(
                        diagnosticDescriptor,
                        syntaxRef,
                        paramSymbol.Name,
                        paramSymbol.Type.ToDisplayString(),
                        expectedTypeSymbol.ToDisplayString()));
                }
            }
            paramIndex++;
        }
    }

    
    private static readonly Dictionary<string, string> KeywordToMetadataName = new()
    {
        { "int", "System.Int32" },
        { "long", "System.Int64" },
        { "short", "System.Int16" },
        { "byte", "System.Byte" },
        { "sbyte", "System.SByte" },
        { "uint", "System.UInt32" },
        { "ulong", "System.UInt64" },
        { "ushort", "System.UInt16" },
        { "float", "System.Single" },
        { "double", "System.Double" },
        { "decimal", "System.Decimal" },
        { "bool", "System.Boolean" },
        { "char", "System.Char" },
        { "string", "System.String" },
        { "object", "System.Object" },
        { "void", "System.Void" }
    };
    
    private ITypeSymbol? GetExpectedType(IParameterSymbol parameter, Compilation compilation, Rule rule, int paramIndex)
    {
        
        var realClauses = rule.Clauses.Where(clause => !(clause is TerminalClause terminal && terminal.Discarded)).ToList();
        var clause  = realClauses[paramIndex];
        Console.WriteLine($"checking #{paramIndex} >{parameter.ToString()}< against clause >{clause.GetType().ToString()}< for rule [{rule.Dump(false)}]");
        // Example convention: Put your custom rules here
        switch (clause)
        {
            case TerminalClause:
            {
                ITypeSymbol lexerType = compilation.GetTypeByMetadataName(_lexerName);
                var token =  compilation.GetTypeByMetadataName("Token`1");
                if (token != null && lexerType != null)
                {
                    ITypeSymbol tokenOfLexer = token.Construct(lexerType);
                    return tokenOfLexer;
                }

                break;
            } 
            case NonTerminalClause:
            {
                ITypeSymbol outputType = compilation.GetTypeByMetadataName(_outputType);
                if (outputType == null)
                {
                    if (KeywordToMetadataName.TryGetValue(_outputType, out var metadataName))
                    outputType = compilation.GetTypeByMetadataName(metadataName);
                }

                return outputType;
            } 
            default:
            {
                return null;
            }
        }

        return null;

    }
}