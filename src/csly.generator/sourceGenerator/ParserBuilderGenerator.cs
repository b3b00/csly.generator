using csly.ebnf.builder;
using ebnf.grammar;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csly.generator.sourceGenerator;

public class ParserBuilderGenerator
{
    private readonly string _lexerName;
    private readonly string _parserName;
    private readonly string _outputType;
    private readonly List<string> _lexerGeneratorTokens;
    private readonly string _namespace;
    private readonly TemplateEngine _templateEngine;

    private Dictionary<string, TerminalClause> _terminalParsers = new();
    private Dictionary<string, NonTerminalClause> _nonTerminalParsers = new();
    private Dictionary<string, ZeroOrMoreClause> _zeroOrMoreParsers = new();
    private Dictionary<string, OneOrMoreClause> _oneOrMoreParsers = new();
    private Dictionary<string, OptionClause> _optionParsers = new();
    private Dictionary<string, ChoiceClause> _choiceParsers = new();
    private Dictionary<string, GroupClause> _groupParsers = new();
    private Dictionary<string, List<Rule>> _ruleParsers = new();
    private StaticParserBuilder _staticParserBuilder;
    private Visitor2Generator _visitor2Generator;

    private List<Rule> _rules = new();


    public ParserBuilderGenerator(string lexerName, string parserName, string outputType, string ns, List<string> lexerGeneratorTokens)
    {
        _lexerName = lexerName;
        _parserName = parserName;
        _outputType = outputType;
        _namespace = ns;
        _lexerGeneratorTokens = lexerGeneratorTokens;
        _templateEngine = new TemplateEngine(_lexerName, _parserName, _outputType, ns);
        
    }

    #region generate parser
    public string GenerateParser(ClassDeclarationSyntax classDeclarationSyntax)
    {
        string name = classDeclarationSyntax.Identifier.ToString();
        _staticParserBuilder = new StaticParserBuilder(_lexerGeneratorTokens, name, _lexerName, _outputType);

        ParserSyntaxWalker walker = new(name, _lexerName, _outputType, _staticParserBuilder);

        walker.Visit(classDeclarationSyntax);
        _rules = _staticParserBuilder.Model.Rules;
        _visitor2Generator = new Visitor2Generator(_lexerName, _parserName, _outputType, _namespace, _templateEngine, _staticParserBuilder.Model.Rules);

        ExpressionRulesGenerator expressionRulesGenerator = new();
        expressionRulesGenerator.Generate(_staticParserBuilder.Model);
        _staticParserBuilder.ComputeLeaders();

        GeneratorLogger.Log($"\nfound {_rules.Count} rules");
        var staticParser = GenerateStaticParser(_staticParserBuilder.Model.Rules, _staticParserBuilder.ParserOPtions.StartingNonTerminal);

        var syntaxTree = CSharpSyntaxTree.ParseText(staticParser);
        var root = syntaxTree.GetRoot();

        return root.ToString();


    }

    private void SetEmptyNonTerminals()
    {
        _rules.GroupBy(x => x.Head).ToDictionary(x => x.Key, x => x.ToList());

        var nonTerminalClauses = _rules.SelectMany(x => x.Clauses).OfType<NonTerminalClause>().ToList();

        foreach (var nt in nonTerminalClauses)
        {
            var rules = _rules.Where(x => x.Head == nt.Name).ToList();
            nt.SetMayBeEmpty(rules.Exists(x => x.MayBeEmpty));
        }
    }


    private string GetRootRule(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var rootAttribute = classDeclarationSyntax.AttributeLists.ToList().SelectMany(x => x.Attributes).ToList()
            .FirstOrDefault(x => x.Name.ToString() == "ParserRoot");
        if (rootAttribute == null)
        {
            return null;
        }
        var root = rootAttribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression?.ToString();
        return root;
    }

