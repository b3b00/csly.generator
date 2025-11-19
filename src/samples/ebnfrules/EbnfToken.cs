using csly.models;

namespace ebnf
{   
    
    public enum EbnfTokenGeneric
    {
        [AlphaNumDashId] 
        IDENTIFIER = 1,
        
        [Sugar(":")]
        COLON = 2,
        
        [Sugar("*")]
        ZEROORMORE = 3,
        
        [Sugar("+")]
        ONEORMORE = 4,
        
        [Sugar("?")]
        OPTION = 6,
        
        [Sugar("[d]")]
        DISCARD = 7,
        
        [Sugar("(")]
        LPAREN = 8,
        
        [Sugar(")")]
        RPAREN = 9,
        
        [Sugar("|")]
        OR = 10,
        
        [Sugar("[")]
        LCROG = 11,
        
        [Sugar("]")]
        RCROG = 12,

        [String("'","\\")]
        STRING = 13,
        
        [Sugar( "-")]
        DASH = 14,
        
        [Sugar( "{")]  
        LCURLY = 15,
        
        [Sugar( "}")]  
        RCURLY = 16,

        [Int]
        INT = 17
    }
}