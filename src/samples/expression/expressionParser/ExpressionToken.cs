using csly.models;

namespace sourceGenerationTester.expressionParser
{
    public enum ExpressionToken
    {
        // float number 
        //[Double] DOUBLE = 1,

        // integer        
        [Int] INT = 3,

        [AlphaId] IDENTIFIER = 4,

        [Sugar("^^")] EXP = 12,

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

        [Keyword("TEN")] TEN = 15,

        [Keyword("FORTYTWO")] FORTYTWO = 17,

    }
}
