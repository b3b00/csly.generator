using System;
using System.Collections.Generic;
using System.Linq;
using csly.ebnf.models;

namespace ebnf.grammar
{

    public enum Visitors
    {
        rule_0,
        clauses_0,
        clauses_1,
        clause_0,
        clause_1,
        clause_2,
        clause_3,
        clause_4,
        clause_5,
        clause_6,
        clause_7,
        clause_8,
        clause_9,
        clause_10,
        clause_11,
        clause_12,
        clause_13,
        clause_14,
        clause_15,
        clause_16,
        clause_17,
        clause_18,
        clause_19,
        clause_20,
        clause_21,
        choiceclause_0,
        choices_0,
        choices_1,
        choices_2,
        choices_3,
        groupclauses_0,
        groupclauses_1,
        groupclause_0,
        groupclause_1,
        groupclause_2,
        groupclause_3,
        groupclause_4,

    }

    public partial class StaticRuleParser : AbstractParserGenerator<EbnfTokenGeneric, RuleParser, GrammarNode>
    {
        public Dictionary<EbnfTokenGeneric, Dictionary<string, string>> LexemeLabels { get; set; }

        public string I18n { get; set; }


        #region helpers

        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> parseTerminal(List<Token<EbnfTokenGeneric>> tokens, EbnfTokenGeneric expected, int position,
                bool discarded = false)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            result.IsError = expected != tokens[position].TokenID;
            result.EndingPosition = !result.IsError ? position + 1 : position;

            if (result.IsError)
            {
                result.AddError(new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(token, LexemeLabels, I18n, new LeadingToken<EbnfTokenGeneric>(expected)));
            }
            else
            {

                token.Discarded = discarded;
                token.IsExplicit = false;
                result.Root = new SyntaxLeaf<EbnfTokenGeneric, GrammarNode>(token, discarded);
                result.HasByPassNodes = false;
            }

            return result;
        }

        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> parseExplicitTerminal(List<Token<EbnfTokenGeneric>> tokens, string expected, int position,
                bool discarded = false)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();

            result.EndingPosition = !result.IsError ? position + 1 : position;

            var leading = new LeadingToken<EbnfTokenGeneric>(default(EbnfTokenGeneric), expected);

            result.IsError = !leading.Match(tokens[position]);
            var token = tokens[position];

            if (result.IsOk)
            {
                token.Discarded = discarded;
                token.IsExplicit = false;
                result.Root = new SyntaxLeaf<EbnfTokenGeneric, GrammarNode>(token, discarded);
                result.HasByPassNodes = false;
            }
            else
            {
                result.AddError(new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(token, LexemeLabels, I18n, leading));
            }

