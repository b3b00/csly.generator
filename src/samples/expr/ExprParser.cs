using csly.models;

namespace expr;

[ParserRoot("root")]
public class ExprParser
{
    [Production("root : ExprParser_expressions")]
    public string Root(string value)
    {
        return value;
    }

    [Operation("PLUS", Affix.InFix, Associativity.Left, 10)]
    public string Plus(string left, Token<ExprToken> op, string right)
    {
        return $"( {left} + {right} )";
    }

    [Operation("MINUS", Affix.InFix, Associativity.Left, 10)]
    public string Minus(string left, Token<ExprToken> op, string right)
    {
        return $"( {left} + {right} )";
    }

    [Right("TIMES", 50)]
    public string Times(string left, Token<ExprToken> op, string right)
    {
        return $"( {left} * {right} )";
    }

    [Right("DIV", 50)]
    public string Divide(string left, Token<ExprToken> op, string right)
    {
        return $"( {left} / {right} )";
    }
    /*
    [Prefix("MINUS",Associativity.Left,100)]
    public string UMinus(Token<ExprToken> op, string right)
    {
        return $"( - {right} )";
    }
    */

    [Operand]
    [Production("intOperand : INT")]
    public string intOperand(Token<ExprToken> intToken)
    {
        return intToken.Value;
    }

}

