

using ebnf.grammar;
using System;
using csly.ebnf.models;
using System.Collections.Generic;

    using csly.ebnf.models;

namespace ebnf.grammar
{

    internal class RuleParserVisitor
    {
        private readonly RuleParser _instance;

        public RuleParserVisitor(RuleParser instance)
        {
            _instance = instance;
        }

        public GrammarNode Visitrule(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.rule_0):
                    return Visitrule_0(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitrule_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = Visitclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[2]);

            return _instance.Root(arg0, arg1, arg2);
        }
        public GrammarNode Visitclauses(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.clauses_0):
                    return Visitclauses_0(node);
                case nameof(Visitors.clauses_1):
                    return Visitclauses_1(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitclauses_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = Visitclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);

            return _instance.Clauses(arg0, arg1);
        }
        public GrammarNode Visitclauses_1(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);

            return _instance.SingleClause(arg0);
        }
        public GrammarNode Visitclause(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.clause_0):
                    return Visitclause_0(node);
                case nameof(Visitors.clause_1):
                    return Visitclause_1(node);
                case nameof(Visitors.clause_2):
                    return Visitclause_2(node);
                case nameof(Visitors.clause_3):
                    return Visitclause_3(node);
                case nameof(Visitors.clause_4):
                    return Visitclause_4(node);
                case nameof(Visitors.clause_5):
                    return Visitclause_5(node);
                case nameof(Visitors.clause_6):
                    return Visitclause_6(node);
                case nameof(Visitors.clause_7):
                    return Visitclause_7(node);
                case nameof(Visitors.clause_8):
                    return Visitclause_8(node);
                case nameof(Visitors.clause_9):
                    return Visitclause_9(node);
                case nameof(Visitors.clause_10):
                    return Visitclause_10(node);
                case nameof(Visitors.clause_11):
                    return Visitclause_11(node);
                case nameof(Visitors.clause_12):
                    return Visitclause_12(node);
                case nameof(Visitors.clause_13):
                    return Visitclause_13(node);
                case nameof(Visitors.clause_14):
                    return Visitclause_14(node);
                case nameof(Visitors.clause_15):
                    return Visitclause_15(node);
                case nameof(Visitors.clause_16):
                    return Visitclause_16(node);
                case nameof(Visitors.clause_17):
                    return Visitclause_17(node);
                case nameof(Visitors.clause_18):
                    return Visitclause_18(node);
                case nameof(Visitors.clause_19):
                    return Visitclause_19(node);
                case nameof(Visitors.clause_20):
                    return Visitclause_20(node);
                case nameof(Visitors.clause_21):
                    return Visitclause_21(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitclause_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ZeroMoreClause(arg0, arg1);
        }
        public GrammarNode Visitclause_1(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.OneMoreClause(arg0, arg1);
        }
        public GrammarNode Visitclause_2(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg4 = (node.Children[4] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg5 = (node.Children[5] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.RepeatClauseMinMax(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        public GrammarNode Visitclause_3(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.RepeatClause(arg0, arg1, arg2, arg3);
        }
        public GrammarNode Visitclause_4(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.OptionClause(arg0, arg1);
        }
        public GrammarNode Visitclause_5(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.SimpleDiscardedClause(arg0, arg1);
        }
        public GrammarNode Visitclause_6(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.AlternateDiscardedClause(arg0, arg1);
        }
        public GrammarNode Visitclause_7(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);

            return _instance.AlternateClause(arg0);
        }
        public GrammarNode Visitclause_8(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.SimpleClause(arg0);
        }
        public GrammarNode Visitclause_9(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ExplicitTokenClause(arg0);
        }
        public GrammarNode Visitclause_10(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ExplicitTokenClauseDiscarded(arg0, arg1);
        }
        public GrammarNode Visitclause_11(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.Group(arg0, arg1, arg2);
        }
        public GrammarNode Visitclause_12(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoiceOneOrMore(arg0, arg1);
        }
        public GrammarNode Visitclause_13(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoiceZeroOrMore(arg0, arg1);
        }
        public GrammarNode Visitclause_14(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg4 = (node.Children[4] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg5 = (node.Children[5] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoiceRepeatRange(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        public GrammarNode Visitclause_15(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoiceRepeat(arg0, arg1, arg2, arg3);
        }
        public GrammarNode Visitclause_16(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoiceOptional(arg0, arg1);
        }
        public GrammarNode Visitclause_17(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupOneOrMore(arg0, arg1, arg2, arg3);
        }
        public GrammarNode Visitclause_18(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupZeroOrMore(arg0, arg1, arg2, arg3);
        }
        public GrammarNode Visitclause_19(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg4 = (node.Children[4] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg5 = (node.Children[5] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg6 = (node.Children[6] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg7 = (node.Children[7] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupRepeatRange(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public GrammarNode Visitclause_20(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg4 = (node.Children[4] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg5 = (node.Children[5] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupRepeat(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        public GrammarNode Visitclause_21(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg3 = (node.Children[3] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupOptional(arg0, arg1, arg2, arg3);
        }
        public GrammarNode Visitchoiceclause(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.choiceclause_0):
                    return Visitchoiceclause_0(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitchoiceclause_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = Visitchoices((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);
            var arg2 = (node.Children[2] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.AlternateChoices(arg0, arg1, arg2);
        }
        public GrammarNode Visitchoices(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.choices_0):
                    return Visitchoices_0(node);
                case nameof(Visitors.choices_1):
                    return Visitchoices_1(node);
                case nameof(Visitors.choices_2):
                    return Visitchoices_2(node);
                case nameof(Visitors.choices_3):
                    return Visitchoices_3(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitchoices_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoicesOne(arg0);
        }
        public GrammarNode Visitchoices_1(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.ChoicesString(arg0);
        }
        public GrammarNode Visitchoices_2(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = Visitchoices((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[2]);

            return _instance.ChoicesMany(arg0, arg1, arg2);
        }
        public GrammarNode Visitchoices_3(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg2 = Visitchoices((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[2]);

            return _instance.ChoicesManyExplicit(arg0, arg1, arg2);
        }
        public GrammarNode Visitgroupclauses(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.groupclauses_0):
                    return Visitgroupclauses_0(node);
                case nameof(Visitors.groupclauses_1):
                    return Visitgroupclauses_1(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitgroupclauses_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitgroupclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);
            var arg1 = Visitgroupclauses((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[1]);

            return _instance.GroupClauses(arg0, arg1);
        }
        public GrammarNode Visitgroupclauses_1(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitgroupclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);

            return _instance.GroupClausesOne(arg0);
        }
        public GrammarNode Visitgroupclause(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            switch (node.Visitor)
            {
                case nameof(Visitors.groupclause_0):
                    return Visitgroupclause_0(node);
                case nameof(Visitors.groupclause_1):
                    return Visitgroupclause_1(node);
                case nameof(Visitors.groupclause_2):
                    return Visitgroupclause_2(node);
                case nameof(Visitors.groupclause_3):
                    return Visitgroupclause_3(node);
                case nameof(Visitors.groupclause_4):
                    return Visitgroupclause_4(node);

                default:
                    throw new NotImplementedException($"Visitor {node.Visitor} not implemented");
            }
            return default(GrammarNode);
        }
        public GrammarNode Visitgroupclause_0(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupClause(arg0);
        }
        public GrammarNode Visitgroupclause_1(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupClauseExplicit(arg0);
        }
        public GrammarNode Visitgroupclause_2(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupClauseDiscarded(arg0, arg1);
        }
        public GrammarNode Visitgroupclause_3(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = (node.Children[0] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;
            var arg1 = (node.Children[1] as SyntaxLeaf<EbnfTokenGeneric, GrammarNode>).Token;

            return _instance.GroupClauseExplicitDiscarded(arg0, arg1);
        }
        public GrammarNode Visitgroupclause_4(SyntaxNode<EbnfTokenGeneric, GrammarNode> node)
        {
            var arg0 = Visitchoiceclause((SyntaxNode<EbnfTokenGeneric, GrammarNode>)node.Children[0]);

            return _instance.GroupChoiceClause(arg0);
        }
    }
}

