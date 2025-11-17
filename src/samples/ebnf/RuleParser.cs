using csly.models;
using System;

namespace ebnf.grammar;


[ParserRoot("rule")]
 public class RuleParser 
{
    #region rules grammar

    [Production("rule : IDENTIFIER COLON clauses")]
    public GrammarNode Root(Token<EbnfTokenGeneric> name, Token<EbnfTokenGeneric> discarded, ClauseSequence clauses)
    {
        var rule = new Rule();
        rule.NonTerminalName = name.Value;
        rule.Clauses = clauses.Clauses;
        return rule;
    }


    [Production("clauses : clause clauses")]
    public GrammarNode Clauses(IClause clause, ClauseSequence clauses)
    {
        var list = new ClauseSequence(clause);
        if (clauses != null) list.AddRange(clauses);
        return list;
    }

    [Production("clauses : clause ")]
    public ClauseSequence SingleClause(IClause clause)
    {
        return new ClauseSequence(clause);
    }


    [Production("clause : IDENTIFIER ZEROORMORE")]
    public IClause ZeroMoreClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
    {
        var innerClause = BuildTerminalOrNonTerimal(id.Value, true);
        return new ZeroOrMoreClause(innerClause);
    }

    [Production("clause : IDENTIFIER ONEORMORE")]
    public IClause OneMoreClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
    {
        var innerClause = BuildTerminalOrNonTerimal(id.Value);
        return new OneOrMoreClause(innerClause);
    }
    
   
    
    [Production("clause : IDENTIFIER LCURLY INT DASH INT RCURLY")]
    public IClause RepeatClauseMinMax(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> lcurl, Token<EbnfTokenGeneric> min, Token<EbnfTokenGeneric> dash, Token<EbnfTokenGeneric> max, Token<EbnfTokenGeneric> rcurl)
    {
        var innerClause = BuildTerminalOrNonTerimal(id.Value);
        return new RepeatClause(innerClause, min.IntValue, max.IntValue);
    }
    
