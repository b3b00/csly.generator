using System;
using System.Collections.Generic;
using System.Linq;
using csly.ebnf.builder;
using ebnf.grammar;

namespace csly.generator.sourceGenerator
{
    public class LeftRecursionChecker
    {

        private static List<string> BuildPath(List<string> current, string step)
        {
            var newPath = new List<string>();
            newPath.AddRange(current);
            newPath.Add(step);
            return newPath;
        }

        public static List<string> Lst(params string[] args)
        {
            return args.ToList<string>();
        }

        public static List<string> GetLeftClausesName(IClause clause)
        {
            switch (clause)
            {
                case NonTerminalClause nonTerminal:
                    return Lst(nonTerminal.Name);
                case ManyClause many:
                    return GetLeftClausesName(many.manyClause);
                case OptionClause option:
                    return GetLeftClausesName(option.Clause);
                case ChoiceClause choice when choice.IsNonTerminalChoice:
                    return choice.Choices.SelectMany<IClause, string>(x => GetLeftClausesName(x)).ToList<string>();
                case GroupClause group:
                    return GetLeftClausesName(group.Clauses.First<IClause>());
                default:
                    return new List<string>();
            }
        }

        public static List<string> GetLeftClausesName(Rule rule,
            IReadOnlyDictionary<string, List<Rule>> nonTerminals)
        {
            List<string> lefts = new List<string>();

            if (rule.Clauses == null || rule.Clauses.Count == 0)
            {
                return lefts;
            }

            int i = 0;
            IClause current = rule.Clauses[0];
            var currentLefts = GetLeftClausesName(current);
            bool stopped = false;
            while (i < rule.Clauses.Count && ! stopped && currentLefts != null && currentLefts.Any<string>())
            {
                stopped = !current.MayBeEmpty();
                lefts.AddRange(currentLefts);
                stopped = !current.MayBeEmpty();
                i++;
                if (i < rule.Clauses.Count)
                {
                    currentLefts = GetLeftClausesName(rule.Clauses[i]);
                }
                else
                {
                    current = null;
                    currentLefts = null;
                }
            }

            return lefts;

        }


        public static (bool foundRecursion, List<List<string>> recursions) CheckLeftRecursion(ParserModel configuration)
        {
            var nonTerminals = configuration.Rules
                .GroupBy(r => r.Head)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<List<string>> recursions = new List<List<string>>();
            bool foundRecursion = false;
            foreach (var nonTerminal in nonTerminals)
            {
                var (found, recursion) = CheckLeftRecursion(nonTerminals, nonTerminal.Key,
                    new List<string> { nonTerminal.Key });
                if (found)
                {
                    foundRecursion = true;
                    recursions.AddRange(recursion);
                }
            }


            return (foundRecursion, recursions);
        }

        public static (bool recursionFound, List<List<string>> recursion) CheckLeftRecursion(
            IReadOnlyDictionary<string, List<Rule>> nonTerminals,
            string nonTerminalName, List<string> currentPath)
        {
            var foundRecursion = false;
            List<List<string>> recursions = new List<List<string>>();

            var (found, path) = FindRecursion(currentPath);
            if (found)
            {
                return (true, new List<List<string>> { currentPath });
            }

            var rules = nonTerminals.TryGetValue(nonTerminalName, out var ntRules) ? ntRules : new List<Rule>();
            var leftClauses = rules.SelectMany<Rule, string>(x => GetLeftClausesName(x, nonTerminals)).ToList<string>();

            foreach (var leftClause in leftClauses)
            {
                if (nonTerminals.TryGetValue(leftClause, out var newNonTerminal) && newNonTerminal != null)
                {
                    var nPath = BuildPath(currentPath, leftClause);
                    var (foundRRuleRecursion, recursion) = CheckLeftRecursion(nonTerminals, leftClause, nPath);
                    if (!foundRRuleRecursion)
                    {
                        continue;
                    }
                    foundRecursion = true;
                    recursions.AddRange(recursion);

                }
            }

            return (foundRecursion, recursions);


        }

        private static (bool, string) FindRecursion(List<string> path)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                string step = path[i];
                int next = path.LastIndexOf(step);
                if (next > i)
                {
                    string failure = string.Join(" > ", path.GetRange(i, next - i + 1));
                    return (true, failure);
                }
            }

            return (false, null);
        }


    }

    [Serializable]
    public class LeftRecursionException : Exception
    {
        public LeftRecursionException(string message) : base(message) { }
    }
}
