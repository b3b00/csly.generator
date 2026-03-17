using csly.ebnf.models;
using System;
using System.Collections.Generic;

namespace ebnf.grammar
{


    [ParserRoot("rule")]
    public class RuleParser
    {

        private readonly List<string> _tokens;

        public RuleParser(List<string> tokens)
        {
            _tokens = tokens;
        }

        #region rules grammar

        [Production("rule : IDENTIFIER COLON clauses")]
        public GrammarNode Root(Token<EbnfTokenGeneric> name, Token<EbnfTokenGeneric> discarded, GrammarNode clauses)
        {
            
            var rule = new Rule();
            rule.NonTerminalName = name.Value;
            rule.Clauses = (clauses as ClauseSequence).Clauses;
            rule.Clauses.ForEach(clause => clause.Parent = rule);
            return rule;
        }


        [Production("clauses : clause clauses")]
        public GrammarNode Clauses(GrammarNode clause, GrammarNode clauses)
        {
            var list = new ClauseSequence(clause as IClause);
            if (clauses != null) list.AddRange(clauses as ClauseSequence);
            list.Clauses.ForEach(x => x.Parent = list);
            return list;
        }

        [Production("clauses : clause ")]
        public GrammarNode SingleClause(GrammarNode clause)
        {
            return new ClauseSequence(clause as IClause);
        }


        [Production("clause : IDENTIFIER ZEROORMORE")]
        public GrammarNode ZeroMoreClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
        {
            var innerClause = BuildTerminalOrNonTerminal(id.Value, true);
            var zeroOrMore = new ZeroOrMoreClause(innerClause as IClause);
            innerClause.Parent = zeroOrMore;
            return zeroOrMore;
        }

        [Production("clause : IDENTIFIER ONEORMORE")]
        public GrammarNode OneMoreClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
        {
            var innerClause = BuildTerminalOrNonTerminal(id.Value);
            var oneOrMore = new  OneOrMoreClause(innerClause as IClause);
            innerClause.Parent = oneOrMore;
            return oneOrMore;
        }



        [Production("clause : IDENTIFIER LCURLY INT DASH INT RCURLY")]
        public GrammarNode RepeatClauseMinMax(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> lcurl, Token<EbnfTokenGeneric> min, Token<EbnfTokenGeneric> dash, Token<EbnfTokenGeneric> max, Token<EbnfTokenGeneric> rcurl)
        {
            var innerClause = BuildTerminalOrNonTerminal(id.Value);
            var repeat = new RepeatClause(innerClause as IClause, min.IntValue, max.IntValue);
            innerClause.Parent = repeat;
            return repeat;
        }

        [Production("clause : IDENTIFIER LCURLY INT RCURLY")]
        public GrammarNode RepeatClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> lcurl, Token<EbnfTokenGeneric> min, Token<EbnfTokenGeneric> rcurl)
        {
            var innerClause = BuildTerminalOrNonTerminal(id.Value);
            var repeat = new RepeatClause(innerClause as IClause, min.IntValue, min.IntValue);
            innerClause.Parent = repeat;
            return repeat;
        }

        [Production("clause : IDENTIFIER OPTION")]
        public GrammarNode OptionClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
        {
            var innerClause = BuildTerminalOrNonTerminal(id.Value);
            var option = new OptionClause(innerClause as IClause);
            innerClause.Parent = option;
            return option;
        }

