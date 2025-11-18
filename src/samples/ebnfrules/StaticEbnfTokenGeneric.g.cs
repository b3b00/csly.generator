
using csly.models;
using System;
using ebnf;
using ebnf.grammar;
using System.Collections.Generic;


using System.Collections.Generic;

namespace ebnf.grammar;


public enum LexerStates
{
    Start,
    InInt
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
        List<Token<EbnfTokenGeneric>> tokens = new List<Token<EbnfTokenGeneric>>();
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
