using System;
using System.Collections.Generic;
using System.Linq;

namespace csly.generator.model.parser.grammar;

public class ParserOPtions {
    public ParserOPtions()
    {        
    }

    public string StartingNonTerminal { get; set; }
    public bool AutoCloseIndentations { get; set; }
    public bool UseMemoization { get; set; }
    public bool BroadenTokenWindow { get; set; }
}

    public class StaticParserBuilder
{
    public List<string> _tokens { get; set; }

    public List<Rule> Model { get; set; } = new List<Rule>();

    public ParserOPtions ParserOPtions { get; set; }

    public StaticParserBuilder(List<string> tokens)
    {
        _tokens = tokens;
        ParserOPtions = new ParserOPtions();
        
    }


    public Rule Parse(string ruleString)
    {
            (string head, string[] clauses) result = (null,null);
            if (ruleString != null)
            {
                ruleString = ruleString.Substring(1, ruleString.Length - 2);
                var i = ruleString.IndexOf(":", StringComparison.Ordinal);
                if (i <= 0)
                {
                    return null;
                }

                var head = ruleString.Substring(0, i).Trim();
                var clauseStrings = ruleString.Substring(i + 1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var clauses = clauseStrings.Select(
                    x => _tokens.Contains(x) ? 
                        (IClause)new TerminalClause(x) :
                        (IClause)new NonTerminalClause(x)).ToList();
                var rule = new Rule(head, clauses);
                Model.Add(rule);
                return rule;
            }
        return null;
    }

    internal void ComputeLeaders()
    {
        Dictionary<string,List<string>> nonTerminalLeaders = new Dictionary<string, List<string>>();

        List<string> startingPoints;
        if (string.IsNullOrEmpty(ParserOPtions.StartingNonTerminal)) {
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
    }

    private void ComputeLeadersForNonTerminal(string nonTerminal, Dictionary<string, List<string>> leadersForNTs)
    {
     
        if (leadersForNTs.ContainsKey(nonTerminal))
        {
            return;
        }
        var rules = Model.Where(r => r.Head == nonTerminal).ToList();



        foreach (var rule in rules)
        {
            ComputeLeaderForRule(rule, leadersForNTs);
        }



    }


        private void ComputeLeaderForRule(Rule rule, Dictionary<string, List<string>> leadersForNTs)
        {
           
        var first = rule.Clauses[0];
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
    }
    
    private List<string> FindStartingPoints()
    {
        var nonTerminals = Model.Select(r => r.Head).Distinct().ToList();
        // nt from NonTerminals such that
        // ! exist rule from Model such that rule.Clauses contains nt
        return nonTerminals.Where(nt =>
            !Model.Any(rule => rule.Clauses.OfType<NonTerminalClause>().Any(c => c.Name == nt)))
            .ToList();
    }

}