    private string GenerateStaticParser(List<Rule> rules, string startingNonTerminal)
    {
        StringBuilder builder = new();
        StringBuilder visitors = new StringBuilder();
        var helpers = GenerateHelpers();

        StringBuilder parsers = new StringBuilder();
        foreach (var rulesByHead in rules.GroupBy(x => x.Head))
        {
            var name = rulesByHead.Key;
            var ruleForHead = rulesByHead.ToList();
            for (int i = 0; i < ruleForHead.Count(); i++)
            {
                visitors.Append(name).Append("_").Append(i).AppendLine(",");
                GenerateRule(ruleForHead[i], parsers, i);
            }

        }

        foreach (var choiceParser in _choiceParsers)
        {
            GenerateChoice(choiceParser.Value, parsers);
        }

        foreach (var zeroOrMoreParser in _zeroOrMoreParsers)
        {
            GenerateZeroOrMore(zeroOrMoreParser.Value, parsers);
        }

        foreach (var oneOrMoreParser in _oneOrMoreParsers)
        {
            GenerateOneOrMore(oneOrMoreParser.Value, parsers);
        }

        foreach (var optionParser in _optionParsers)
        {
            GenerateOption(optionParser.Value, parsers);
        }

        foreach (var terminalParser in _terminalParsers)
        {
            GenerateTerminal(terminalParser.Value, parsers);
        }



        var missings = rules.Select(x => new NonTerminalClause(x.Head)).ToList();




        foreach (var nonTerminalParser in _nonTerminalParsers)
        {
            GenerateNonTerminal(nonTerminalParser.Value, parsers);
            missings = missings.Where(x => x.Name != nonTerminalParser.Key).ToList();
        }

        missings = missings.GroupBy(x => x.Name).Select(x => x.First()).ToList();

        // generate parser for non terminals that are not used in rules ( ex : root non terminal )
        if (missings.Count > 0)
        {
            foreach (var missing in missings)
            {
                GenerateNonTerminal(missing, parsers);
            }
        }

        var parser = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ParserTemplate),
            additional: new Dictionary<string, string>()
            {
                { "HELPERS", helpers },
                { "PARSERS", parsers.ToString() },
                { "NAMESPACE", _namespace },
                { "VISITORS", visitors.ToString()}
            });

        return parser;
    }



    private string GenerateHelpers()
    {
        // STATIC : read resource
        var content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.HelpersTemplate));
        return content;
    }

    private void GenerateTerminal(TerminalClause terminalClause, StringBuilder builder)
    {
        string content = "";
        if (terminalClause != null)
        {
            if (terminalClause.IsExplicit)
            {
                // STATIC : beware non alphanumeric chars in terminalClause.Name
                content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ExplicitTerminalParserTemplate), terminalClause.Name);
            }
            else
            {
                content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.TerminalParserTemplate), terminalClause.Name);
            }
        }

        builder.AppendLine(content).AppendLine();
    }

    private void GenerateNonTerminal(NonTerminalClause nonTerminalClause, StringBuilder builder)
    {
        if (_ruleParsers.TryGetValue(nonTerminalClause.Name, out var rules))
        {
            StringBuilder calls = new();

            var allLeaders = rules.SelectMany(r => r.Leaders)
                    .Distinct().ToList()
                    .Select(x => $"new LeadingToken<{_lexerName}>({_lexerName}.{x})");
            var expecting = string.Join(", ", allLeaders);

            for (int i = 0; i < rules.Count(); i++)
            {


                var rule = rules[i];
                if (rule.Leaders.Count != 0)
                {
                    ;
                }
                var leaders = string.Join(", ", rule.Leaders.Distinct().Select(x => $"new LeadingToken<{_lexerName}>({_lexerName}.{x})"));
                string callTemplate = _templateEngine.ApplyTemplate(nameof(ParserTemplates.RuleCallTemplate), nonTerminalClause.Name,
                    additional: new Dictionary<string, string>()
                {
                {"LEADINGS",leaders}, // static : compute leadings for rule
                {"INDEX",i.ToString()}
                });
                calls.AppendLine(callTemplate);
            }

            string content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalParserTemplate), nonTerminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                {"CALLS", calls.ToString()},
                {"EXPECTEDTOKENS",expecting }
                });
            builder.AppendLine(content).AppendLine();
        }
        else
        {
            ;
        }
    }

    private void GenerateChoice(ChoiceClause choiceClause, StringBuilder builder)
    {
        StringBuilder callsBuilder = new StringBuilder();
        for (int i = 0; i < choiceClause.Choices.Count; i++)
        {
            var innerClause = choiceClause.Choices[i];
            if (innerClause != null)
            {
                string call = "";
                if (innerClause is TerminalClause terminalClause)
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.TerminalClauseInChoiceTemplate), terminalClause.Name,
                                additional: new Dictionary<string, string>() { { "INDEX", i.ToString() } });
                    AddClause(terminalClause);
                }
                else if (innerClause is NonTerminalClause nonTerminalClause)
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalClauseInChoiceTemplate), nonTerminalClause.Name,
                                additional: new Dictionary<string, string>() { { "INDEX", i.ToString() } });
                    AddClause(nonTerminalClause);
                }
                callsBuilder.AppendLine(call).AppendLine();
            }
        }

        var expected = string.Join(",", choiceClause.Choices.Select(c =>
        {
            if (c is TerminalClause tc)
            {
                return $"new LeadingToken<{_lexerName}>({_lexerName}.{tc.Name})";
            }
            else if (c is NonTerminalClause ntc)
            {
                if (_ruleParsers.TryGetValue(ntc.Name, out var rules))
                {
                    var leaders = rules.SelectMany(r => r.Leaders).Distinct()
                        .Select(x => $"new LeadingToken<{_lexerName}>({_lexerName}.{x})");
                    return string.Join(", ", leaders);
                }
            }
            return "";
        }));

        var content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ChoiceParserTemplate), choiceClause.Name,
            additional: new Dictionary<string, string>()
            {
                {"CHOICECALLLIST", callsBuilder.ToString() },
                {"CHOICE_COUNT", (choiceClause.Choices.Count - 1).ToString() },
                {"EXPECTEDTOKENS", expected}
            });
        builder.AppendLine(content).AppendLine();
    }

    private string GenerateInnerClauseCallForMany(IClause innerClause, int index)
    {
        string call = "";
        switch (innerClause)
        {
            case TerminalClause terminalClause:
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.TerminalClauseForManyTemplate), innerClause.Name,
                        additional: new Dictionary<string, string>() { { "INDEX", index.ToString() } });
                    AddClause(terminalClause);
                    break;
                }
            case NonTerminalClause nonTerminalClause:
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalClauseForManyTemplate), innerClause.Name,
                        additional: new Dictionary<string, string>() {
                            { "INDEX", index.ToString() },
                            {"IS_GROUP", nonTerminalClause.IsGroup.ToString().ToLower() }
                        });
                    AddClause(nonTerminalClause);
                    break;
                }
            default:
                {
                    throw new NotImplementedException("many clause not implemented for " + innerClause.GetType().Name);
                }
        }
        return call;
    }

    private void GenerateZeroOrMore(ZeroOrMoreClause zeroOrMoreClause, StringBuilder builder)
    {
        string call = GenerateInnerClauseCallForMany(zeroOrMoreClause.manyClause, 0);


        var content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ZeroOrMoreParserTemplate), zeroOrMoreClause.Name,
            additional: new Dictionary<string, string>()
            {
                {"CALL", call },
                {"INNER_CLAUSE_NAME", zeroOrMoreClause.manyClause.Name}
            });
        builder.AppendLine(content).AppendLine();
    }

    private void GenerateOneOrMore(OneOrMoreClause oneOrMoreClause, StringBuilder builder)
    {
        string call = "";

        string firstcall = GenerateInnerClauseCallForMany(oneOrMoreClause.manyClause, 0);

        string manycall = GenerateInnerClauseCallForMany(oneOrMoreClause.manyClause, 1);



        var content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.OneOrMoreParserTemplate), oneOrMoreClause.Name,
            additional: new Dictionary<string, string>()
            {
                {"FIRSTCALL", firstcall },
                {"MANYCALL", manycall },
                {"INNER_CLAUSE_NAME", oneOrMoreClause.manyClause.Name}
            });
        builder.AppendLine(content).AppendLine();
    }

    private void GenerateOption(OptionClause optionClause, StringBuilder builder)
    {
        string call = GenerateInnerClauseCallForOption(optionClause.Clause, 0);
        var content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.OptionParserTemplate), optionClause.Name,
            additional: new Dictionary<string, string>()
            {
                {"CALL", call },
                {"INNER_CLAUSE_NAME", optionClause.Clause.Name},
                {"OUTPUT_TYPE",""}
            });
        builder.AppendLine(content).AppendLine();
    }

    private string GenerateInnerClauseCallForOption(IClause innerClause, int index)
    {
        string call = "";
        switch (innerClause)
        {
            case TerminalClause terminalClause:
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.TerminalClauseForOptionTemplate), innerClause.Name);
                    AddClause(terminalClause);
                    break;
                }
            case NonTerminalClause nonTerminalClause:
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalClauseForOptionTemplate), innerClause.Name);
                    AddClause(nonTerminalClause);
                    break;
                }
            default:
                {
                    throw new NotImplementedException("option clause not implemented for " + innerClause.GetType().Name);
                }
        }
        return call;
    }



    private void GenerateRule(Rule rule, StringBuilder builder, int index)
    {
        AddRule(rule);

        if (rule.IsExpressionRule && rule.ExpressionAffix != Affix.PreFix) // exclude prefixes 
        {
            GenerateOperationRule(rule, builder, index);
            return;
        }
        GeneratorLogger.Log($"\nGenerating rule parser for rule {rule.Name} : {rule.Dump()}");

        StringBuilder clausesBuilder = new StringBuilder();
        string children = "";
        for (int i = 0; i < rule.Clauses.Count; i++)
        {
            if (i != 0)
            {
                children += ", ";
            }

            children += $"r{i}.Root";
            var clause = rule.Clauses[i];

            if (clause != null)
            {
                string call = "";
                switch (clause)
                {

                    case TerminalClause terminalClause:
                        {
                            // STATIC : later , manage discarded tokens
                            call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.TerminalClauseTemplate), terminalClause.Name,
                                additional: new Dictionary<string, string>()
                                {
                            {"INDEX",i.ToString()}
                                });
                            AddClause(terminalClause);
                            break;
                        }

                    case NonTerminalClause nonTerminalClause:
                        {
                            call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalClauseTemplate), nonTerminalClause.Name,
                                additional: new Dictionary<string, string>() { { "INDEX", i.ToString() } });
                            AddClause(nonTerminalClause);
                            break;
                        }
                    case ManyClause manyClause:
                        {
                            call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ManyClauseTemplate), manyClause.Name,
                            additional: new Dictionary<string, string>() {
                                { "INDEX", i.ToString() },
                                {"NOT_EMPTY",(!manyClause.MayBeEmpty()).ToString().ToLower()}
                            });
                            AddClause(manyClause);
                            break;
                        }
                    case OptionClause optionClause:
                        {
                            call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.OptionClauseTemplate), optionClause.Name,
                            additional: new Dictionary<string, string>() {
                                { "INDEX", i.ToString() },
                                {"NOT_EMPTY","false"}
                            });
                            AddClause(optionClause);
                            break;
                        }
                    case ChoiceClause choiceClause:
                        {
                            call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ChoiceClauseTemplate), choiceClause.Name,
                            additional: new Dictionary<string, string>() {
                                { "INDEX", i.ToString() },
                                {"NOT_EMPTY","false"}
                            });
                            AddClause(choiceClause);
                            break;
                        }
                    case GroupClause groupClause:
                        {
                            AddClause(groupClause);
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException("clause class not implemented for " + clause.GetType().Name);
                        }
                }
                clausesBuilder.AppendLine(call).AppendLine();
            }
        }


        var content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.RuleParserTemplate), rule.Name,
            additional: new Dictionary<string, string>()
            {
                { "CLAUSES", clausesBuilder.ToString() },
                { "RULE_COUNT", (rule.Clauses.Count - 1).ToString() },
                { "CHILDREN", string.Join(", ", children) },
                { "HEAD", rule.Head },
                { "INDEX", index.ToString() },
                { "RULESTRING", $"{rule.Head} : {string.Join(" ", Enumerable.Select<IClause, string>(rule.Clauses, x => x.Name))}" },
            });
        GeneratorLogger.Log($"\nGenerated rule parser:\n{content}");
        builder.AppendLine(content);
    }


    private void GenerateOperationRule(Rule rule, StringBuilder builder, int index)
    {
        GeneratorLogger.Log($"\nGenerating expression rule parser for rule {rule.Name} : {rule.Dump()}");
        if (rule.IsInfixExpressionRule)
        {
            var lower = rule.Clauses[0].Name;
            var operatorClause = rule.Clauses[1].Name;

            if (rule.Clauses[1] is ChoiceClause operatorChoice)
            {
                AddClause(operatorChoice);
            }

            var parser = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ExpressionInfixRuleParser), rule.Name, additional: new Dictionary<string, string>()
        {
                { "HEAD", rule.Head },
                { "RULESTRING", rule.Dump() },
                { "INDEX", index.ToString() },
                {"AFFIX",rule.ExpressionAffix.ToString() },
                {"PRECEDENCE", rule.Precedence.ToString() },
                {"ASSOCIATIVITY", rule.Associativity.ToString() },
                {"LOWER_PRECEDENCE", lower }, // TODO
                {"OPERATOR",  operatorClause } // TODO
        });
            GeneratorLogger.Log($"\nGenerated infix expression parser:\n{parser}");
            builder.AppendLine(parser);
        }
        else if (rule.ExpressionAffix == Affix.PreFix)
        {
            if (rule.Clauses.Count == 2)
            {
                var lower = rule.Clauses[1].Name;
                var operatorClause = rule.Clauses[0].Name;
                var parser = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ExpressionPrefixRuleParser), rule.Name, additional: new Dictionary<string, string>()
            {
                { "HEAD", rule.Head },
                { "RULESTRING", rule.Dump() },
                { "INDEX", index.ToString() },
                {"AFFIX",rule.ExpressionAffix.ToString() },
                {"PRECEDENCE", rule.Precedence.ToString() },
                {"ASSOCIATIVITY", rule.Associativity.ToString() },
                {"LOWER_PRECEDENCE", lower }, // TODO
                {"OPERATOR",  operatorClause }
            });
                GeneratorLogger.Log($"\nGenerated prefix expression parser:\n{parser}");
                builder.AppendLine(parser);
            }
        }
        else if (rule.ExpressionAffix == Affix.PostFix)
        {
            var lower = rule.Clauses[0].Name;
            var operatorClause = rule.Clauses[1].Name;
            string tokenKind = "ParseTerminal";
            if (rule.Clauses[1] is ChoiceClause operatorChoice)
            {
                tokenKind = "ParseChoice";
                AddClause(operatorChoice);
            }
            if (rule.Clauses[1] is TerminalClause singleChoice)
            {
                tokenKind = "ParseTerminal";
                AddClause(singleChoice);
            }

            var parser = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ExpressionPostfixRuleParser), rule.Name, additional: new Dictionary<string, string>()
        {
                { "HEAD", rule.Head },
                { "RULESTRING", rule.Dump() },
                { "INDEX", index.ToString() },
                {"AFFIX",rule.ExpressionAffix.ToString() },
                {"PRECEDENCE", rule.Precedence.ToString() },
                {"ASSOCIATIVITY", rule.Associativity.ToString() },
                {"LOWER_PRECEDENCE", lower },
                {"OPERATOR",  operatorClause },
                {"TOKEN_KIND", tokenKind }
            });
            GeneratorLogger.Log($"\nGenerated infix expression parser:\n{parser}");
            builder.AppendLine(parser);
        }
        else
        {
            GeneratorLogger.Log($"\nExpression affix {rule.ExpressionAffix} not handled yet. This must be the expression root rule");
        }
    }

    private void AddClause(OptionClause optionClause)
    {
        if (!_zeroOrMoreParsers.ContainsKey(optionClause.Name))
        {
            _optionParsers[optionClause.Name] = optionClause;
        }
    }

    private void AddRule(Rule rule)
    {
        List<Rule> parsers = new List<Rule>();
        if (_ruleParsers.ContainsKey(rule.Name))
        {
            parsers = _ruleParsers[rule.Name];
        }
        parsers.Add(rule);
        _ruleParsers[rule.Name] = parsers;
    }

    private void AddClause(TerminalClause terminalClause)
    {
        if (!_terminalParsers.ContainsKey(terminalClause.Name))
        {
            _terminalParsers.Add(terminalClause.Name, terminalClause);
        }
    }

    private void AddClause(NonTerminalClause nonTerminalClause)
    {
        if (!_nonTerminalParsers.ContainsKey(nonTerminalClause.Name))
        {
            _nonTerminalParsers[nonTerminalClause.Name] = nonTerminalClause;
        }
    }

    private void AddClause(ManyClause manyClause)
    {
        if (manyClause is ZeroOrMoreClause zeroOrMoreClause)
        {
            AddClause(zeroOrMoreClause);
        }
        else if (manyClause is OneOrMoreClause oneOrMoreClause)
        {
            AddClause(oneOrMoreClause);
        }
    }

    private void AddClause(ZeroOrMoreClause zeroOrMoreClause)
    {
        if (!_zeroOrMoreParsers.ContainsKey(zeroOrMoreClause.Name))
        {
            _zeroOrMoreParsers[zeroOrMoreClause.Name] = zeroOrMoreClause;
        }
    }

    private void AddClause(OneOrMoreClause oneOrMoreClause)
    {
        if (!_oneOrMoreParsers.ContainsKey(oneOrMoreClause.Name))
        {
            _oneOrMoreParsers[oneOrMoreClause.Name] = oneOrMoreClause;
        }
    }

    private void AddClause(ChoiceClause choiceClause)
    {
        if (!_choiceParsers.ContainsKey(choiceClause.Name))
        {
            _choiceParsers[choiceClause.Name] = choiceClause;
        }
    }

    private void AddClause(GroupClause groupClause)
    {
        if (!_groupParsers.ContainsKey(groupClause.Name))
        {
            _groupParsers[groupClause.Name] = groupClause;
        }
    }

    #endregion

    #region generate visitor

    public string GenerateStaticVisitor()
    {

        StringBuilder builder = new();
        StringBuilder visitors = new StringBuilder();

        foreach (var rulesByHead in _rules.GroupBy(x => x.Head))
        {
            bool isGroup = rulesByHead.Count() == 1 && rulesByHead.ToList()[0].IsSubRule;
            bool isExpressionRule = rulesByHead.ToList().All(x => x.IsExpressionRule);
            bool isByPassRule = rulesByHead.ToList().All(x => x.IsByPassRule);
            var nonTerminalVisitor = GenerateNonTerminalVisitor(rulesByHead.Key, rulesByHead.Count(), isGroup, isExpressionRule, isByPassRule);
            visitors.AppendLine(nonTerminalVisitor);
            for (int i = 0; i < rulesByHead.Count(); i++)
            {
                var rule = rulesByHead.ToList()[i];
                var ruleVisitor = GenerateRuleVisitor(rule, i);
                visitors.AppendLine(ruleVisitor);
            }
        }

        _rules.SelectMany(_rules => _rules.Clauses).ToList().ForEach(clause =>
        {
            if (clause is ZeroOrMoreClause zeroOrMoreClause)
            {
                var zeroOrMoreVisitor = GenerateZeroOrMoreVisitor(zeroOrMoreClause, 0);
                visitors.AppendLine(zeroOrMoreVisitor);
            }
        });

        _rules.SelectMany(_rules => _rules.Clauses).ToList().ForEach(clause =>
        {
            if (clause is OneOrMoreClause oneOrMoreClause)
            {
                var zeroOrMoreVisitor = GenerateOneOrMoreVisitor(oneOrMoreClause, 0);
                visitors.AppendLine(zeroOrMoreVisitor);
            }
        });

        _rules.SelectMany(_rules => _rules.Clauses).ToList().ForEach(clause =>
        {
            if (clause is OptionClause optionClause)
            {
                var zeroOrMoreVisitor = GenerateOptionVisitor(optionClause, 0);
                visitors.AppendLine(zeroOrMoreVisitor);
            }
        });

        _rules.SelectMany(_rules => _rules.Clauses).ToList().ForEach(clause =>
        {
            if (clause is ChoiceClause choiceClause)
            {
                var choiceVisitor = GenerateChoiceVisitor(choiceClause, 0);
                visitors.AppendLine(choiceVisitor);
            }
        });

        var parser = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.VisitorTemplate),
            additional: new Dictionary<string, string>()
            {
                { "VISITORS", visitors.ToString() },
                { "NAMESPACE", _namespace }
            });

        return parser;
    }

    private string GenerateNonTerminalVisitor(string name, int count, bool isGroup, bool isExpressionRule, bool isByPassRule)
    {
        StringBuilder cases = new StringBuilder();
        if (isByPassRule)
        {
            return $@"public {_outputType} Visit{name}(SyntaxNode<ExprToken, {_outputType}> node)
{{
    
        return Visit{name}_0(node);

}}";
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var caseTemplate = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitRuleTemplate),
                    additional: new Dictionary<string, string>()
                    {
                    {"NAME",name },
                    {"INDEX",i.ToString() }
                    });
                cases.AppendLine(caseTemplate);
            }
        }
        string outputType = isGroup ? $"Group<{_lexerName},{_outputType}> " : _outputType;

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.NonTerminalVisitorTemplate), name,
            additional: new Dictionary<string, string>() {
            {"VISITORS", cases.ToString()},
                {"OUTPUT_TYPE",  outputType}
            });
        GeneratorLogger.Log($"\nGenerated non terminal visitor **{name}**:\n{content}");
        return content;
    }

    private string GenerateZeroOrMoreVisitor(ZeroOrMoreClause zeroOrMore, int count)
    {
        string clauseVisitor = "";
        string outputType = "";
        if (zeroOrMore.manyClause is TerminalClause terminalClause)
        {
            outputType = $"Token<{_lexerName}>";

            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitTerminalTemplate), terminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                        {"INDEX","i"}
                });
        }
        if (zeroOrMore.manyClause is NonTerminalClause nonTerminalClause)
        {
            outputType = nonTerminalClause.IsGroup ? $"Group<{_lexerName},{_outputType}>" : _outputType;
            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitNonTerminalTemplate), nonTerminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                        {"INDEX","i"}
                });
        }

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.ZeroOrMoreVisitorTemplate), zeroOrMore.Name,
            additional: new Dictionary<string, string>() {
            {"VISITOR", clauseVisitor},
            {"CLAUSE_OUTPUT", outputType },
            });

        return content;
    }

    private string GenerateOneOrMoreVisitor(OneOrMoreClause oneOrMore, int count)
    {
        string clauseVisitor = "";
        string outputType = "";
        if (oneOrMore.manyClause is TerminalClause terminalClause)
        {
            outputType = $"Token<{_lexerName}>";

            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitTerminalTemplate), terminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                        {"INDEX","i"}
                });
        }
        if (oneOrMore.manyClause is NonTerminalClause nonTerminalClause)
        {
            outputType = nonTerminalClause.IsGroup ? $"Group<{_lexerName},{_outputType}>" : _outputType;
            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitNonTerminalTemplate), nonTerminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                        {"INDEX","i"}
                });
        }

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.OneOrMoreVisitorTemplate), oneOrMore.Name,
            additional: new Dictionary<string, string>() {
            {"VISITOR", clauseVisitor},
            {"CLAUSE_OUTPUT", outputType },
            });

        return content;
    }

    private string GenerateOptionVisitor(OptionClause optionClause, int count)
    {
        string clauseVisitor = "";
        string outputType = "";
        string emptyValue = "";
        if (optionClause.Clause is TerminalClause terminalClause)
        {
            outputType = $"Token<{_lexerName}>";

            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitTerminalTemplate), terminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                        {"INDEX","0"}
                });
            emptyValue = $"new Token<{_lexerName}>() {{ IsEmpty = true }};";
        }
        if (optionClause.Clause is NonTerminalClause nonTerminalClause)
        {

            outputType = $"ValueOption<Group<{_lexerName},{_outputType}>>";
            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitNonTerminalTemplate), nonTerminalClause.Name,
                additional: new Dictionary<string, string>()
                {
                        {"INDEX","0"}
                });
            emptyValue = $"new {outputType}()";
        }


        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.OptionVisitorTemplate), optionClause.Name,
            additional: new Dictionary<string, string>() {
            {"EMPTY_VALUE", emptyValue},
            {"VISITOR", clauseVisitor},
            {"CLAUSE_OUTPUT", outputType },
            });

        return content;
    }

    private string GenerateChoiceVisitor(ChoiceClause choiceClause, int count)
    {
        if (choiceClause.IsTerminalChoice)
        {
            var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.TerminalChoiceVisitorTemplate), choiceClause.Name);
            return content;
        }
        else
        {
            var choices = string.Join("\n", choiceClause.Choices.Select((c, i) =>
            {
                var caseTemplate = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.NonTerminalChoiceVisitorCall),
                    c.Name);
                return caseTemplate;
            }));
            var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.NonTerminalChoiceVisitorTemplate), choiceClause.Name, additional:
                new Dictionary<string, string>()
                {
                    { "CHOICES", choices }
                });
            return content;
        }
    }

    private string GenerateRuleVisitor(Rule rule, int index)
    {
        if (rule.IsExpressionRule && rule.IsInfixExpressionRule)
        {
            return GenerateInfixExpressionVisitor(rule, index);
        }
        if (rule.IsExpressionRule && rule.ExpressionAffix == Affix.PostFix)
        {
            return GeneratePostfixExpressionVisitor(rule, index);
        }
        if (rule.IsExpressionRule && rule.ExpressionAffix == Affix.PreFix && rule.IsByPassRule && rule.Clauses.Count == 1)
        {
            return $@"
public int VisitExpr_Prec_100_{index}(SyntaxNode<{_lexerName}, {_outputType}> node)
    {{        
        return Visit{rule.Clauses[0].Name}(node.Children[0] as SyntaxNode<ExprToken, int>);
    }}";
        }
        if (rule.ExpressionAffix == Affix.PostFix)
        {
            ;
        }

        GeneratorLogger.Log($"\nGenerating rule visitor for rule {rule.Name} : {rule.Dump()}");

        StringBuilder visitors = new StringBuilder();

        for (int i = 0; i < rule.Clauses.Count; i++)
        {
            var clause = rule.Clauses[i];
            var clauseVisitor = "";

            switch (clause)
            {
                case TerminalClause terminalClause:
                    {
                        if (!terminalClause.Discarded)
                        {
                            clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitTerminalTemplate), terminalClause.Name,
                            additional: new Dictionary<string, string>()
                            {
                        {"INDEX",i.ToString()}
                            });
                        }
                        break;
                    }
                case NonTerminalClause nonTerminalClause:
                    {
                        clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitNonTerminalTemplate), nonTerminalClause.Name,
                            additional: new Dictionary<string, string>()
                            {
                        {"INDEX",i.ToString()}
                            });
                        break;
                    }
                case ZeroOrMoreClause zeroOrMoreClause:
                    {
                        clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitManyTemplate), zeroOrMoreClause.Name,
                            additional: new Dictionary<string, string>()
                            {
                        {"INDEX",i.ToString()}
                            });
                        break;
                    }
                case OneOrMoreClause oneOrMoreClause:
                    {
                        clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitManyTemplate), oneOrMoreClause.Name,
                            additional: new Dictionary<string, string>()
                            {
                        {"INDEX",i.ToString()}
                            });
                        break;
                    }
                case OptionClause optionClause:
                    {
                        clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitOptionTemplate), optionClause.Name,
                            additional: new Dictionary<string, string>()
                            {
                        {"INDEX",i.ToString()}
                            });
                        break;
                    }
                case ChoiceClause choiceClause:
                    {
                        clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitChoiceTemplate), choiceClause.Name,
                            additional: new Dictionary<string, string>()
                            {
                        {"INDEX",i.ToString()},
                                {"NODE_TYPE",choiceClause.IsTerminalChoice ? "SyntaxLeaf" : "SyntaxNode"}
                            });
                        break;
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("clause visitor not implemented for " + clause.GetType().Name);
                    }
            }
            visitors.AppendLine(clauseVisitor);
            if (rule.IsByPassRule)
            {
                break;
            }
        }
        var args = "";
        bool started = false;
        for (int i = 0; i < rule.Clauses.Count; i++)
        {
            if (rule.Clauses[i] is TerminalClause tc && tc.Discarded)
            {
                continue;
            }
            if (started)
            {
                args += ", ";
            }
            started = true;
            args += $"arg{i}";
        }

        string returnValue = "";
        string returnType = "";
        if (rule.IsSubRule)
        {
            returnType = $"Group<{_lexerName}, {_outputType}>";
            StringBuilder groupReturnValue = new StringBuilder();
            groupReturnValue.AppendLine($"var group = new Group<{_lexerName}, {_outputType}>();");
            for (int i = 0; i < rule.Clauses.Count; i++)
            {
                var clause = rule.Clauses[i];
                if (clause is TerminalClause term && term.Discarded)
                {
                    continue;
                }
                groupReturnValue.AppendLine($"group.Add(\"{clause.Name}\",arg{i});");
            }
            groupReturnValue.AppendLine($"return group;");
            returnValue = groupReturnValue.ToString();
        }
        else
        {
            if (string.IsNullOrEmpty(rule.MethodName))
            {
                Console.WriteLine($"Warning: rule {rule.Name} has no associated method name for visitor generation. maube bypass node ? {rule.IsByPassRule}");
            }
            returnType = _outputType;
            if (rule.IsByPassRule)
            {
                returnValue = $"return arg0;";
            }
            else
            {
                if (rule.IsExpressionRule && rule.ExpressionAffix == Affix.PreFix)
                {
                    StringBuilder operatorSwitch = new StringBuilder();
                    operatorSwitch.AppendLine("switch(arg0.TokenID) {");
                    foreach (var op in rule.TokenToVisitorMethodName)
                    {
                        operatorSwitch.AppendLine($" case {_lexerName}.{op.Key} :");
                        operatorSwitch.AppendLine($"\treturn _instance.{op.Value}({args});");
                        break;
                    }
                    operatorSwitch.AppendLine(@$"default: throw new NotImplementedException($""Operator {{arg0.TokenID}} not implemented for precedence {rule.Precedence}"");");
                    operatorSwitch.AppendLine("}");
                    returnValue = operatorSwitch.ToString();
                }
                else
                {
                    returnValue = $"return _instance.{rule.MethodName}({args});";
                }
            }
        }

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.RuleVisitorTemplate), rule.Name,
            additional: new Dictionary<string, string>()
            {
                {"INDEX",index.ToString() },
                {"VISITORS", visitors.ToString() },
                {"RETURN", returnValue },
                {"RETURN_TYPE", returnType }
            });
        GeneratorLogger.Log($"\nGenerated rule visitor for rule {rule.Name} : \n{rule.Dump()}");
        GeneratorLogger.Log($"\n{content}");
        GeneratorLogger.Log("==============================");
        return content;
    }


    private string GenerateInfixExpressionVisitor(Rule rule, int index)
    {
        GeneratorLogger.Log($"\nGenerating infix expression visitor for rule {rule.Name} : {rule.Dump()}");

        var operatorClause = rule.Clauses[1];
        StringBuilder returnBuilder = new StringBuilder();

        if (operatorClause is ChoiceClause operatorChoice)
        {
            returnBuilder.AppendLine("switch(arg1.TokenID) {");
            foreach (var choice in operatorChoice.Choices)
            {
                if (choice is TerminalClause terminalChoice)
                {
                    if (rule.TokenToVisitorMethodName.TryGetValue(terminalChoice.Name, out var methodName))
                    {
                        returnBuilder.AppendLine(@$" case {_lexerName}.{terminalChoice.Name} :
{{
    return _instance.{methodName}(arg0, arg1, arg2);    
    break;
}}");
                    }
                }
            }
            returnBuilder.AppendLine(@$"
default: {{
throw new NotImplementedException($""Operator {{arg1.TokenID}} not implemented for precedence {rule.Precedence}"");
        }}
}}");
        }
        else if (rule.Clauses[1] is TerminalClause terminalClause)
        {
            if (rule.TokenToVisitorMethodName.TryGetValue(terminalClause.Name, out var methodName))
            {
                returnBuilder.AppendLine(@$" case <#LEXER#>.{terminalClause.Name} :
{{
    return _instance.{methodName}(arg0, arg1, arg2);    
    break;
}}");
            }
        }

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.InfixExpressionVisitorTemplate), rule.Name,
            additional: new Dictionary<string, string>()
            {
                {"LEFT_NAME", rule.Clauses[0].Name },
                {"RIGHT_NAME", rule.Clauses[2].Name },
                {"INDEX",index.ToString() },
                {"METHOD_NAME", rule.MethodName }, // TODO there may are multiple methods for the same rule
                {"LOWER_PRECEDENCE_VISITOR", rule.Clauses[0].Name },
                {"RETURN_TYPE", _outputType },
                    {"RETURN", returnBuilder.ToString() },
            });
        GeneratorLogger.Log($"\nGenerated infix expression visitor:\n{content}");
        return content;
    }

    private string GeneratePostfixExpressionVisitor(Rule rule, int index)
    {
        GeneratorLogger.Log($"\nGenerating postfix expression visitor for rule {rule.Name} : {rule.Dump()}");

        var operatorClause = rule.Clauses[1];
        StringBuilder returnBuilder = new StringBuilder();

        if (operatorClause is ChoiceClause operatorChoice)
        {
            returnBuilder.AppendLine("switch(arg1.TokenID) {");
            foreach (var choice in operatorChoice.Choices)
            {
                if (choice is TerminalClause terminalChoice)
                {
                    if (rule.TokenToVisitorMethodName.TryGetValue(terminalChoice.Name, out var methodName))
                    {
                        returnBuilder.AppendLine(@$" case {_lexerName}.{terminalChoice.Name} :
{{
    return _instance.{methodName}(arg0, arg1);    
    break;
}}");
                    }
                }
            }
            returnBuilder.AppendLine(@$"
default: {{
throw new NotImplementedException($""Operator {{arg1.TokenID}} not implemented for precedence {rule.Precedence}"");
        }}
}}");
        }
        else if (rule.Clauses[1] is TerminalClause terminalClause)
        {
            if (rule.TokenToVisitorMethodName.TryGetValue(terminalClause.Name, out var methodName))
            {
                returnBuilder.AppendLine(@$"return _instance.{methodName}(arg0, arg1);");
            }
        }

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.PostfixExpressionVisitorTemplate), rule.Name,
            additional: new Dictionary<string, string>()
            {
                {"LEFT_NAME", rule.Clauses[0].Name },                
                {"INDEX",index.ToString() },
                {"METHOD_NAME", rule.MethodName }, // TODO there may are multiple methods for the same rule
                {"LOWER_PRECEDENCE_VISITOR", rule.Clauses[0].Name },
                {"RETURN_TYPE", _outputType },
                    {"RETURN", returnBuilder.ToString() },
            });
        GeneratorLogger.Log($"\nGenerated infix expression visitor:\n{content}");
        return content;
    }
    #endregion

    #region generate entry point

    public string GenerateEntryPoint()
    {
        var root = _staticParserBuilder.ParserOPtions.StartingNonTerminal;
        var content = _templateEngine.ApplyTemplate("EntryPointParserTemplate", additional: new Dictionary<string, string>()
        {
            {"ROOT",root } //TODO
        });
        return content;
    }

    internal string GenerateVisitor2()
    {
        return _visitor2Generator.GenerateVisitor();
    }

    #endregion




}
