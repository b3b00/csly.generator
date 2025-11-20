
using csly.models;
using System;

namespace sourceGenerationTester.expressionParser
{

    [ParserRoot("expression")]
    public class ExpressionParser
    {
        [Production("primary: INT")]
        public int Primaryint(Token<ExpressionToken> dbl) => dbl.IntValue;

        [Production("primary: TEN")]
        public int Primary10(Token<ExpressionToken> pi)
        {
            return 10;
        }

        [Production("primary: FORTYTWO")]
        public int Primary42(Token<ExpressionToken> e)
        {
            return 42;
        }

        [NodeName("group")]
        [Production("primary: LPAREN  expression RPAREN ")]
        public int Group(Token<ExpressionToken> l, int groupValue, Token<ExpressionToken> r)
        {
            return groupValue;
        }


        [NodeName("addOrSubstract")]
        [Production("expression : term PLUS expression")]
        [Production("expression : term MINUS expression")]
        public int Expression(int left, Token<ExpressionToken> operatorToken, int right)
        {
            int result = 0;


            switch (operatorToken.TokenID)
            {
                case ExpressionToken.PLUS:
                    {
                        result = left + right;
                        break;
                    }
                case ExpressionToken.MINUS:
                    {
                        result = left - right;
                        break;
                    }
            }

            return result;
        }


        [NodeName("expression")]
        [Production("expression : term")]
        public int Expression_Term(int termValue)
        {
            return termValue;
        }

        [NodeName("multOrDivide")]
        [Production("term : factor TIMES term")]
        [Production("term : factor DIVIDE term")]
        [Production("term : factor EXP term")]
        public int Term(int left, Token<ExpressionToken> operatorToken, int right)
        {
            int result = 0;


            switch (operatorToken.TokenID)
            {
                case ExpressionToken.TIMES:
                    {
                        result = left * right;
                        break;
                    }
                case ExpressionToken.DIVIDE:
                    {
                        result = left / right;
                        break;
                    }
                    case ExpressionToken.EXP:
                    {
                        result = (int)Math.Pow(left, right);
                        break;
                    }
            }

            return result;
        }

        [Production("term : factor")]
        [NodeName("term")]
        public int Term_Factor(int factorValue)
        {
            return factorValue;
        }

        [Production("factor : primary")]
        [NodeName("primary")]
        public int primaryFactor(int primValue)
        {
            return primValue;
        }

        [NodeName("negate")]
        [Production("factor : MINUS factor")]
        public int MinusFactor(Token<ExpressionToken> discardedMinus, int factorValue)
        {
            return -factorValue;
        }
    }
}