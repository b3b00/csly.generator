
using ebnf.grammar;
using System;
using csly.models;
using System.Collections.Generic;


   using System.Collections.Generic;

namespace ebnf.grammar;


public enum LexerStates
{
    Start,
    InIdentifier, InSugar_COLON_1, InSugar_ZEROORMORE_1, InSugar_ONEORMORE_1, InSugar_OPTION_1, InSugar_DISCARD_1, InSugar_LPAREN_1, InSugar_RPAREN_1, InSugar_OR_1, InSugar_LCROG_1, InSugar_RCROG_1, InSugar_DASH_1, InSugar_LCURLY_1, InSugar_RCURLY_1, InInt, InSugar_DISCARD_2, InSugar_DISCARD_3
}

public class StaticEbnfTokenGeneric
{


    private readonly Dictionary<string, EbnfTokenGeneric> _keyWords = new Dictionary<string, EbnfTokenGeneric>()  
    {
        
    };

    public StaticEbnfTokenGeneric()
    {
    }

    public LexerResult<EbnfTokenGeneric> Scan(ReadOnlySpan<char> source)
    {
        List<Token <EbnfTokenGeneric>> tokens = new List<Token<EbnfTokenGeneric>>();
        LexerPosition position = new LexerPosition(0, 0, 0);
        LexerPosition previous = new LexerPosition(0, 0, 0);
        int length = source.Length;
        LexerStates state = LexerStates.Start;

        while (position.Index <= length)
        {
            char currentChar = (char)0;
            if (position.Index < length)
            {
                currentChar = source[position.Index];
            }
            if (state == LexerStates.Start)
            {
                
if ((currentChar == '_') || (currentChar >= 'a' && currentChar <= 'z') || (currentChar >= 'A' && currentChar <= 'Z'))
{
    state = LexerStates.InIdentifier;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == ':')
{
    state = LexerStates.InSugar_COLON_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '*')
{
    state = LexerStates.InSugar_ZEROORMORE_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '+')
{
    state = LexerStates.InSugar_ONEORMORE_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '?')
{
    state = LexerStates.InSugar_OPTION_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '[')
{
    state = LexerStates.InSugar_DISCARD_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '(')
{
    state = LexerStates.InSugar_LPAREN_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == ')')
{
    state = LexerStates.InSugar_RPAREN_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '|')
{
    state = LexerStates.InSugar_OR_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '[')
{
    state = LexerStates.InSugar_LCROG_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == ']')
{
    state = LexerStates.InSugar_RCROG_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '-')
{
    state = LexerStates.InSugar_DASH_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '{')
{
    state = LexerStates.InSugar_LCURLY_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (currentChar == '}')
{
    state = LexerStates.InSugar_RCURLY_1;
    previous = position.Clone();    
    position.Index++;
}else 
if (char.IsDigit(currentChar))
{
    state = LexerStates.InInt;
    previous = position.Clone();    
    position.Index++;
}
                else if (currentChar == (char)0)
                {
                    position.Index++;
                }
            else if (char.IsWhiteSpace(currentChar))
            {
                position.Index++;
            }
                else
            {
                return $"Unexpected character '{currentChar}' at position {position.Index}";
            }
            }
            else if (state == LexerStates.InIdentifier){
    if ((currentChar == '-') || (currentChar == '_') || (currentChar >= 'a' && currentChar <= 'z') || (currentChar >= 'A' && currentChar <= 'Z') || (currentChar >= '0' && currentChar <= '9'))
    {
        position.Index++;
        state = LexerStates.InIdentifier;
    }
    else
    {
        var value = source.Slice(previous.Index, position.Index - previous.Index).ToString();
        // TODO : Check for keywords here
        if (_keyWords.TryGetValue(value, out var keyword)) {
            tokens.Add(new Token<EbnfTokenGeneric>(keyword, value, previous.Clone()));
        }
        else
        {
            tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.IDENTIFIER, value, previous.Clone()));
        if (currentChar == (char)0)
            {
                position.Index++;
            }
        }
        state = LexerStates.Start;
    }
}

else if (state == LexerStates.InSugar_COLON_1) // Sugar : "COLON"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.COLON, ":", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_ZEROORMORE_1) // Sugar : "ZEROORMORE"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.ZEROORMORE, "*", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_ONEORMORE_1) // Sugar : "ONEORMORE"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.ONEORMORE, "+", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_OPTION_1) // Sugar : "OPTION"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.OPTION, "?", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}
else 
if (state == LexerStates.InSugar_DISCARD_1 && currentChar == 'd')
{
    state = LexerStates.InSugar_DISCARD_2;
    previous = position.Clone();    
    position.Index++;
}
else 
if (state == LexerStates.InSugar_DISCARD_2 && currentChar == ']')
{
    state = LexerStates.InSugar_DISCARD_3;
    previous = position.Clone();    
    position.Index++;
}

else if (state == LexerStates.InSugar_DISCARD_3) // Sugar : "DISCARD"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.DISCARD, "[d]", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_LPAREN_1) // Sugar : "LPAREN"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.LPAREN, "(", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_RPAREN_1) // Sugar : "RPAREN"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.RPAREN, ")", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_OR_1) // Sugar : "OR"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.OR, "|", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_LCROG_1) // Sugar : "LCROG"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.LCROG, "[", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_RCROG_1) // Sugar : "RCROG"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.RCROG, "]", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_DASH_1) // Sugar : "DASH"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.DASH, "-", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_LCURLY_1) // Sugar : "LCURLY"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.LCURLY, "{", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}

else if (state == LexerStates.InSugar_RCURLY_1) // Sugar : "RCURLY"
{
    tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.RCURLY, "}", position));
    //position.Index++;
    previous = position.Clone();
    state = LexerStates.Start;
}
else if (state == LexerStates.InInt)
{
    if (char.IsDigit(currentChar))
    {
        position.Index++;
        state = LexerStates.InInt;
    }
    else
    {
        var intValue = source.Slice(previous.Index, position.Index - previous.Index).ToString();
        tokens.Add(new Token<EbnfTokenGeneric>(EbnfTokenGeneric.INT, intValue, previous.Clone()));
        if (currentChar == (char)0)
        {
            position.Index++;
        }
        state = LexerStates.Start;
    }
}
        }
    tokens.Add(new Token<EbnfTokenGeneric>() { IsEOS = true, Position = position });
    return tokens;
    }
}
