using ebnf.grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csly.ebnf.builder
{

    public class ParserOPtions
    {
        public ParserOPtions()
        {
        }

        public string StartingNonTerminal { get; set; }
        public bool AutoCloseIndentations { get; set; }
        public bool UseMemoization { get; set; }
        public bool BroadenTokenWindow { get; set; }
    }

    public class Operand
    {
        string MethodName => _rule.MethodName;

        private readonly Rule _rule;

        public Rule Rule => _rule;

        public Operand(Rule rule)
        {
            _rule = rule;            
        }
    }

    public class Operation
    {
        public string MethodName { get; set; }

        public Affix Affix { get; set; }

        public Associativity Associativity { get; set; }

        public string TokenName { get; set; }

        public int Precedence { get; set; }

        public Operation(string methodName, Affix affix, Associativity associativity, string tokenName, int precedence)
        {
            MethodName = methodName;
            Affix = affix;
            Associativity = associativity;
            TokenName = tokenName;
            Precedence = precedence;
        }
    }

    public class ParserModel {
        private readonly List<Rule> _rules  = new List<Rule>();
        private readonly List<Operation> _operations  = new List<Operation>();

        private readonly List<Operand> _operands  = new List<Operand>();

        public List<Rule> Rules => _rules;

        public List<Operation> Operations => _operations;

        public List<Operand> Operands => _operands;

        public void AddRule(Rule rule)
        {
            _rules.Add(rule);
        }

        public void AddOperation(Operation operation)
        {
            _operations.Add(operation);
        }

        public void AddOperand(Operand operand)
        {
            _operands.Add(operand);
        }

    }

    public class StaticParserBuilder
    {
        public List<string> _tokens { get; set; }

        public ParserModel Model { get; set; } = new ParserModel();

        public ParserOPtions ParserOPtions { get; set; }

        private readonly RuleParserMain _parserMain;

        private readonly RuleParser _ruleParser;


        public StaticParserBuilder(List<string> tokens)
        {
            _tokens = tokens;
            ParserOPtions = new ParserOPtions();
            _ruleParser = new RuleParser(_tokens);
            _parserMain = new RuleParserMain(_ruleParser);
        }


        public Rule Parse(string ruleString, string methodName)
        {
            if (ruleString != null)
            {                
                ruleString = ruleString.Substring(1, ruleString.Length - 2);

                var parsed = _parserMain.Parse(ruleString);                
                var rule = parsed.Result as Rule;
                rule.MethodName = methodName;
                return rule;
            }
            return null;
        }

        public void ComputeLeaders()
        {
            Dictionary<string, List<string>> nonTerminalLeaders = new Dictionary<string, List<string>>();

            List<string> startingPoints;
            if (string.IsNullOrEmpty(ParserOPtions.StartingNonTerminal))
            {
                startingPoints = FindStartingPoints();
            }
            else
            {
                startingPoints = new List<string> { ParserOPtions.StartingNonTerminal };
            }
            foreach (var sp in startingPoints)
            {
                ComputeLeadersForNonTerminal(sp, nonTerminalLeaders);
            }
            Model.Rules.Where(x => !startingPoints.Contains(x.Head)) .ToList().ForEach(r => ComputeLeadersForNonTerminal(r.Head, nonTerminalLeaders));


        }

        private void ComputeLeadersForNonTerminal(string nonTerminal, Dictionary<string, List<string>> leadersForNTs)
        {

            if (leadersForNTs.ContainsKey(nonTerminal))
            {
                return;
            }
            var rules = Model.Rules.Where(r => r.Head == nonTerminal).ToList();



            foreach (var rule in rules)
            {
                ComputeLeaderForRule(rule, leadersForNTs);
            }
            leadersForNTs[nonTerminal] = rules.SelectMany(r => r.Leaders).Distinct().ToList();


        }


        private void ComputeLeaderForRule(Rule rule, Dictionary<string, List<string>> leadersForNTs)
        {
            int i = 0;
            var first = rule.Clauses[0];
            do
            {
                first = rule.Clauses[i];
                if (first is TerminalClause term)
                {
                    rule.Leaders.Add(term.Name);
                    if (leadersForNTs.TryGetValue(rule.Head, out var existingLeaders))
                    {
                        if (!existingLeaders.Contains(term.Name))
                        {
                            existingLeaders.Add(term.Name);
                        }
                    }
                    else
                    {
                        leadersForNTs[rule.Head] = new List<string> { term.Name };
                    }
                }
                else if (first is NonTerminalClause nt)
                {
                    // get leaders for nt
                    if (leadersForNTs.ContainsKey(nt.Name))
                    {
                        var ntLeaders = leadersForNTs[nt.Name];
                        rule.Leaders.AddRange(ntLeaders);
                    }
                    else
                    {
                        ComputeLeadersForNonTerminal(nt.Name, leadersForNTs);
                        if (leadersForNTs.ContainsKey(nt.Name))
                        {
                            var ntLeaders = leadersForNTs[nt.Name];
                            rule.Leaders.AddRange(ntLeaders);
                        }
                    }
                }
                else if (first is ManyClause manyClause)
                {
                    var innerFirst = manyClause.manyClause;
                    if (innerFirst is TerminalClause termInner)
                    {
                        rule.Leaders.Add(termInner.Name);
                        if (leadersForNTs.TryGetValue(rule.Head, out var existingLeaders))
                        {
                            if (!existingLeaders.Contains(termInner.Name))
                            {
                                existingLeaders.Add(termInner.Name);
                            }
                        }
                        else
                        {
                            leadersForNTs[rule.Head] = new List<string> { termInner.Name };
                        }
                    }
                    else if (innerFirst is NonTerminalClause ntInner)
                    {
                        // get leaders for nt
                        if (leadersForNTs.ContainsKey(ntInner.Name))
                        {
                            var ntLeaders = leadersForNTs[ntInner.Name];
                            rule.Leaders.AddRange(ntLeaders);
                        }
                        else
                        {
                            ComputeLeadersForNonTerminal(ntInner.Name, leadersForNTs);
                            if (leadersForNTs.ContainsKey(ntInner.Name))
                            {
                                var ntLeaders = leadersForNTs[ntInner.Name];
                                rule.Leaders.AddRange(ntLeaders);
                            }
                        }
                    }
                }
                else if (first is OptionClause optionClause)
                {
                    var inner = optionClause.Clause;
                    if (inner is TerminalClause termInner)
                    {
                        rule.Leaders.Add(termInner.Name);
                        if (leadersForNTs.TryGetValue(rule.Head, out var existingLeaders))
                        {
                            if (!existingLeaders.Contains(termInner.Name))
                            {
                                existingLeaders.Add(termInner.Name);
                            }
                        }
                        else
                        {
                            leadersForNTs[rule.Head] = new List<string> { termInner.Name };
                        }
                    }
                    else if (inner is NonTerminalClause ntInner)
                    {
                        // get leaders for nt
                        if (leadersForNTs.ContainsKey(ntInner.Name))
                        {
                            var ntLeaders = leadersForNTs[ntInner.Name];
                            rule.Leaders.AddRange(ntLeaders);
                        }
                        else
                        {
                            ComputeLeadersForNonTerminal(ntInner.Name, leadersForNTs);
                            if (leadersForNTs.ContainsKey(ntInner.Name))
                            {
                                var ntLeaders = leadersForNTs[ntInner.Name];
                                rule.Leaders.AddRange(ntLeaders);
                            }
                        }
                    }
                }
                else if (first is ChoiceClause choiceClause)
                {
                    foreach (var choice in choiceClause.Choices)
                    {
                        if (choice is TerminalClause termChoice)
                        {
                            rule.Leaders.Add(termChoice.Name);
                            if (leadersForNTs.TryGetValue(rule.Head, out var existingLeaders))
                            {
                                if (!existingLeaders.Contains(termChoice.Name))
                                {
                                    existingLeaders.Add(termChoice.Name);
                                }
                            }
                            else
                            {
                                leadersForNTs[rule.Head] = new List<string> { termChoice.Name };
                            }
                        }
                        else if (choice is NonTerminalClause ntChoice)
                        {
                            // get leaders for nt
                            if (leadersForNTs.ContainsKey(ntChoice.Name))
                            {
                                var ntLeaders = leadersForNTs[ntChoice.Name];
                                rule.Leaders.AddRange(ntLeaders);
                            }
                            else
                            {
                                ComputeLeadersForNonTerminal(ntChoice.Name, leadersForNTs);
                                if (leadersForNTs.ContainsKey(ntChoice.Name))
                                {
                                    var ntLeaders = leadersForNTs[ntChoice.Name];
                                    rule.Leaders.AddRange(ntLeaders);
                                }
                            }
                        }
                    }                 
                }
                i++;
            }
            while (first.MayBeEmpty() && i < rule.Clauses.Count);
        }

        private List<string> FindStartingPoints()
        {
            var nonTerminals = Model.Rules.Select(r => r.Head).Distinct().ToList();
            // nt from NonTerminals such that
            // ! exist rule from Model such that rule.Clauses contains nt
            return nonTerminals.Where(nt =>
                !Model.Rules.Any(rule => rule.Clauses.OfType<NonTerminalClause>().Any(c => c.Name == nt)))
                .ToList();
        }
    }
}

