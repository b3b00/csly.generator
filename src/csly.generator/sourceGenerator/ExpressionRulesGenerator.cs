using csly.ebnf.builder;
using ebnf.grammar;
using System;
using System.Collections.Generic;

using System.Linq;


namespace csly.generator.sourceGenerator
{
    internal class ExpressionRulesGenerator
    {

        public ExpressionRulesGenerator()
        {
        }

        public void Generate(ParserModel model)
        {
            if (model.Operations == null || model.Operations.Count == 0)
            {
                return;
            }
            var operationsByPrecedence = model.Operations.GroupBy(x => x.Precedence).ToDictionary(x => x.Key, x => x.ToList());

            Dictionary<int, string> ruleNameForPrecedence = new Dictionary<int, string>();

            Dictionary<int, string> ruleNameForLowerPrecedence = new Dictionary<int, string>();


            var precedences = operationsByPrecedence.Keys.OrderBy(x => x).ToList();

            List<Rule> expressionRules = new List<Rule>();

            for (int i = 0; i < precedences.Count; i++)
            {
                int precedence = precedences[i];
                string ruleName = $"Expr_Prec_{precedence}";
                string lowerPrecedenceRuleName;

                if (i < precedences.Count - 1)
                {
                    lowerPrecedenceRuleName = $"Expr_Prec_{precedences[i + 1]}";
                }
                else
                {
                    lowerPrecedenceRuleName = "Expr_Operand";
                }
                var rule = GenerateRuleForPrecedence(precedence, operationsByPrecedence[precedence], ruleName, lowerPrecedenceRuleName, i == precedences.Count - 1);
                expressionRules.AddRange(rule);
            }
           
                var operandRule = GenerateRuleForOperands(model.Operands);
                if (operandRule != null)
                {
                    expressionRules.Add(operandRule);
                }

            if (precedences.Any())
            {
                var rootRule = new Rule($"{model.ParserName}_expressions", new List<IClause>() { new NonTerminalClause($"Expr_Prec_{precedences[0]}") }, "")
                {
                    IsByPassRule = true
                };
                expressionRules.Add(rootRule);
            }
            model.Rules.AddRange(expressionRules);
        }

        public List<Rule> GenerateRuleForPrecedence(int precedence, List<Operation> operations, string ruleName, string lowerPrecedenceRuleName, bool isLowest)
        {
            var affix = operations[0].Affix;
            var rules = new List<Rule>();
            var rule = new Rule()
            {
                NonTerminalName = ruleName,
                IsExpressionRule = true,
                ExpressionAffix = affix,
                Precedence = precedence,
                Associativity = operations[0].Associativity,
                IsInfixExpressionRule = affix == Affix.InFix,
                IsByPassRule = true
            };

            foreach (var operation in operations)
            {
                rule.SetVisitorMethodName(operation.TokenName, operation.MethodName);
            }


            

            switch (affix)
            {
                case Affix.InFix:
                    {
                        var left = new NonTerminalClause(lowerPrecedenceRuleName);
                        var right = new NonTerminalClause(ruleName);
                        IClause operatorClause = null;
                        if (operations.Count == 1)
                        {
                            operatorClause = new TerminalClause(operations[0].TokenName);
                        }
                        else
                        {
                            List<IClause> choices = new List<IClause>();
                            choices = operations.Select(x => new TerminalClause(x.TokenName)).Cast<IClause>().ToList();
                            operatorClause = new ChoiceClause(choices);
                        }
                        rule.Clauses = new List<IClause>
                            {
                                left,
                                operatorClause,
                                right
                            };
                        rule.OperatorVisitors = operations.ToDictionary(x => x.TokenName, x => x.MethodName);
                        rules.Add(rule);
                        break;
                    }
                case Affix.PreFix:
                    {
                        var operand = new NonTerminalClause(lowerPrecedenceRuleName);
                        IClause operatorClause = null;
                        if (operations.Count == 1)
                        {
                            operatorClause = new TerminalClause(operations[0].TokenName);
                        }
                        else
                        {
                            List<IClause> choices = new List<IClause>();
                            choices = operations.Select(x => new TerminalClause(x.TokenName)).Cast<IClause>().ToList();
                            operatorClause = new ChoiceClause(choices);
                        }
                        rule.Clauses = new List<IClause>
                            {
                                operatorClause,
                                operand
                            };
                        rule.IsByPassRule = false;
                        rules.Add(rule);

                        var byPassRule = new Rule()
                        {
                            NonTerminalName = ruleName,
                            IsByPassRule = true,
                            IsExpressionRule = true,
                            ExpressionAffix = affix,
                            Precedence = precedence,
                            Associativity = operations[0].Associativity,
                            IsInfixExpressionRule = affix == Affix.InFix,
                        };
                        byPassRule.Clauses = new List<IClause>
                            {
                                operand
                            };
                        rules.Add(byPassRule);
                        break;
                    }
                case Affix.PostFix:
                    {
                        var operand = new NonTerminalClause(lowerPrecedenceRuleName);
                        IClause operatorClause = null;
                        if (operations.Count == 1)
                        {
                            operatorClause = new TerminalClause(operations[0].TokenName);
                        }
                        else
                        {
                            List<IClause> choices = new List<IClause>();
                            choices = operations.Select(x => new TerminalClause(x.TokenName)).Cast<IClause>().ToList();
                            operatorClause = new ChoiceClause(choices);
                        }
                        rule.Clauses = new List<IClause>
                            {
                                operand,
                                operatorClause
                            };
                        rules.Add(rule);
                        break;
                    }
            }
            return rules;
        }

        public Rule GenerateRuleForOperands(List<Operand> operands)
        {
            if (operands.Count == 1)
            {
                Rule singleOperandRule = new Rule()
                {
                    NonTerminalName = "Expr_Operand",
                    IsExpressionRule = false,
                    IsByPassRule = true
                };
                singleOperandRule.Clauses.Add(new NonTerminalClause(operands[0].Rule.NonTerminalName));

                return singleOperandRule;
            }

            Rule rule = new Rule()
            {
                NonTerminalName = "Expr_Operand",
                IsExpressionRule = false, 
                IsByPassRule = true, 
            };
            rule.Clauses.Add(
                new ChoiceClause(
                    operands.Select(x => (IClause)new NonTerminalClause(x.Rule.NonTerminalName)).ToList()
                )
            );

            return rule;
        }
    }
}
