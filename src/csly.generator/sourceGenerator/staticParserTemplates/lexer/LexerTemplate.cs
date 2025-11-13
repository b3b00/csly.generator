//using csly.generator.model.lexer;
//using System;
//using System.Collections.Generic;
//using sourceGenerationTester.expressionParser;

//namespace sourceGenerationTester.staticlexer;



using System.Net.Mime;

public enum LexerStates
{
    Start,
    <#STATES#>
}

public class Static<#LEXER#>
{


    public Static<#LEXER#>()
    {
    }

    public LexerResult<<#LEXER#>> Scan(ReadOnlySpan<char> source)
    {
        List<Token <<#LEXER#>>> tokens = new List<Token<<#LEXER#>>>();
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
                <#START_RULES#>
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
            <#OTHER_STATES#>
        }
    tokens.Add(new Token<<#LEXER#>>() { IsEOS = true, Position = position });
    return tokens;
    }
}