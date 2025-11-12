
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
    private readonly TemplateEngine  _templateEngine;

    private Dictionary<string, TerminalClause> _terminalParsers = new();
    private Dictionary<string, NonTerminalClause> _nonTerminalParsers = new();
    private Dictionary<string, List<Rule>> _ruleParsers = new();
    
    
    public ParserBuilderGenerator(string lexerName, string parserName, string outputType, List<string> lexerGeneratorTokens)
    {
        _lexerName = lexerName;
        _parserName = parserName;
        _outputType = outputType;
        _lexerGeneratorTokens = lexerGeneratorTokens;
        _templateEngine = new TemplateEngine(_lexerName, _parserName, _outputType);
    }
    
    
    public string GenerateParser(ClassDeclarationSyntax classDeclarationSyntax)
    {
        string name = classDeclarationSyntax.Identifier.ToString();
        StaticParserBuilder staticParserBuilder = new StaticParserBuilder(_lexerGeneratorTokens);
        StringBuilder builder = new StringBuilder();
        ParserSyntaxWalker walker = new(builder, name, _lexerName, _outputType, staticParserBuilder);

        walker.Visit(classDeclarationSyntax);

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
    
        var helpers = GenerateHelpers();
    
        StringBuilder parsers = new StringBuilder();
        foreach (var rulesByHead in rules.GroupBy(x => x.Head))
        {
            var ruleForHead = rulesByHead.ToList();
            for (int i = 0; i < ruleForHead.Count(); i++)
            {
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
    
        var parser = _templateEngine.ApplyTemplate("parser.txt",
            additional: new Dictionary<string, string>()
            {
                { "<#HELPERS#>", helpers },
                { "<#PARSERS#>", parsers.ToString() }
            });
    
        return parser;
    }
    
    private string GenerateHelpers()
    {
        // STATIC : read resource
        var content = _templateEngine.ApplyTemplate("helpers.txt");
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
              content = _templateEngine.ApplyTemplate("explicitTerminalParser.txt",terminalClause.Name);   
            }
            else
            {
                content = _templateEngine.ApplyTemplate("terminalParser.txt", terminalClause.Name);
            }
        }
    
        builder.AppendLine(content).AppendLine();
    }
    
    private void GenerateNonTerminal(NonTerminalClause nonTerminalClause, StringBuilder builder)
    {
        var rules = _ruleParsers[nonTerminalClause.Name];
        StringBuilder calls = new();
        for (int i = 0; i < rules.Count(); i++)
        {
            string callTemplate = _templateEngine.ApplyTemplate("ruleCall.txt",nonTerminalClause.Name,
                additional: new Dictionary<string, string>()
            {
                {"<#LEADINGS#>",""}, // static : compute leadings for rule
                {"<#INDEX#>",i.ToString()}
            });
            calls.AppendLine(callTemplate);
        }
        
        string content = _templateEngine.ApplyTemplate("nonTerminalParser.txt",nonTerminalClause.Name,
            additional: new Dictionary<string, string>()
            {
                {"<#CALLS#>", calls.ToString()}
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
    
            children += $"r{i}";
            var clause = rule.Clauses[i];
            
            if (clause != null)
            {
                string call = "";
                if (clause is TerminalClause terminalClause)
                {
                    // STATIC : later , manage discarded tokens
                    call = _templateEngine.ApplyTemplate("terminalClause.txt", terminalClause.Name,
                        additional:new Dictionary<string,string>()
                        {
                            {"<#INDEX#>",i.ToString()}
                        });
                    AddClause(terminalClause);
                }
    
                if (clause is NonTerminalClause nonTerminalClause)
                {
                    call = _templateEngine.ApplyTemplate("nonTerminalClause.txt", nonTerminalClause.Name,
                        additional:new Dictionary<string,string>() {{"<#INDEX#>",i.ToString()}});
                    AddClause(nonTerminalClause);
                }
                clausesBuilder.AppendLine(call).AppendLine();
            }
        }

        var content = _templateEngine.ApplyTemplate("ruleParser.txt", rule.Name,
            additional: new Dictionary<string, string>()
            {
                { "<#CLAUSES#>", clausesBuilder.ToString() },
                { "<#RULE_COUNT#>", (rule.Clauses.Count - 1).ToString() },
                { "<#CHILDREN#>", string.Join(", ", children) },
                { "<#HEAD#>", rule.Head },
                { "<#INDEX#>", index.ToString() },
                { "<#RULESTRING#>", $"{rule.Head} : {string.Join(" ", Enumerable.Select<IClause, string>(rule.Clauses, x => x.Name))}" },
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
}