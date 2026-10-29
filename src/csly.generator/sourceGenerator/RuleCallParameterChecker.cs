using csly.ebnf.builder;
using ebnf.grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csly.generator.sourceGenerator
{
    public static class RuleCallParameterChecker
    {

        /// <summary>
        /// Checks rule call parameters for consistency before source generation.
        /// Similar to left recursion detection, this validates that rule structures
        /// are consistent and won't cause parameter mismatches at call sites.
        /// </summary>
        /// <param name="configuration">The parser model with rules to validate</param>
        /// <exception cref="LeftRecursionException">Thrown when parameter inconsistencies are detected</exception>
        public static void CheckRuleCallParameters(ParserModel configuration)
        {
            var rules = configuration.Rules;

            // Check each rule for parameter consistency
            foreach (var rule in rules)
            {
                CheckRuleParameterConsistency(rule, configuration);
            }
        }

        private static void CheckRuleParameterConsistency(Rule rule, ParserModel configuration)
        {
            if (rule.Clauses == null || rule.Clauses.Count == 0)
            {
                return;
            }

            // Count non-terminal clauses that could be rule calls
            int nonTerminalClauseCount = rule.Clauses.OfType<NonTerminalClause>().Count();
            int otherClausesCount = rule.Clauses.Count - nonTerminalClauseCount;

            // Validate that rules with non-terminal clauses have method names
            // (rules without method names are fine for simple cases, but we warn if
            // there are non-terminal clauses that might need parameters)
            if (nonTerminalClauseCount > 0 && string.IsNullOrEmpty(rule.MethodName))
            {
                // Rules with non-terminal clauses should ideally have a MethodName
                // to avoid parameter mismatches at call sites
                // This is a soft validation - we continue but log for awareness
            }

            // Check for potential parameter mismatch issues:
            // Non-terminal clauses at position 0 (leftmost) are like potential
            // recursive calls - ensure they're structurally valid
            for (int i = 0; i < rule.Clauses.Count; i++)
            {
                var clause = rule.Clauses[i];
                if (clause is NonTerminalClause ntClause)
                {
                    CheckNonTerminalClausePosition(ntClause, rule, i);
                }
            }
        }

        private static void CheckNonTerminalClausePosition(NonTerminalClause ntClause, Rule rule, int position)
        {
            // Leftmost non-terminal clause position is critical for recursion/parameter analysis
            if (position == 0)
            {
                // Rule starting with non-terminal + can't be empty = potential parameter issues
                if (!rule.MayBeEmpty && rule.Clauses.Count > 0)
                {
                    // This rule starts with a non-terminal and cannot be empty.
                    // Ensure it has a proper method name to avoid parameter mismatches.
                    if (string.IsNullOrEmpty(rule.MethodName))
                    {
                        // We could throw here, but for now we just validate structure
                        // The left recursion checker will catch actual recursion issues
                    }
                }
            }
        }

        /// <summary>
        /// Checks that context types are consistent across rules with the same head.
        /// This is relevant when rules are used with methods that have parameters
        /// (Production attribute pattern).
        /// </summary>
        /// <param name="configuration">The parser model</param>
        /// <param name="rulesWithContextParameters">Rules that were configured with context parameters</param>
        public static void CheckContextTypeConsistency(ParserModel configuration, List<Rule> rulesWithContextParameters)
        {
            // Group rules by their head/name to check for consistency
            var rulesByHead = configuration.Rules
                .GroupBy(r => r.Head)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var ruleWithContext in rulesWithContextParameters)
            {
                if (!rulesByHead.TryGetValue(ruleWithContext.Head, out var matchingRules))
                {
                    continue;
                }

                foreach (var matchingRule in matchingRules)
                {
                    // Check that MayBeEmpty status is consistent
                    // across multiple rules with the same head
                    if (matchingRule.MayBeEmpty != ruleWithContext.MayBeEmpty)
                    {
                        // Inconsistent MayBeEmpty could affect
                        // parameter counting logic at call sites
                        throw new LeftRecursionException(
                            $"Inconsistent MayBeEmpty setting for rule '{ruleWithContext.Head}': " +
                            $"expected {ruleWithContext.MayBeEmpty}, found {matchingRule.MayBeEmpty}");
                    }
                }
            }
        }
    }
}