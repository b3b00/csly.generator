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
            if (attribute.Name.ToString() == "Operation")
            {
                var args = GetAttributeArgsAsStringArray(attribute, skip: 0);
                ;

                string tokenName = args[0].Trim(new[] { '"' });
                string infixArg = args[1].Trim(new[] { '"' }).Replace("Affix.", "");
                Affix affix = (Affix)System.Enum.Parse(typeof(Affix), infixArg);
                string associativityArg = args[2].Trim(new[] { '"' }).Replace("Associativity.", "");
                Associativity associativity = (Associativity)System.Enum.Parse(typeof(Associativity), associativityArg);
                int precedence = int.Parse(args[3]);

                var operation = new Operation(methodName, tokenName, affix, associativity, precedence);

                _staticParserBuilder.Model.AddOperation(operation);                
            }
            if (attribute.Name.ToString() == "Left")
            {
                var args = GetAttributeArgsAsStringArray(attribute, skip: 0);                
                string tokenName = args[0].Trim(new[] { '"' });
                int precedence = int.Parse(args[1]);
                var operation = new Operation(methodName, tokenName, Affix.InFix, Associativity.Left, precedence);
                _staticParserBuilder.Model.AddOperation(operation);                
            }
            if (attribute.Name.ToString() == "Right")
            {
                var args = GetAttributeArgsAsStringArray(attribute, skip: 0);                
                string tokenName = args[0].Trim(new[] { '"' });
                int precedence = int.Parse(args[1]);
                var operation = new Operation(methodName, tokenName, Affix.InFix, Associativity.Right, precedence);
                _staticParserBuilder.Model.AddOperation(operation);
            }
            if (attribute.Name.ToString() == "Prefix")
            {
                var args = GetAttributeArgsAsStringArray(attribute, skip: 0);                
                string tokenName = args[0].Trim(new[] { '"' });
                string associativityArg = args[1].Trim(new[] { '"' }).Replace("Associativity.", "");
                Associativity associativity = (Associativity)System.Enum.Parse(typeof(Associativity), associativityArg);
                int precedence = int.Parse(args[2]);
                var operation = new Operation(methodName, tokenName, Affix.PreFix, associativity, precedence);
                _staticParserBuilder.Model.AddOperation(operation);
            }
            if (attribute.Name.ToString() == "Postfix")
            {
                var args = GetAttributeArgsAsStringArray(attribute, skip: 0);                
                string tokenName = args[0].Trim(new[] { '"' });
                string associativityArg = args[1].Trim(new[] { '"' }).Replace("Associativity.", "");
                Associativity associativity = (Associativity)System.Enum.Parse(typeof(Associativity), associativityArg);
                int precedence = int.Parse(args[2]);
                var operation = new Operation(methodName, tokenName, Affix.PostFix, associativity, precedence);
                _staticParserBuilder.Model.AddOperation(operation);
            }
            if (attribute.Name.ToString() == "Production")
            {

                var ruleString = GetAttributeArgs(attribute, withLeadingComma: false);
                
                var rule = _staticParserBuilder.Parse(ruleString, node.Identifier.Text);
                for (int i = 0; i < rule.Clauses.Count; i++)
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
                        _staticParserBuilder.Model.AddRule(subRule);
                        rule.Clauses[i] = new NonTerminalClause(subRule.NonTerminalName) { IsGroup = true };
                    }
                    if (clause is ManyClause many)
                    {
                        if (many.manyClause is GroupClause mg)
                        {
                            Rule subRule = new Rule()
                            {
                                Clauses = mg.Clauses,
                                IsSubRule = true,
                                NonTerminalName = mg.Name
                            };
                            _staticParserBuilder.Model.AddRule(subRule);
                            many.manyClause = new NonTerminalClause(subRule.NonTerminalName) { IsGroup = true };
                        }
                    }
                    if (clause is OptionClause option)
                    {
                        if (option.Clause is GroupClause og)
                        {
                            Rule subRule = new Rule()
                            {
                                Clauses = og.Clauses,
                                IsSubRule = true,
                                NonTerminalName = og.Name
                            };
                            _staticParserBuilder.Model.AddRule(subRule);
                            option.Clause = new NonTerminalClause(subRule.NonTerminalName) { IsGroup = true };
                        }
                    }
                }
                _staticParserBuilder.Model.AddRule(rule);
                if (IsOperand(node))
                {
                    _staticParserBuilder.Model.AddOperand(new Operand(rule));
                }

            }
            
            
            if (!string.IsNullOrEmpty(nodeName))
            {
            
            }
        }
    }

    
    
    
    
    
    
}