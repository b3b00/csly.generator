using sly.lexer;
using sly.parser.generator;

namespace expr;

[ParserRoot("root")]
public class ExprParser
{
    [Production("root : ExprParser_expressions")]
    public int Root(int value)
    {
        return value;
    }

    [Right("PLUS", 10)]
    public int Plus(int left, Token<ExprToken> op, int right)
    {
        return left+right;
    }

    [Left("MINUS", 10)]
    public int Minus(int left, Token<ExprToken> op, int right)
    {
        return left-right;
    }

    [Right("TIMES", 50)]
    public int Times(int left, Token<ExprToken> op, int right)
    {
        return left * right;
    }

    [Left("DIV", 50)]
    public int Divide(int left, Token<ExprToken> op, int right)
    {
        return left / right;
    }


    //[Prefix("MINUS", Associativity.Left, 100)]
    //public int UMinus(Token<ExprToken> op, int right)
    //{
    //    return -right;
    //}

    //[Postfix("FACTORIAL", Associativity.Left, 110)]
    //public int Factorial(int value, Token<ExprToken> op)
    //{
    //    int factorial = 1;
    //    for (int i = value; i > 1; i--)
    //    {
    //        factorial *= i;
    //    }
    //    return factorial;
    //}

    [Operand]
    [Production("intOperand : INT")]
    public int intOperand(Token<ExprToken> intToken)
    {
        return intToken.IntValue;
    }

    [Operand]
    [Production("parenthesizedOperand : LPAREN ExprParser_expressions RPAREN")]
    public int parenthesizedOperand(Token<ExprToken> lp, int expr, Token<ExprToken> rp)
    {
        return expr;
    }
}

