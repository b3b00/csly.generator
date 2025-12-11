namespace generatorTests;

using csly.testing.models;



public enum ExprToken
{
    [Int] INT = 1,

    [Sugar("+")] PLUS = 2,

    [Sugar("-")] MINUS = 3,

    [Sugar("*")] MULT = 4,

    [Sugar("/")] DIVIDE = 5
}

[ParserRoot("expression")]
public class ExprParser
{
    
    [Production("expression : ExprParser_expressions")]
    public int expression(int expr) => expr;
    

    [Right("PLUS",10)]
    [Right("MINUS",10)]
    public int Term(int left, Token<ExprToken> oper, int right)
    {
        return oper.TokenID switch
        {
            ExprToken.PLUS => left + right,
            ExprToken.MINUS => left - right,
            _ => throw new Exception("Unknown operator")
        };
    }

    [Right("MULT",20)]
    [Right("DIVIDE",20)]
    public int Factor(int left, Token<ExprToken> oper, int right) 
    {
        return oper.TokenID switch
        {
            ExprToken.MULT => left * right,
            ExprToken.DIVIDE => left / right,
            _ => throw new Exception("Unknown operator")
        };
    }

}

[ParserGenerator]
public partial class Expr : AbstractParserGenerator<ExprToken,ExprParser,int>
{
    
}
