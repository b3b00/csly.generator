using csly.models;

namespace expr;

public enum ExprToken
{
    [Sugar("+")] PLUS,
    [Sugar("-")] MINUS,
    [Sugar("*")] TIMES,
    [Sugar("/")] DIV,
    [Int] INT,
    [Sugar("(")] LPAREN,
    [Sugar(")")] RPAREN,
    [Sugar("!")] FACTORIAL,

    [Sugar(";")] SEMICOLON,
    [Sugar("=")] ASSIGN,
    [Sugar(",")] COMMA,

    [AlphaNumId] ID,
    [Keyword("PRINT")] PRINT,
    [Keyword("TEST")] TEST,
    [Keyword("TRUE")] T,
    [Keyword("FALSE")] F,
    [Keyword("HEAD")] HEAD,




}