    [Production("clause : IDENTIFIER LCURLY INT RCURLY")]
    public IClause RepeatClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> lcurl, Token<EbnfTokenGeneric> min,  Token<EbnfTokenGeneric> rcurl)
    {
        var innerClause = BuildTerminalOrNonTerimal(id.Value);
        return new RepeatClause(innerClause, min.IntValue, min.IntValue);
    }

    [Production("clause : IDENTIFIER OPTION")]
    public IClause OptionClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
    {
        var innerClause = BuildTerminalOrNonTerimal(id.Value);
        return new OptionClause(innerClause);
    }

    [Production("clause : IDENTIFIER DISCARD ")]
    public IClause SimpleDiscardedClause(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discard)
    {
        var clause = BuildTerminalOrNonTerimal(id.Value, true);
        return clause;
    }

    [Production("clause : choiceclause DISCARD")]
    public IClause AlternateDiscardedClause(ChoiceClause choices, Token<EbnfTokenGeneric> discarded)
    {
        choices.IsDiscarded = true;
        return choices;
    }
    
    [Production("clause : choiceclause")]
    public IClause AlternateClause(ChoiceClause choices)
    {
        choices.IsDiscarded = false;
        return choices;
    }

    [Production("choiceclause : LCROG  choices RCROG  ")]
    public IClause AlternateChoices(Token<EbnfTokenGeneric> discardleft, IClause choices, Token<EbnfTokenGeneric> discardright)
    {            
        return choices;
    }
    
    [Production("choices : IDENTIFIER  ")]
    public IClause ChoicesOne(Token<EbnfTokenGeneric> head)
    {
        var choice = BuildTerminalOrNonTerimal(head.Value);
        return new ChoiceClause(choice);
    }
    
    [Production("choices : STRING")]
    public IClause ChoicesString(Token<EbnfTokenGeneric> head)
    {
        var choice = BuildTerminalOrNonTerimal(head.Value, discard: false);
        return new ChoiceClause(choice);
    }
    
    [Production("choices : IDENTIFIER OR choices ")]
    public IClause ChoicesMany(Token<EbnfTokenGeneric> head, Token<EbnfTokenGeneric> discardOr, ChoiceClause tail)
    {
        var headClause = BuildTerminalOrNonTerimal(head.Value); 
        return new ChoiceClause(headClause,tail.Choices);
    }
    
    [Production("choices : STRING OR choices ")]
    public IClause ChoicesManyExplicit(Token<EbnfTokenGeneric> head, Token<EbnfTokenGeneric> discardOr, ChoiceClause tail)
    {
        var headClause = BuildTerminalOrNonTerimal(head.Value,discard:false); 
        return new ChoiceClause(headClause,tail.Choices);
    }
    

    [Production("clause : IDENTIFIER ")]
    public IClause SimpleClause(Token<EbnfTokenGeneric> id)
    {
        var clause = BuildTerminalOrNonTerimal(id.Value);
        return clause;
    }

    [Production("clause : STRING")]
    public IClause ExplicitTokenClause(Token<EbnfTokenGeneric> explicitToken) {
        var clause = BuildTerminalOrNonTerimal(explicitToken.Value,discard:false);
        return clause;
    }
    
    [Production("clause : STRING DISCARD")]
    public IClause ExplicitTokenClauseDiscarded(Token<EbnfTokenGeneric> explicitToken, Token<EbnfTokenGeneric> discard) {
        var clause = BuildTerminalOrNonTerimal(explicitToken.Value,discard:true);
        return clause;
    }


    #region  groups

    [Production("clause : LPAREN  groupclauses RPAREN ")]
    public GroupClause Group(Token<EbnfTokenGeneric> discardLeft, GroupClause clauses,
        Token<EbnfTokenGeneric> discardRight)
    {
        return clauses;
    }
    
    [Production("clause : choiceclause ONEORMORE ")]
    public IClause ChoiceOneOrMore(ChoiceClause choices,Token<EbnfTokenGeneric> discardOneOrMore)
    {
        return new OneOrMoreClause(choices);
    }

    [Production("clause : choiceclause ZEROORMORE ")]
    public IClause ChoiceZeroOrMore(ChoiceClause choices,Token<EbnfTokenGeneric> discardZeroOrMore)
    {
        return new ZeroOrMoreClause(choices);
    }
    
    [Production("clause : choiceclause LCURLY INT DASH INT RCURLY")]
    public IClause ChoiceRepeatRange(ChoiceClause choices,Token<EbnfTokenGeneric> dicardLcurl,
        Token<EbnfTokenGeneric> min, Token<EbnfTokenGeneric> discardDash, Token<EbnfTokenGeneric> max, 
        Token<EbnfTokenGeneric> discardRcurl)
    {
        return new RepeatClause(choices, min.IntValue, max.IntValue);
    }
    
    [Production("clause : choiceclause LCURLY INT RCURLY")]
    public IClause ChoiceRepeat(ChoiceClause choices,Token<EbnfTokenGeneric> dicardLcurl,
        Token<EbnfTokenGeneric> min,   Token<EbnfTokenGeneric> discardRcurl)
    {
        return new RepeatClause(choices, min.IntValue, min.IntValue);
    }
    

    [Production("clause : choiceclause OPTION ")]
    public IClause ChoiceOptional(ChoiceClause choices,Token<EbnfTokenGeneric> discardOption)
    {
        return new OptionClause(choices);
    }

    [Production("clause : LPAREN  groupclauses RPAREN ONEORMORE ")]
    public IClause GroupOneOrMore(Token<EbnfTokenGeneric> discardLeft, GroupClause clauses,
        Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> oneZeroOrMore)
    {
        return new OneOrMoreClause(clauses);
    }

    [Production("clause : LPAREN  groupclauses RPAREN ZEROORMORE ")]
    public IClause GroupZeroOrMore(Token<EbnfTokenGeneric> discardLeft, GroupClause clauses,
        Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> discardZeroOrMore)
    {
        return new ZeroOrMoreClause(clauses);
    }

    [Production("clause : LPAREN  groupclauses RPAREN LCURLY INT DASH INT RCURLY")]
    public IClause GroupRepeatRange(Token<EbnfTokenGeneric> discardLeft, GroupClause clauses,
        Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> discardLcurl, Token<EbnfTokenGeneric> min,
        Token<EbnfTokenGeneric> discarddash, Token<EbnfTokenGeneric> max, Token<EbnfTokenGeneric> discardRcurl)
    {
        return new RepeatClause(clauses, min.IntValue, max.IntValue);
    }
    
    [Production("clause : LPAREN  groupclauses RPAREN LCURLY INT RCURLY")]
    public IClause GroupRepeat(Token<EbnfTokenGeneric> discardLeft, GroupClause clauses,
        Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> discardLcurl, Token<EbnfTokenGeneric> min,
        Token<EbnfTokenGeneric> discardRcurl)
    {
        return new RepeatClause(clauses, min.IntValue, min.IntValue);
    }

    
    [Production("clause : LPAREN  groupclauses RPAREN OPTION ")]
    public IClause GroupOptional(Token<EbnfTokenGeneric> discardLeft, GroupClause group,
        Token<EbnfTokenGeneric> discardRight, Token<EbnfTokenGeneric> option)
    {
        return new OptionClause(group);
    }


    [Production("groupclauses : groupclause groupclauses")]
    public GroupClause GroupClauses(GroupClause group, GroupClause groups)
    {
        if (groups != null) group.AddRange(groups);
        return group;
    }

    [Production("groupclauses : groupclause")]
    public GroupClause GroupClausesOne(GroupClause group)
    {
        return group;
    }

    [Production("groupclause : IDENTIFIER ")]
    public GroupClause GroupClause(Token<EbnfTokenGeneric> id)
    {
        var clause = BuildTerminalOrNonTerimal(id.Value);
        return new GroupClause(clause);
    }

    [Production("groupclause : STRING ")]
    public GroupClause GroupClauseExplicit(Token<EbnfTokenGeneric> explicitToken)
    {
        var clause = BuildTerminalOrNonTerimal(explicitToken.Value,discard:false);
        return new GroupClause(clause);
    }

    
    

    [Production("groupclause : IDENTIFIER DISCARD ")]
    public GroupClause GroupClauseDiscarded(Token<EbnfTokenGeneric> id, Token<EbnfTokenGeneric> discarded)
    {
        var clause = BuildTerminalOrNonTerimal(id.Value, true);
        return new GroupClause(clause);
    }
    
    [Production("groupclause : STRING DISCARD ")]
    public GroupClause GroupClauseExplicitDiscarded(Token<EbnfTokenGeneric> explicitToken, Token<EbnfTokenGeneric> discarded)
    {
        var clause = BuildTerminalOrNonTerimal(explicitToken.Value, true);
        return new GroupClause(clause);
    }

    [Production("groupclause : choiceclause ")]
    public GroupClause GroupChoiceClause(ChoiceClause choices)
    {
        return new GroupClause(choices);
    }



    #endregion


    private IClause BuildTerminalOrNonTerimal(string name, bool discard = false)
    {
        
        string token = null;
        IClause clause;
        var isTerminal = false;
        // TODO : use available token list => need a context . or a instance variable even it is not as clean, it does not mind as it's a build step

        
        
        if (isTerminal)
            clause = new TerminalClause(token, discard);
        else
        {
            if (name.StartsWith("'"))
            {
                clause = new TerminalClause(name.Substring(1,name.Length-2), discard);
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