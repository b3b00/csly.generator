using System.Collections.Immutable;
using ebnf.grammar;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace csly.generator.sourceGenerator;

public static class TypeSymbolExtensions
{
    public static string ToSimpleDisplayString(this ITypeSymbol typeSymbol)
    {
        // Handle special types to return C# keywords (string, int, bool, etc.)
        if (typeSymbol.SpecialType != SpecialType.None)
        {
            return typeSymbol.SpecialType switch
            {
                SpecialType.System_Boolean => "bool",
                SpecialType.System_Byte => "byte",
                SpecialType.System_SByte => "sbyte",
                SpecialType.System_Int16 => "short",
                SpecialType.System_UInt16 => "ushort",
                SpecialType.System_Int32 => "int",
                SpecialType.System_UInt32 => "uint",
                SpecialType.System_Int64 => "long",
                SpecialType.System_UInt64 => "ulong",
                SpecialType.System_Char => "char",
                SpecialType.System_Single => "float",
                SpecialType.System_Double => "double",
                SpecialType.System_Decimal => "decimal",
                SpecialType.System_String => "string",
                SpecialType.System_Object => "object",
                SpecialType.System_Void => "void",
                _ => typeSymbol.Name
            };
        }

        // Handle generic types recursively
        if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var typeName = namedType.Name;
            var typeArgs = string.Join(", ", namedType.TypeArguments.Select(t => t.ToSimpleDisplayString()));
            return $"{typeName}<{typeArgs}>";
        }

        return typeSymbol.Name;
    }
}

public partial class ParserSyntaxWalker
{
    
    
    
    public void ExecuteMethodParamValidation(SourceProductionContext context, Compilation compilation,
        MethodDeclarationSyntax methodSyntax, Rule rule)
    {
     
        
        var diagnosticDescriptor = new DiagnosticDescriptor(
            id: CslyGeneratorErrors.PARAMETER_TYPE_ERROR,
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
                string actualType = paramSymbol.Type.ToSimpleDisplayString(); 
                if (actualType.Replace(" ","") != expectedType.Replace(" ",""))
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

    private string GetExpectedType(IClause clause)
    {
        string token = $"Token<{_lexerName}>";
        
        switch (clause)
        {
            case TerminalClause:
            {
                return token;
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
                return token;
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
                if (option.Clause is TerminalClause)
                {
                    return token;
                }
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

