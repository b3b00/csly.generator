using csly.models;

namespace expr;

public enum ExprToken
{
    [Sugar("+")] PLUS,
    [Sugar("-")] MINUS,
    [Sugar("*")] TIMES,
    [Sugar("/")] DIV,
    [Int] INT,
}

