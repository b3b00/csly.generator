using System.Linq;
using csly.ebnf.builder;
using ebnf.grammar;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace csly.generator.sourceGenerator;

public class ParserSyntaxWalker : CslySyntaxWalker
{
    
    StaticParserBuilder _staticParserBuilder;

    public ParserSyntaxWalker(string parserName, string lexerName, string outputType, StaticParserBuilder staticParserBuilder)
    {
        _staticParserBuilder = staticParserBuilder;
    }


    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        string name = node.Identifier.ToString();
        var attributes = node.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(x => x.Name.ToString() != nameof(model.parser.attributes.ParserRootAttribute))
            .ToList();
        foreach (var attribute in attributes)
        {
            VisitAttribute(attribute);
        }

        var methods = node.Members
            .ToList()
            .Where(x => x is MethodDeclarationSyntax)
            .Cast<MethodDeclarationSyntax>().ToList();
        foreach (var method in methods)
        {
            VisitMethodDeclaration(method);
        }
        _staticParserBuilder.ComputeLeaders();
    }

    public override void VisitAttribute(AttributeSyntax node)
    {
        var name = node.Name.ToString();
        switch (name)
        {
            case "AutoCloseIndentations":
                {
                    _staticParserBuilder.ParserOPtions.AutoCloseIndentations = true;
                    break;
                }
            case "UseMemoization":
                {
                    _staticParserBuilder.ParserOPtions.UseMemoization = true;
                    break;
                }
            case "BroadenTokenWindow":
                {
                    _staticParserBuilder.ParserOPtions.BroadenTokenWindow = true;   
                    break;
                }
            case "ParserRoot":
                {
                    var value = GetAttributeArgs(node, withLeadingComma: false).Trim(new[] { '"' });
                    _staticParserBuilder.ParserOPtions.StartingNonTerminal = value;
                    break;
                }
        }        
    }

    private bool IsOperand(MethodDeclarationSyntax method)
    {
        return method.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == "Operand");
    }
    
    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        string methodName = node.Identifier.ToString();
        var attributes = node.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(x => x.Name.ToString() != "Operand" && x.Name.ToString() != "NodeName").ToList();
        
        var nodeNames = node.AttributeLists
            .SelectMany(x => x.Attributes)
            .Where(x => x.Name.ToString() == "NodeName")
            .ToList();
        var nodeName = nodeNames.Any() ? nodeNames[0].ArgumentList.Arguments[0].Expression.ToString() : null;

        foreach (var attribute in attributes)
        {
            if (attribute.Name.ToString() == "Production")
            {
                if (IsOperand(node))
                {
                    var rule = GetAttributeArgs(attribute, withLeadingComma: false);                    
                }
                else
                {
                    var ruleString = GetAttributeArgs(attribute, withLeadingComma: false);
                    // STATIC : parse rule 
                    GeneratorLogger.Log($"\nparsing rule >>{ruleString}<<");
                    var rule = _staticParserBuilder.Parse(ruleString,node.Identifier.Text);      
                    for(int i = 0; i< rule.Clauses.Count; i++)
                    {
                        var clause = rule.Clauses[i];
                        if (clause is GroupClause group)
                        {
                            Rule subRule = new Rule()
                            {
                                Clauses = group.Clauses,
                                IsSubRule = true,
                                NonTerminalName = group.Name
                            };
                            _staticParserBuilder.Model.Add(subRule);
                            rule.Clauses[i] = new NonTerminalClause(subRule.NonTerminalName);
                        }
                    }
                    _staticParserBuilder.Model.Add(rule);
                    
                    GeneratorLogger.Log($"\nparsed rule >>{rule.Dump()}<<");
                }
            }
            // STATIC : operations later
            
            if (!string.IsNullOrEmpty(nodeName))
            {
                // STATIC : maybe later, or not
            }
        }
    }

    
    
    
    
    
    
}