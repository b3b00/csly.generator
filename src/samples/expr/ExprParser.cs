using csly.models;

namespace expr;

[ParserRoot("root")]
public class ExprParser
{

    Dictionary<string, int> variables = new Dictionary<string, int>();

    [Production("root : 'GO'[d] ':'[d] header statement*")]
    public int root(int head, List<int> statements)
    {
        return statements.Sum();
    }

    [Production("statement : [test|print]")]
    public int statement_assign(int statement)
    {
        return statement;
    }

    [Production("header : HEAD[d] LPAREN[d] (ID ExprParser_expressions)? (COMMA[d] ID  ExprParser_expressions)* RPAREN[d]")]
    public int header(ValueOption<Group<ExprToken, int>> firstPair, List<Group<ExprToken, int>> additionalPairs)
    {
        firstPair.Match(
            (x) =>
            {
                var id = x.Token(0);
                var expr = x.Value(1);
                Console.WriteLine($"HEADER PARAM: {id.Value} with expression value {expr}");
                variables[id.Value] = expr;
                return expr;
            },
            () => 0
        );

        foreach (var pair in additionalPairs)
        {
            var id = pair.Token(0);
            var expr = pair.Value(1);
            Console.WriteLine($"HEADER PARAM: {id.Value} with expression value {expr}");
            variables[id.Value] = expr;
        }
        return 0;
    }



    [Production("test : TEST[d] LPAREN[d] ID COMMA[d] ExprParser_expressions COMMA[d] INT COMMA[d] [T|F]  RPAREN[d] SEMICOLON?")]
    public int test(Token<ExprToken> id, int expr, Token<ExprToken> expected, Token<ExprToken> neg, Token<ExprToken> semi)
    {
        Console.WriteLine("TEST : semi is here ? " + (semi.IsEmpty ? "NO" : "YES"));
        int expectedValue = expected.IntValue;
        bool isNegated = neg.TokenID == ExprToken.T;
        expectedValue = isNegated ? -expectedValue : expectedValue;

        if (expr != expectedValue)
        {
            Console.WriteLine($"\x1b[31mTEST FAILED for variable {id.Value}: got {expr} but was expecting {expectedValue}\x1b[0m");
        }
        else
        {
            Console.WriteLine($"\x1b[32mTEST SUCCEEDED for variable {id.Value}: got expected value {expectedValue}\x1b[0m");
        }

        variables[id.Value] = expr;

        return expr;
    }



    [Production("semi : SEMICOLON")]
    public int Semi(Token<ExprToken> s)
    {
        return -1;
    }


    [Production("print : PRINT ID semi ?")]
    public int print(Token<ExprToken> printTok, Token<ExprToken> id, ValueOption<int> semi)
    {
        Console.WriteLine("PRINT : semi is here ? " + (semi.IsNone ? "NO" : "YES"));
        if (variables.TryGetValue(id.Value, out int value))
        {
            Console.WriteLine($"PRINT: {id.Value} = {value}");
        }
        else
        {
            Console.WriteLine($"\x1b[31mERROR: variable {id.Value} is not defined.\x1b[0m");
        }
        return 0;
    }

    #region  Expressions


    [Operation("PLUS", Affix.InFix, Associativity.Left, 10)]
    public int Plus(int left, Token<ExprToken> op, int right)
    {
        return left + right;
    }

    [Operation("MINUS", Affix.InFix, Associativity.Left, 10)]
    public int Minus(int left, Token<ExprToken> op, int right)
    {
        return left - right;
    }

    [Right("TIMES", 50)]
    public int Times(int left, Token<ExprToken> op, int right)
    {
        return left * right;
    }

    [Right("DIV", 50)]
    public int Divide(int left, Token<ExprToken> op, int right)
    {
        return left / right;
    }

    [Prefix("MINUS", Associativity.Left, 100)]
    public int UMinus(Token<ExprToken> op, int right)
    {
        return -right;
    }

    [Postfix("FACTORIAL", Associativity.Left, 110)]
    public int Factorial(int value, Token<ExprToken> op)
    {
        int factorial = 1;
        for (int i = value; i > 1; i--)
        {
            factorial *= i;
        }
        return factorial;
    }


    [Operand]
    [Production("intOperand : INT")]
    public int intOperand(Token<ExprToken> intToken)
    {
        return intToken.IntValue;
    }

    [Operand]
    [Production("parenthesizedOperand : LPAREN[d] ExprParser_expressions RPAREN[d]")]
    public int parenthesizedOperand(int expr)
    {
        return expr;
    }

    #endregion

}