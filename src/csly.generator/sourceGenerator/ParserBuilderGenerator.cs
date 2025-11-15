
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csly.generator.model.parser.grammar;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace csly.generator.sourceGenerator;

public class ParserBuilderGenerator
{
    private readonly string _lexerName;
    private readonly string _parserName;
    private readonly string _outputType;
    private readonly List<string> _lexerGeneratorTokens;
    private readonly string _namespace;
    private readonly TemplateEngine  _templateEngine;

    private Dictionary<string, TerminalClause> _terminalParsers = new();
    private Dictionary<string, NonTerminalClause> _nonTerminalParsers = new();
    private Dictionary<string, List<Rule>> _ruleParsers = new();

    private List<Rule> _rules = new();


    public ParserBuilderGenerator(string lexerName, string parserName, string outputType, string ns,  List<string> lexerGeneratorTokens)
    {
        _lexerName = lexerName;
        _parserName = parserName;
        _outputType = outputType;
        _namespace = ns;
        _lexerGeneratorTokens = lexerGeneratorTokens;
        _templateEngine = new TemplateEngine(_lexerName, _parserName, _outputType);
    }
    
    
    public string GenerateParser(ClassDeclarationSyntax classDeclarationSyntax)
    {
        string name = classDeclarationSyntax.Identifier.ToString();
        StaticParserBuilder staticParserBuilder = new StaticParserBuilder(_lexerGeneratorTokens);
        
        ParserSyntaxWalker walker = new(name, _lexerName, _outputType, staticParserBuilder);

        walker.Visit(classDeclarationSyntax);
        _rules = staticParserBuilder.Model;
        var staticParser = GenerateStaticParser(staticParserBuilder.Model);
        
        var syntaxTree = CSharpSyntaxTree.ParseText(staticParser);
        var root = syntaxTree.GetRoot();
        
        System.IO.File.WriteAllText(System.IO.Path.Combine("c:/tmp/generation/",$"static{name}.cs"),root.ToString());
        
        return root.ToString();
        
        
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

    private string GenerateStaticParser(List<Rule> rules)
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
                GenerateRule(ruleForHead[i],parsers,i);    
            }
               
        }
    
        foreach (var terminalParser in _terminalParsers)
        {
            GenerateTerminal(terminalParser.Value,parsers);
        }
    
        foreach (var nonTerminalParser in _nonTerminalParsers)
        {
            GenerateNonTerminal(nonTerminalParser.Value,parsers);
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
              content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.ExplicitTerminalParserTemplate),terminalClause.Name);   
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
        var rules = _ruleParsers[nonTerminalClause.Name];
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
            var leaders = string.Join(", ",rule.Leaders.Distinct().Select(x => $"new LeadingToken<{_lexerName}>({_lexerName}.{x})"));
            string callTemplate = _templateEngine.ApplyTemplate(nameof(ParserTemplates.RuleCallTemplate),nonTerminalClause.Name,
                additional: new Dictionary<string, string>()
            {
                {"LEADINGS",leaders}, // static : compute leadings for rule
                {"INDEX",i.ToString()}
            });
            calls.AppendLine(callTemplate);
        }
        
        string content = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalParserTemplate),nonTerminalClause.Name,
            additional: new Dictionary<string, string>()
            {
                {"CALLS", calls.ToString()},
                {"EXPECTEDTOKENS",expecting }
            });
        builder.AppendLine(content).AppendLine();
    }
    
    private void GenerateRule(Rule rule, StringBuilder builder, int index)
    {
        AddRule(rule);
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
                if (clause is TerminalClause terminalClause)
                {
                    // STATIC : later , manage discarded tokens
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.TerminalClauseTemplate), terminalClause.Name,
                        additional:new Dictionary<string,string>()
                        {
                            {"INDEX",i.ToString()}
                        });
                    AddClause(terminalClause);
                }
    
                if (clause is NonTerminalClause nonTerminalClause)
                {
                    call = _templateEngine.ApplyTemplate(nameof(ParserTemplates.NonTerminalClauseTemplate), nonTerminalClause.Name,
                        additional:new Dictionary<string,string>() {{"INDEX",i.ToString()}});
                    AddClause(nonTerminalClause);
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
        builder.AppendLine(content);
    }

    private void AddRule(Rule rule)
    {
        List<Rule> parsers =  new List<Rule>();
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


    public string GenerateStaticVisitor()
    {
        
        StringBuilder builder = new();
        StringBuilder visitors = new StringBuilder();
        
        foreach (var rulesByHead in _rules.GroupBy(x => x.Head))
        {
               var nonTerminalVisitor = GenerateNonTerminalVisitor(rulesByHead.Key, rulesByHead.Count());
            visitors.AppendLine(nonTerminalVisitor);
            for (int i = 0; i < rulesByHead.Count(); i++)
           {
                var rule = rulesByHead.ToList()[i];
                var ruleVisitor = GenerateRuleVisitor(rule, i);
                visitors.AppendLine(ruleVisitor);
            }

        }

        var parser = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.VisitorTemplate),
            additional: new Dictionary<string, string>()
            {                
                { "VISITORS", visitors.ToString() },
                { "NAMESPACE", _namespace }                
            });

        return parser;
    }

    private string GenerateNonTerminalVisitor(string name, int count)
    {
        StringBuilder cases = new StringBuilder();
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

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.NonTerminalVisitorTemplate), name,
            additional: new Dictionary<string, string>() {
            {"VISITORS", cases.ToString()}
            }); 

        return content;
    }

    private string GenerateRuleVisitor(Rule rule, int index)
    {
        StringBuilder visitors = new StringBuilder();
        for (int i = 0; i < rule.Clauses.Count; i++)
        {
            var clause = rule.Clauses[i];
            var clauseVisitor = "";
            if (clause is TerminalClause terminalClause)
            {
                clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitTerminalTemplate), terminalClause.Name,
                    additional: new Dictionary<string, string>()
                    {
                        {"INDEX",i.ToString()}
                    });
            }
            if (clause is NonTerminalClause nonTerminalClause)
            {
                clauseVisitor = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.CallVisitNonTerminalTemplate), nonTerminalClause.Name,
                    additional: new Dictionary<string, string>()
                    {
                        {"INDEX",i.ToString()}
                    });
            }
            visitors.AppendLine(clauseVisitor);
        }
        var args = "";
        for (int i = 0; i < rule.Clauses.Count; i++)
        {
            if (i != 0)
            {
                args += ", ";
            }
            args += $"arg{i}";
        }

        var content = _templateEngine.ApplyTemplate(nameof(VisitorTemplates.RuleVisitorTemplate), rule.Name,
            additional: new Dictionary<string, string>()
            {
                {"INDEX",index.ToString() },
                {"VISITORS", visitors.ToString() },
                {"VISITOR", rule.MethodName },
                {"ARGS", args }
            });

        return content;
    }

}
