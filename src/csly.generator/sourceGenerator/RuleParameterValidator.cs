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

        var realClauses = rule.Clauses.Where(clause => !((clause is TerminalClause terminal && terminal.Discarded))
        && !(clause is ChoiceClause choice && choice.IsDiscarded)).ToList();
        int paramIndex = 0;
        foreach (var paramSymbol in methodSymbol.Parameters)
        {
            if (paramIndex >= realClauses.Count)
            {
                // the context case
                break;
            }
            var expectedType = GetExpectedType(realClauses[paramIndex]);

            if (expectedType != null)
            {
                if (paramSymbol.Type.ToDisplayString() != expectedType)
                //if (!SymbolEqualityComparer.Default.Equals(paramSymbol.Type, expectedTypeSymbol))
                {
                    var syntaxRef = methodSyntax.ParameterList.Parameters
                        .FirstOrDefault(p => p.Identifier.Text == paramSymbol.Name)?
                        .GetLocation() ?? methodSyntax.GetLocation();

                    context.ReportDiagnostic(Diagnostic.Create(
                        diagnosticDescriptor,
                        syntaxRef,
                        paramSymbol.Name,
                        paramSymbol.Type.ToDisplayString(),
                        expectedType));
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


    private string GetExpectedType(IClause clause)
    {
        switch (clause)
        {
            case TerminalClause:
            {
                return $"Token<{_lexerName}>";
            } 
            case NonTerminalClause:
            {
                return _outputType;
            }
            case ChoiceClause choice when choice.IsNonTerminalChoice:
            {
                return _outputType;
            }
            case ChoiceClause choice when choice.IsTerminalChoice:
            {
                return _lexerName;
            }
            case ManyClause many:
            {
                var expected = GetExpectedType(many.manyClause);
                return $"List<{expected}>";
            }
            case GroupClause group:
            {
                return $"Group<{_lexerName},{_outputType}>";
            }
            case OptionClause option:
            {
                var expected =  GetExpectedType(option.Clause);
                return $"ValueOption<{expected}>";
            }
            default:
            {
                return null;
            }
        }
    }
    
    
}