        [Production("clause : IDENTIFIER DISCARD ")]
        public GrammarNode SimpleDiscardedClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discard)
        {
            var clause = BuildTerminalOrNonTerminal(id.Value, true);
            return clause;
        }

        [Production("clause : choiceclause DISCARD")]
        public GrammarNode AlternateDiscardedClause(GrammarNode choices, Token<EbnfTokenGeneric> discarded)
        {
            (choices as ChoiceClause).IsDiscarded = true;
            return choices;
        }

        [Production("clause : choiceclause")]
        public GrammarNode AlternateClause(GrammarNode choices)
        {
            (choices as ChoiceClause).IsDiscarded = false;
            return choices;
        }

        [Production("choiceclause : LCROG  choices RCROG  ")]
        public GrammarNode AlternateChoices(Token<EbnfTokenGeneric> discardleft, GrammarNode choices, Token<EbnfTokenGeneric> discardright)
        {
            return choices;
        }

        [Production("choices : IDENTIFIER  ")]
        public GrammarNode ChoicesOne(Token<EbnfTokenGeneric> head)
        {
            var innerClause = BuildTerminalOrNonTerminal(head.Value);
            var choice = new ChoiceClause(innerClause as IClause);
            innerClause.Parent = choice;
            return choice;
        }

        [Production("choices : STRING")]
        public GrammarNode ChoicesString(Token<EbnfTokenGeneric> head)
        {
            var innerClause = BuildTerminalOrNonTerminal(head.Value, discard: false);
            var choice = new ChoiceClause(innerClause as IClause);
            innerClause.Parent = choice;
            return choice;
        }

        [Production("choices : IDENTIFIER OR choices ")]
        public GrammarNode ChoicesMany(Token<EbnfTokenGeneric> head, Token<EbnfTokenGeneric> discardOr, GrammarNode tail)
        {
            var headClause = BuildTerminalOrNonTerminal(head.Value);
            var choice = new  ChoiceClause(headClause as IClause, (tail as ChoiceClause).Choices);
            choice.Choices.ForEach(x => x.Parent = choice);
            return choice;
        }

        [Production("choices : STRING OR choices ")]
        public GrammarNode ChoicesManyExplicit(Token<EbnfTokenGeneric> head, Token<EbnfTokenGeneric> discardOr, GrammarNode tail)
        {
            var headClause = BuildTerminalOrNonTerminal(head.Value, discard: false);
            var choice = new ChoiceClause(headClause as IClause, (tail as ChoiceClause).Choices);
            choice.Choices.ForEach(x => x.Parent = choice);
            return choice;
        }


        [Production("clause : IDENTIFIER ")]
        public GrammarNode SimpleClause(Token<EbnfTokenGeneric> id)
        {
            var clause = BuildTerminalOrNonTerminal(id.Value);
            return clause;
        }

        [Production("clause : STRING")]
        public GrammarNode ExplicitTokenClause(Token<EbnfTokenGeneric> explicitToken)
        {
            var clause = BuildTerminalOrNonTerminal(explicitToken.Value, discard: false);
            return clause;
        }

        [Production("clause : STRING DISCARD")]
        public GrammarNode ExplicitTokenClauseDiscarded(Token<EbnfTokenGeneric> explicitToken, Token<EbnfTokenGeneric> discard)
        {
            var clause = BuildTerminalOrNonTerminal(explicitToken.Value, discard: true);
            return clause;
        }


        #region  groups

        [Production("clause : LPAREN  groupclauses RPAREN ")]
        public GrammarNode Group(Token<EbnfTokenGeneric> discardLeft, GrammarNode clauses,
            Token<EbnfTokenGeneric> discardRight)
        {
            return clauses;
        }

        [Production("clause : choiceclause ONEORMORE ")]
        public GrammarNode ChoiceOneOrMore(GrammarNode choices, Token<EbnfTokenGeneric> discardOneOrMore)
        {
            var oneOrMore = new OneOrMoreClause(choices as IClause);
            oneOrMore.manyClause.Parent = oneOrMore;
            return oneOrMore;
        }

        [Production("clause : choiceclause ZEROORMORE ")]
        public GrammarNode ChoiceZeroOrMore(GrammarNode choices, Token<EbnfTokenGeneric> discardZeroOrMore)
        {
            var zeroOrMore = new ZeroOrMoreClause(choices as IClause);
            zeroOrMore.manyClause.Parent = zeroOrMore;
            return zeroOrMore;
        }

        [Production("clause : choiceclause LCURLY INT DASH INT RCURLY")]
        public GrammarNode ChoiceRepeatRange(GrammarNode choices, Token<EbnfTokenGeneric> dicardLcurl,
            Token<EbnfTokenGeneric> min, Token<EbnfTokenGeneric> discardDash, Token<EbnfTokenGeneric> max,
            Token<EbnfTokenGeneric> discardRcurl)
        {
            var repeat = new RepeatClause(choices as IClause, min.IntValue, max.IntValue);
            repeat.manyClause.Parent = repeat;
            return repeat;
        }

        [Production("clause : choiceclause LCURLY INT RCURLY")]
        public GrammarNode ChoiceRepeat(GrammarNode choices, Token<EbnfTokenGeneric> dicardLcurl,
            Token<EbnfTokenGeneric> min, Token<EbnfTokenGeneric> discardRcurl)
        {
            var repeat = new RepeatClause(choices as IClause, min.IntValue, min.IntValue);
            repeat.manyClause.Parent = repeat;
            return repeat;
        }


        [Production("clause : choiceclause OPTION ")]
        public GrammarNode ChoiceOptional(GrammarNode choices, Token<EbnfTokenGeneric> discardOption)
        {
            var option = new OptionClause(choices as IClause);
            option.Clause.Parent = option;
            return option;
        }

        [Production("clause : LPAREN  groupclauses RPAREN ONEORMORE ")]
        public GrammarNode GroupOneOrMore(Token<EbnfTokenGeneric> discardLeft, GrammarNode clauses,
            Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> oneZeroOrMore)
        {
            var oneOrMore = new OneOrMoreClause(clauses as IClause);
            oneOrMore.manyClause.Parent = oneOrMore;
            return oneOrMore;
        }

        [Production("clause : LPAREN  groupclauses RPAREN ZEROORMORE ")]
        public GrammarNode GroupZeroOrMore(Token<EbnfTokenGeneric> discardLeft, GrammarNode clauses,
            Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> discardZeroOrMore)
        {
            var zeroOrMore = new ZeroOrMoreClause(clauses as IClause);
            zeroOrMore.manyClause.Parent = zeroOrMore;
            return zeroOrMore;
        }

        [Production("clause : LPAREN  groupclauses RPAREN LCURLY INT DASH INT RCURLY")]
        public GrammarNode GroupRepeatRange(Token<EbnfTokenGeneric> discardLeft, GrammarNode clauses,
            Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> discardLcurl, Token<EbnfTokenGeneric> min,
            Token<EbnfTokenGeneric> discarddash, Token<EbnfTokenGeneric> max, Token<EbnfTokenGeneric> discardRcurl)
        {
            var repeat = new RepeatClause(clauses as IClause, min.IntValue, max.IntValue);
            repeat.manyClause.Parent = repeat;
            return repeat;
        }

        [Production("clause : LPAREN  groupclauses RPAREN LCURLY INT RCURLY")]
        public GrammarNode GroupRepeat(Token<EbnfTokenGeneric> discardLeft, GrammarNode clauses,
            Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> discardLcurl, Token<EbnfTokenGeneric> min,
            Token<EbnfTokenGeneric> discardRcurl)
        {
            var repeat = new RepeatClause(clauses as IClause, min.IntValue, min.IntValue);
            repeat.manyClause.Parent = repeat;
            return repeat;
        }


        [Production("clause : LPAREN  groupclauses RPAREN OPTION ")]
        public GrammarNode GroupOptional(Token<EbnfTokenGeneric> discardLeft, GrammarNode group,
            Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> _option)
        {
            var option = new OptionClause(group as IClause);
            option.Clause.Parent = option;
            return option;
        }


        [Production("groupclauses : groupclause groupclauses")]
        public GrammarNode GroupClauses(GrammarNode group, GrammarNode groups)
        {
            if (groups != null)
            {
                (group as GroupClause).AddRange(groups as GroupClause);
                (group as GroupClause).Clauses.ForEach(clause => clause.Parent = group);
            }
            return group;
        }

        [Production("groupclauses : groupclause")]
        public GrammarNode GroupClausesOne(GrammarNode group)
        {
            return group;
        }

        [Production("groupclause : IDENTIFIER ")]
        public GrammarNode GroupClause(Token<EbnfTokenGeneric> id)
        {
            var clause = BuildTerminalOrNonTerminal(id.Value);
            var group = new GroupClause(clause as IClause);
            group.Clauses.ForEach(x => x.Parent = group);
            return group;
        }

        [Production("groupclause : STRING ")]
        public GrammarNode GroupClauseExplicit(Token<EbnfTokenGeneric> explicitToken)
        {
            var clause = BuildTerminalOrNonTerminal(explicitToken.Value, discard: false);
            var group = new GroupClause(clause as IClause);
            group.Clauses.ForEach(x => x.Parent = group);
            return group;
        }




        [Production("groupclause : IDENTIFIER DISCARD ")]
        public GrammarNode GroupClauseDiscarded(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
        {
            var clause = BuildTerminalOrNonTerminal(id.Value, true);
            var group = new GroupClause(clause as IClause);
            group.Clauses.ForEach(x => x.Parent = group);
            return group;
        }

        [Production("groupclause : STRING DISCARD ")]
        public GrammarNode GroupClauseExplicitDiscarded(Token<EbnfTokenGeneric> explicitToken, Token<EbnfTokenGeneric> discarded)
        {
            var clause = BuildTerminalOrNonTerminal(explicitToken.Value, true);
            var group = new GroupClause(clause as IClause);
            group.Clauses.ForEach(x => x.Parent = group);
            return group;
        }

        [Production("groupclause : choiceclause ")]
        public GrammarNode GroupChoiceClause(GrammarNode choices)
        {
            var group = new GroupClause(choices as IClause);
            group.Clauses.ForEach(x => x.Parent = group);
            return group;
        }



        #endregion


        private GrammarNode BuildTerminalOrNonTerminal(string name, bool discard = false)
        {

            string token = null;
            IClause clause;
            var isTerminal = false;
            isTerminal = _tokens.Contains(name) || (name.StartsWith("'") && name.EndsWith("'"));



            if (isTerminal)
            {
                if (name.StartsWith("'") && name.EndsWith("'"))
                {
                    token = name.Trim('\'');
                    clause = new TerminalClause(Math.Abs(name.GetHashCode()).ToString(), token, discard);
                }
                else
                    clause = new TerminalClause(name, discard);
            }
            else
            {
                if (name.StartsWith("'"))
                {
                    clause = new TerminalClause(name.Substring(1, name.Length - 2), discard);
                }
                else
                {
                    switch (name)
                    {
                        case "INDENT":
                            clause = new IndentTerminalClause(IndentationType.Indent, discard);
                            break;
                        case "UINDENT":
                            clause = new IndentTerminalClause(IndentationType.UnIndent, discard);
                            break;
                        default:
                            clause = new NonTerminalClause(name);
                            break;
                    }
                }
            }

            return clause;
        }

        #endregion
    }
}