using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace csly.generator.sourceGenerator;

public class CslySyntaxWalker : CSharpSyntaxWalker
{
    protected string GetAttributeArgs(AttributeSyntax attribute, int skip = 0, bool withLeadingComma = true)
    {

        if (attribute.ArgumentList != null && attribute.ArgumentList.Arguments.Count > 0)
        {
            System.Collections.Generic.List<string> args = GetAttributeArgsAsStringArray(attribute, skip);
            if (args.Count > 0)
            {
                var strargs = string.Join(", ", args);

                if (withLeadingComma)
                {
                    return ", " + strargs;
                }

                return strargs;
            }
        }

        return string.Empty;
    }

    public static System.Collections.Generic.List<string> GetAttributeArgsAsStringArray(AttributeSyntax attribute, int skip = 0)
    {
        if (attribute.ArgumentList != null && attribute.ArgumentList.Arguments.Any())
        {
            return attribute.ArgumentList.Arguments.Skip(skip).Select(x =>
            {
                if (x.Expression is CastExpressionSyntax cast)
                {
                    if (cast.Expression is MemberAccessExpressionSyntax memberAccess)
                    {
                        var name = memberAccess.Name.ToString();
                        return name;
                        ;
                    }
                }
                var value = x.Expression.ToString();
                if (x.NameColon != null && x.NameColon.Name.Identifier.Text != "")
                {
                    return $"{x.NameColon.Name.Identifier.Text} :{value}";
                }

                return value;
            }).ToList();
        }
        return new List<string>();

    }

    public static System.Collections.Generic.List<AttributeArgumentSyntax> GetAttributeArgsArray(AttributeSyntax attribute, int skip = 0)
    {
        if (attribute.ArgumentList != null && attribute.ArgumentList.Arguments.Any())
        {
            return attribute.ArgumentList.Arguments.Skip(skip).Select(x =>
            {
                return x;
            }).ToList();
        }
        return new List<AttributeArgumentSyntax>();

    }

}