            return result;
        }

        #endregion



        ///////////////////////////////////////
        // RULE rule : IDENTIFIER COLON clauses
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_rule_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal COLON
            var r1 = ParseTerminal_COLON(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;




            // parse non terminal clauses
            var r2 = ParseNonTerminal_clauses(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("rule", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root },
                "rule_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r2.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clauses : clause clauses
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clauses_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal clause
            var r0 = ParseNonTerminal_clause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal clauses
            var r1 = ParseNonTerminal_clauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clauses", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clauses_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clauses : clause
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clauses_1(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal clause
            var r0 = ParseNonTerminal_clause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clauses", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "clauses_1");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER ZEROORMORE
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal ZEROORMORE
            var r1 = ParseTerminal_ZEROORMORE(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER ONEORMORE
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_1(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal ONEORMORE
            var r1 = ParseTerminal_ONEORMORE(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_1");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER LCURLY INT DASH INT RCURLY
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_2(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal LCURLY
            var r1 = ParseTerminal_LCURLY(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal INT
            var r2 = ParseTerminal_INT(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal DASH
            var r3 = ParseTerminal_DASH(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            // parse terminal INT
            var r4 = ParseTerminal_INT(tokens, position);
            if (r4.IsError)
            {
                return r4;
            }
            position = r4.EndingPosition;





            // parse terminal RCURLY
            var r5 = ParseTerminal_RCURLY(tokens, position);
            if (r5.IsError)
            {
                return r5;
            }
            position = r5.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root, r4.Root, r5.Root },
                "clause_2");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r5.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER LCURLY INT RCURLY
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_3(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal LCURLY
            var r1 = ParseTerminal_LCURLY(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal INT
            var r2 = ParseTerminal_INT(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal RCURLY
            var r3 = ParseTerminal_RCURLY(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root },
                "clause_3");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r3.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER OPTION
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_4(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal OPTION
            var r1 = ParseTerminal_OPTION(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_4");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER DISCARD
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_5(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal DISCARD
            var r1 = ParseTerminal_DISCARD(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_5");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause DISCARD
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_6(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal DISCARD
            var r1 = ParseTerminal_DISCARD(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_6");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_7(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "clause_7");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : IDENTIFIER
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_8(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "clause_8");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : STRING
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_9(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal STRING
            var r0 = ParseTerminal_STRING(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "clause_9");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : STRING DISCARD
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_10(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal STRING
            var r0 = ParseTerminal_STRING(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal DISCARD
            var r1 = ParseTerminal_DISCARD(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_10");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : LPAREN groupclauses RPAREN
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_11(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LPAREN
            var r0 = ParseTerminal_LPAREN(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RPAREN
            var r2 = ParseTerminal_RPAREN(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root },
                "clause_11");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r2.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause ONEORMORE
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_12(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal ONEORMORE
            var r1 = ParseTerminal_ONEORMORE(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_12");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause ZEROORMORE
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_13(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal ZEROORMORE
            var r1 = ParseTerminal_ZEROORMORE(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_13");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause LCURLY INT DASH INT RCURLY
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_14(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal LCURLY
            var r1 = ParseTerminal_LCURLY(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal INT
            var r2 = ParseTerminal_INT(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal DASH
            var r3 = ParseTerminal_DASH(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            // parse terminal INT
            var r4 = ParseTerminal_INT(tokens, position);
            if (r4.IsError)
            {
                return r4;
            }
            position = r4.EndingPosition;





            // parse terminal RCURLY
            var r5 = ParseTerminal_RCURLY(tokens, position);
            if (r5.IsError)
            {
                return r5;
            }
            position = r5.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root, r4.Root, r5.Root },
                "clause_14");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r5.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause LCURLY INT RCURLY
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_15(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal LCURLY
            var r1 = ParseTerminal_LCURLY(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal INT
            var r2 = ParseTerminal_INT(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal RCURLY
            var r3 = ParseTerminal_RCURLY(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root },
                "clause_15");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r3.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : choiceclause OPTION
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_16(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal OPTION
            var r1 = ParseTerminal_OPTION(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "clause_16");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : LPAREN groupclauses RPAREN ONEORMORE
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_17(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LPAREN
            var r0 = ParseTerminal_LPAREN(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RPAREN
            var r2 = ParseTerminal_RPAREN(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal ONEORMORE
            var r3 = ParseTerminal_ONEORMORE(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root },
                "clause_17");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r3.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : LPAREN groupclauses RPAREN ZEROORMORE
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_18(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LPAREN
            var r0 = ParseTerminal_LPAREN(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RPAREN
            var r2 = ParseTerminal_RPAREN(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal ZEROORMORE
            var r3 = ParseTerminal_ZEROORMORE(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root },
                "clause_18");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r3.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : LPAREN groupclauses RPAREN LCURLY INT DASH INT RCURLY
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_19(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LPAREN
            var r0 = ParseTerminal_LPAREN(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RPAREN
            var r2 = ParseTerminal_RPAREN(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal LCURLY
            var r3 = ParseTerminal_LCURLY(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            // parse terminal INT
            var r4 = ParseTerminal_INT(tokens, position);
            if (r4.IsError)
            {
                return r4;
            }
            position = r4.EndingPosition;





            // parse terminal DASH
            var r5 = ParseTerminal_DASH(tokens, position);
            if (r5.IsError)
            {
                return r5;
            }
            position = r5.EndingPosition;





            // parse terminal INT
            var r6 = ParseTerminal_INT(tokens, position);
            if (r6.IsError)
            {
                return r6;
            }
            position = r6.EndingPosition;





            // parse terminal RCURLY
            var r7 = ParseTerminal_RCURLY(tokens, position);
            if (r7.IsError)
            {
                return r7;
            }
            position = r7.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root, r4.Root, r5.Root, r6.Root, r7.Root },
                "clause_19");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r7.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : LPAREN groupclauses RPAREN LCURLY INT RCURLY
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_20(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LPAREN
            var r0 = ParseTerminal_LPAREN(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RPAREN
            var r2 = ParseTerminal_RPAREN(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal LCURLY
            var r3 = ParseTerminal_LCURLY(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            // parse terminal INT
            var r4 = ParseTerminal_INT(tokens, position);
            if (r4.IsError)
            {
                return r4;
            }
            position = r4.EndingPosition;





            // parse terminal RCURLY
            var r5 = ParseTerminal_RCURLY(tokens, position);
            if (r5.IsError)
            {
                return r5;
            }
            position = r5.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root, r4.Root, r5.Root },
                "clause_20");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r5.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE clause : LPAREN groupclauses RPAREN OPTION
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_clause_21(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LPAREN
            var r0 = ParseTerminal_LPAREN(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RPAREN
            var r2 = ParseTerminal_RPAREN(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            // parse terminal OPTION
            var r3 = ParseTerminal_OPTION(tokens, position);
            if (r3.IsError)
            {
                return r3;
            }
            position = r3.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("clause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root, r3.Root },
                "clause_21");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r3.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE choiceclause : LCROG choices RCROG
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_choiceclause_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal LCROG
            var r0 = ParseTerminal_LCROG(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal choices
            var r1 = ParseNonTerminal_choices(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            // parse terminal RCROG
            var r2 = ParseTerminal_RCROG(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("choiceclause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root },
                "choiceclause_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r2.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE choices : IDENTIFIER
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_choices_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("choices", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "choices_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE choices : STRING
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_choices_1(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal STRING
            var r0 = ParseTerminal_STRING(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("choices", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "choices_1");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE choices : IDENTIFIER OR choices
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_choices_2(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal OR
            var r1 = ParseTerminal_OR(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;




            // parse non terminal choices
            var r2 = ParseNonTerminal_choices(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("choices", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root },
                "choices_2");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r2.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE choices : STRING OR choices
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_choices_3(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal STRING
            var r0 = ParseTerminal_STRING(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal OR
            var r1 = ParseTerminal_OR(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;




            // parse non terminal choices
            var r2 = ParseNonTerminal_choices(tokens, position);
            if (r2.IsError)
            {
                return r2;
            }
            position = r2.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("choices", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root, r2.Root },
                "choices_3");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r2.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclauses : groupclause groupclauses
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclauses_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal groupclause
            var r0 = ParseNonTerminal_groupclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;




            // parse non terminal groupclauses
            var r1 = ParseNonTerminal_groupclauses(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclauses", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "groupclauses_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclauses : groupclause
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclauses_1(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal groupclause
            var r0 = ParseNonTerminal_groupclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclauses", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "groupclauses_1");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclause : IDENTIFIER
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclause_0(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "groupclause_0");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclause : STRING
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclause_1(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal STRING
            var r0 = ParseTerminal_STRING(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "groupclause_1");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclause : IDENTIFIER DISCARD
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclause_2(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal IDENTIFIER
            var r0 = ParseTerminal_IDENTIFIER(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal DISCARD
            var r1 = ParseTerminal_DISCARD(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "groupclause_2");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclause : STRING DISCARD
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclause_3(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();



            // parse terminal STRING
            var r0 = ParseTerminal_STRING(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            // parse terminal DISCARD
            var r1 = ParseTerminal_DISCARD(tokens, position);
            if (r1.IsError)
            {
                return r1;
            }
            position = r1.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root, r1.Root },
                "groupclause_3");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r1.EndingPosition;
            return result;
        }

        ///////////////////////////////////////
        // RULE groupclause : choiceclause
        ///////////////////////////////////////
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseRule_groupclause_4(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();


            // parse non terminal choiceclause
            var r0 = ParseNonTerminal_choiceclause(tokens, position);
            if (r0.IsError)
            {
                return r0;
            }
            position = r0.EndingPosition;





            var tree = new SyntaxNode<EbnfTokenGeneric, GrammarNode>("groupclause", new List<ISyntaxNode<EbnfTokenGeneric, GrammarNode>>() { r0.Root },
                "groupclause_4");
            result.Root = tree;
            result.IsError = false;
            result.EndingPosition = r0.EndingPosition;
            return result;
        }
        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_IDENTIFIER(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.IDENTIFIER, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_COLON(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.COLON, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_ZEROORMORE(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.ZEROORMORE, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_ONEORMORE(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.ONEORMORE, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_LCURLY(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.LCURLY, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_INT(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.INT, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_DASH(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.DASH, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_RCURLY(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.RCURLY, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_OPTION(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.OPTION, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_DISCARD(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.DISCARD, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_STRING(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.STRING, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_LPAREN(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.LPAREN, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_RPAREN(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.RPAREN, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_LCROG(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.LCROG, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_RCROG(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.RCROG, position, discarded);


        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseTerminal_OR(List<Token<EbnfTokenGeneric>> tokens, int position, bool discarded = false)
                => parseTerminal(tokens, EbnfTokenGeneric.OR, position, discarded);



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_clauses(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_clauses_0(tokens, position);
                results.Add(r0);
            }
            var r1Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r1Leadings.Any(x => x.Match(token)))
            {
                var r1 = ParseRule_clauses_1(tokens, position);
                results.Add(r1);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_clause(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_clause_0(tokens, position);
                results.Add(r0);
            }
            var r1Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r1Leadings.Any(x => x.Match(token)))
            {
                var r1 = ParseRule_clause_1(tokens, position);
                results.Add(r1);
            }
            var r2Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r2Leadings.Any(x => x.Match(token)))
            {
                var r2 = ParseRule_clause_2(tokens, position);
                results.Add(r2);
            }
            var r3Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r3Leadings.Any(x => x.Match(token)))
            {
                var r3 = ParseRule_clause_3(tokens, position);
                results.Add(r3);
            }
            var r4Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r4Leadings.Any(x => x.Match(token)))
            {
                var r4 = ParseRule_clause_4(tokens, position);
                results.Add(r4);
            }
            var r5Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r5Leadings.Any(x => x.Match(token)))
            {
                var r5 = ParseRule_clause_5(tokens, position);
                results.Add(r5);
            }
            var r6Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r6Leadings.Any(x => x.Match(token)))
            {
                var r6 = ParseRule_clause_6(tokens, position);
                results.Add(r6);
            }
            var r7Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r7Leadings.Any(x => x.Match(token)))
            {
                var r7 = ParseRule_clause_7(tokens, position);
                results.Add(r7);
            }
            var r8Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r8Leadings.Any(x => x.Match(token)))
            {
                var r8 = ParseRule_clause_8(tokens, position);
                results.Add(r8);
            }
            var r9Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING)
                    };
            if (r9Leadings.Any(x => x.Match(token)))
            {
                var r9 = ParseRule_clause_9(tokens, position);
                results.Add(r9);
            }
            var r10Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING)
                    };
            if (r10Leadings.Any(x => x.Match(token)))
            {
                var r10 = ParseRule_clause_10(tokens, position);
                results.Add(r10);
            }
            var r11Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r11Leadings.Any(x => x.Match(token)))
            {
                var r11 = ParseRule_clause_11(tokens, position);
                results.Add(r11);
            }
            var r12Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r12Leadings.Any(x => x.Match(token)))
            {
                var r12 = ParseRule_clause_12(tokens, position);
                results.Add(r12);
            }
            var r13Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r13Leadings.Any(x => x.Match(token)))
            {
                var r13 = ParseRule_clause_13(tokens, position);
                results.Add(r13);
            }
            var r14Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r14Leadings.Any(x => x.Match(token)))
            {
                var r14 = ParseRule_clause_14(tokens, position);
                results.Add(r14);
            }
            var r15Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r15Leadings.Any(x => x.Match(token)))
            {
                var r15 = ParseRule_clause_15(tokens, position);
                results.Add(r15);
            }
            var r16Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r16Leadings.Any(x => x.Match(token)))
            {
                var r16 = ParseRule_clause_16(tokens, position);
                results.Add(r16);
            }
            var r17Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r17Leadings.Any(x => x.Match(token)))
            {
                var r17 = ParseRule_clause_17(tokens, position);
                results.Add(r17);
            }
            var r18Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r18Leadings.Any(x => x.Match(token)))
            {
                var r18 = ParseRule_clause_18(tokens, position);
                results.Add(r18);
            }
            var r19Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r19Leadings.Any(x => x.Match(token)))
            {
                var r19 = ParseRule_clause_19(tokens, position);
                results.Add(r19);
            }
            var r20Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r20Leadings.Any(x => x.Match(token)))
            {
                var r20 = ParseRule_clause_20(tokens, position);
                results.Add(r20);
            }
            var r21Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN)
                    };
            if (r21Leadings.Any(x => x.Match(token)))
            {
                var r21 = ParseRule_clause_21(tokens, position);
                results.Add(r21);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_choiceclause(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_choiceclause_0(tokens, position);
                results.Add(r0);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_groupclauses(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_groupclauses_0(tokens, position);
                results.Add(r0);
            }
            var r1Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r1Leadings.Any(x => x.Match(token)))
            {
                var r1 = ParseRule_groupclauses_1(tokens, position);
                results.Add(r1);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_choices(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_choices_0(tokens, position);
                results.Add(r0);
            }
            var r1Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING)
                    };
            if (r1Leadings.Any(x => x.Match(token)))
            {
                var r1 = ParseRule_choices_1(tokens, position);
                results.Add(r1);
            }
            var r2Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r2Leadings.Any(x => x.Match(token)))
            {
                var r2 = ParseRule_choices_2(tokens, position);
                results.Add(r2);
            }
            var r3Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING)
                    };
            if (r3Leadings.Any(x => x.Match(token)))
            {
                var r3 = ParseRule_choices_3(tokens, position);
                results.Add(r3);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_groupclause(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING), new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_groupclause_0(tokens, position);
                results.Add(r0);
            }
            var r1Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING)
                    };
            if (r1Leadings.Any(x => x.Match(token)))
            {
                var r1 = ParseRule_groupclause_1(tokens, position);
                results.Add(r1);
            }
            var r2Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
                    };
            if (r2Leadings.Any(x => x.Match(token)))
            {
                var r2 = ParseRule_groupclause_2(tokens, position);
                results.Add(r2);
            }
            var r3Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.STRING)
                    };
            if (r3Leadings.Any(x => x.Match(token)))
            {
                var r3 = ParseRule_groupclause_3(tokens, position);
                results.Add(r3);
            }
            var r4Leadings = new LeadingToken<EbnfTokenGeneric>[]
                    {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG)
                    };
            if (r4Leadings.Any(x => x.Match(token)))
            {
                var r4 = ParseRule_groupclause_4(tokens, position);
                results.Add(r4);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



        public SyntaxParseResult<EbnfTokenGeneric, GrammarNode> ParseNonTerminal_rule(List<Token<EbnfTokenGeneric>> tokens, int position)
        {
            var result = new SyntaxParseResult<EbnfTokenGeneric, GrammarNode>();
            var token = tokens[position];
            var results = new List<SyntaxParseResult<EbnfTokenGeneric, GrammarNode>>();

            var expectedTokens = new List<LeadingToken<EbnfTokenGeneric>>() { new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER) };

            var r0Leadings = new LeadingToken<EbnfTokenGeneric>[]
            {
            new LeadingToken<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER)
            };
            if (r0Leadings.Any(x => x.Match(token)))
            {
                var r0 = ParseRule_rule_0(tokens, position);
                results.Add(r0);
            }


            var okResult = results.OrderByDescending(r => r.EndingPosition).FirstOrDefault(r => r.IsOk);
            if (okResult != null && okResult.IsOk)
            {
                return okResult;
            }

            result.IsError = true;
            var allExpected = new List<UnexpectedTokenSyntaxError<EbnfTokenGeneric>>() { new UnexpectedTokenSyntaxError<EbnfTokenGeneric>(tokens[position], "en", expectedTokens) };
            result.AddErrors(results.SelectMany(x => x.Errors != null ? x.GetErrors() : allExpected).ToList());
            return result;
        }



    }
}