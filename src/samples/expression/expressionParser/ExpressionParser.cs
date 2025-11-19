
using csly.models;
using System;

namespace sourceGenerationTester.expressionParser
{

    [ParserRoot("expression")]
    public class ExpressionParser
    {
        [Production("primary: DOUBLE")]
        public double PrimaryDouble(Token<ExpressionToken> dbl) => dbl.DoubleValue;

        [Production("primary: PI")]
        public double PrimaryPi(Token<ExpressionToken> pi)
        {
            return 3.1415;
        }

        [Production("primary: E")]
        public double PrimaryE(Token<ExpressionToken> e)
        {
            return 2.7182;
        }

        [NodeName("group")]
        [Production("primary: LPAREN  expression RPAREN ")]
        public double Group(Token<ExpressionToken> l, double groupValue, Token<ExpressionToken> r)
        {
            return groupValue;
        }


        [NodeName("addOrSubstract")]
        [Production("expression : term PLUS expression")]
        [Production("expression : term MINUS expression")]
        public double Expression(double left, Token<ExpressionToken> operatorToken, double right)
        {
            double result = 0;


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
        public double Expression_Term(double termValue)
        {
            return termValue;
        }

        [NodeName("multOrDivide")]
        [Production("term : factor TIMES term")]
        [Production("term : factor DIVIDE term")]
        public double Term(double left, Token<ExpressionToken> operatorToken, double right)
        {
            double result = 0;


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
            }

            return result;
        }

        [Production("term : factor")]
        [NodeName("term")]
        public double Term_Factor(double factorValue)
        {
            return factorValue;
        }

        [Production("factor : primary")]
        [NodeName("primary")]
        public double primaryFactor(double primValue)
        {
            return primValue;
        }

        [NodeName("negate")]
        [Production("factor : MINUS factor")]
        public double MinusFactor(Token<ExpressionToken> discardedMinus, double factorValue)
        {
            return -factorValue;
        }
    }
}