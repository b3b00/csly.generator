using csly.ebnf.builder;
using ebnf.grammar;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csly.generator.sourceGenerator
{
    internal class Visitor2Generator
    {

        private readonly string _lexerName;
        private readonly string _parserName;
        private readonly string _outputType;
        private readonly List<string> _lexerGeneratorTokens;
        private readonly string _namespace;
        private readonly TemplateEngine _templateEngine;
        private readonly List<Rule> _rules = new();

        public Visitor2Generator(string lexerName, string parserName, string outputType, string ns, TemplateEngine templateEngine, List<Rule> rules)
        {
            _lexerName = lexerName;
            _parserName = parserName;
            _outputType = outputType;
            _namespace = ns;
            _templateEngine = templateEngine;
            _rules = rules;
        }

        public string GenerateVisitor()
        {
            StringBuilder builder = new();


            // DISPATCHERS
            // case <#LEXER#>.{xxxx} :
            //   return Visit{NonTerminal}_{index}(node);
            StringBuilder dispatchers = new StringBuilder();
            foreach (var rulesByHead in _rules.GroupBy(x => x.Head))
            {
                dispatchers.AppendLine($"// Visitors for {rulesByHead.Key}");
                var first = rulesByHead.First();
               
                for (int i = 0; i < rulesByHead.Count(); i++)
                {
                    var r = rulesByHead.ToList()[i];
                    if (!r.IsSubRule)
                    {
                        int index = r.IsExpressionRule ? 0 : i;
                        var prefixCase = $@"case ""{rulesByHead.Key}_{i}"":
                    return Visit{rulesByHead.Key}_{index}(node);";
                        dispatchers.AppendLine(prefixCase);
                    }                    
                }
            }   

            StringBuilder visitors = new StringBuilder();

            foreach (var rulesByHead in _rules.GroupBy(x => x.Head))
            {
                bool isGroup = rulesByHead.Count() == 1 && rulesByHead.ToList()[0].IsSubRule;
                bool isExpressionRule = rulesByHead.ToList().All(x => x.IsExpressionRule);
                bool isByPassRule = rulesByHead.ToList().All(x => x.IsByPassRule);

                var first = rulesByHead.First();

                if (first.IsInfixExpressionRule)
                {
                    var infixVisitor = GenerateInfixExpressionVisitor(first, 0);
                    visitors.AppendLine(infixVisitor);
                    continue;
                }
                else if (first.ExpressionAffix == Affix.PostFix)
                {
                    var postfixVisitor = GeneratePostfixExpressionVisitor(first, 0);
                    visitors.AppendLine(postfixVisitor);
                    continue;
                }
                else if (first.ExpressionAffix == Affix.PreFix)
                {
                    var postfixVisitor = GeneratePrefixExpressionVisitor(first, 0);
                    visitors.AppendLine(postfixVisitor);
                    continue;
                }
                else if (first.ExpressionAffix == Affix.PreFix && first.IsByPassRule && first.Clauses.Count == 1)
                {
                    var prefixVisitor = "";
                    visitors.AppendLine(prefixVisitor);
                    continue;
                }
                else
                {
                    if (!first.IsSubRule)
                    {
                        int i = 0;
                        foreach(var rule in rulesByHead)
                        {
                            var v = GenerateVisitorRule(rule, i);
                            visitors.AppendLine(v);
                            i++;
                        }

                        
                        continue;
                    }
                }
            }


            var visitor = _templateEngine.ApplyTemplate("Visitor2Template",
                additional: new Dictionary<string, string>()
                {
                { "VISITORS", visitors.ToString() },
                { "NAMESPACE", _namespace },
                {"DISPATCHERS", dispatchers.ToString() }
                });
            return visitor;
        }


        private string GenerateVisitorRule(Rule rule, int index)
        {
            string args = "";
            StringBuilder compute = new StringBuilder();
            bool started = false;
            for (int i = 0; i < rule.Clauses.Count; i++)
            {
                var clause = rule.Clauses[i];
                if (clause is TerminalClause terminal && terminal.Discarded)
                {                    
                    continue;
                }
                if (started)
                {
                    args += ", ";
                }
                started = true;
                args += $"arg{i}";

                if (clause is TerminalClause || clause is ChoiceClause choice && choice.IsTerminalChoice)
                {
                    compute.AppendLine($"var arg{i} = VisitTerminal(node,{i});");
                }
                else if (clause is ManyClause many)
                {
                    if (many.manyClause is NonTerminalClause nt)
                    {
                        if (!nt.IsGroup)
                        {
                            compute.AppendLine($"var arg{i} = VisitManyNonTerminal(node,{i});");
                        }
                        else
                        {
                            compute.AppendLine($"var arg{i} = VisitManyGroup(node,{i});");
                        }
                    }
                    else if (many.manyClause is TerminalClause t)
                    {
                        compute.AppendLine($"var arg{i} = VisitManyTerminal(node,{i});");
                    }                    
                }
                else if (clause is GroupClause)
                {
                    compute.AppendLine($"var arg{i} = VisitGroup(node, {i});");
                }                
                else if (clause is OptionClause option)
                        {
                    if (option.Clause is TerminalClause)
                    {
                        compute.AppendLine($"var arg{i} = VisitOptionalTerminal(node,{i});");
                    }
                    else if (option.Clause is NonTerminalClause nt)
                    {
                        if (!nt.IsGroup)
                        {
                            compute.AppendLine($"var arg{i} = VisitOptionalNonTerminal(node,{i});");
                        }
                        else
                        {
                            compute.AppendLine($"var arg{i} = VisitOptionalGroup(node,{i});");
                        }
                    }

                        }
                        else
                        {
                            compute.AppendLine($"var arg{i} = VisitNonTerminal(node,{i});");
                        }
            }

            var content = _templateEngine.ApplyTemplate(rule.IsByPassRule ? "ByPassRuleVisitor" : "RuleVisitor", rule.Name,
                additional: new Dictionary<string, string>()
                {
                    {"INDEX", index.ToString() },
                {"COMPUTE_ARGS", compute.ToString() },
                {"ARGS", args },
                {"METHOD", rule.MethodName },
                });
            GeneratorLogger.Log($"\nGenerated infix expression visitor:\n{content}");
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
}}");
                }
            }

            var content = _templateEngine.ApplyTemplate("InfixVisitor", rule.Name,
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

            var content = _templateEngine.ApplyTemplate("PostfixVisitor", rule.Name,
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

        private string GeneratePrefixExpressionVisitor(Rule rule, int index)
        {
            GeneratorLogger.Log($"\nGenerating postfix expression visitor for rule {rule.Name} : {rule.Dump()}");

            var operatorClause = rule.Clauses[0];
            StringBuilder returnBuilder = new StringBuilder();

            if (operatorClause is ChoiceClause operatorChoice)
            {
                returnBuilder.AppendLine("switch(arg0.TokenID) {");
                foreach (var choice in operatorChoice.Choices)
                {
                    if (choice is TerminalClause terminalChoice)
                    {
                        if (rule.TokenToVisitorMethodName.TryGetValue(terminalChoice.Name, out var methodName))
                        {
                            returnBuilder.AppendLine(@$" case {_lexerName}.{terminalChoice.Name} :
{{
    return _instance.{methodName}(arg0, arg1);
}}");
                        }
                    }
                }
                returnBuilder.AppendLine(@$"
default: {{
throw new NotImplementedException($""Operator {{arg0.TokenID}} not implemented for precedence {rule.Precedence}"");
        }}
}}");
            }
            else if (operatorClause is TerminalClause terminalClause)
            {
                if (rule.TokenToVisitorMethodName.TryGetValue(terminalClause.Name, out var methodName))
                {
                    returnBuilder.AppendLine(@$"return _instance.{methodName}(arg0, arg1);");
                }
            }

            var content = _templateEngine.ApplyTemplate("PrefixVisitor", rule.Name,
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

    }
}
