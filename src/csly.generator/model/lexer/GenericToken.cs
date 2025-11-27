namespace csly.generator.model.lexer
{
    public enum GenericToken
    {
        Default,
        Identifier,
        Int,
        Hexa,
        Double,
        Date,
        KeyWord,
        String,
        Char,
        SugarToken,

        Extension,

        Comment,
        UpTo
    }
}

public enum EbnfTokenGeneric
{
    IDENTIFIER = 1,

    COLON = 2,

    ZEROORMORE = 3,

    ONEORMORE = 4,

    OPTION = 6,

    DISCARD = 7,

    LPAREN = 8,

    RPAREN = 9,

    OR = 10,

    LCROG = 11,
        
    RCROG = 12,

    STRING = 13,

    DASH = 14,

    LCURLY = 15,
    
    RCURLY = 16,
    
    INT = 17
}