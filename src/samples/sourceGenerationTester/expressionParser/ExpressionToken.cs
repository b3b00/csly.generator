using csly.ebnf.models;

namespace sourceGenerationTester.expressionParser
{
    public enum ExpressionToken
    {
        // float number 
        [Double] DOUBLE = 1,

        // integer        
        [Int] INT = 3,

        [AlphaId] IDENTIFIER = 4,

        // the + operator
        [Sugar("+")] PLUS = 5,

        // the - operator
        [Sugar("-")] MINUS = 6,

        // the * operator
        [Sugar("*")] TIMES = 7,

        //  the  / operator
        [Sugar("/")] DIVIDE = 8,

        // a left paranthesis (
        [Sugar("(")] LPAREN = 9,

        // a right paranthesis )
        [Sugar(")")] RPAREN = 10,

        [Sugar("!")] FACTORIAL = 13,

        [Keyword("hello")] PI = 15,

        [Keyword("world")] E = 17,

    }
}
