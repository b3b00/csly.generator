using csly.generator.model.lexer;
using System;
using System.Collections.Generic;
using sourceGenerationTester.expressionParser;
    
namespace sourceGenerationTester.staticlexer
{


    public enum LexerStates
    {
        Start,
        Plus,
        Minus,
        Times,
        Divide,
        InInt,
        Integer,
    }

    public class StaticLexer
    {


        public StaticLexer()
        {
        }

        public LexerResult<ExpressionToken> Scan(ReadOnlySpan<char> source)
        {
            List<Token<ExpressionToken>> tokens = new List<Token<ExpressionToken>>();
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
                    if (currentChar == '+')
                    {
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.PLUS, "+", position));
                        position.Index++;
                        previous = position.Clone();
                    }
                    else if (currentChar == '-')
                    {
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.MINUS, "-", position));
                        position.Index++;
                        previous = position.Clone();
                    }
                    else if (currentChar == '*')
                    {
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.TIMES, "*", position));
                        position.Index++;
                        previous = position.Clone();
                    }
                    else if (currentChar == '/')
                    {
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.DIVIDE, "/", position));
                        position.Index++;
                        previous = position.Clone();
                    }
                    else if (currentChar == '(')
                    {
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.LPAREN, "(", position));
                        position.Index++;
                        previous = position.Clone();
                    }
                    else if (currentChar == ')')
                    {
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.RPAREN, ")", position));
                        position.Index++;
                        previous = position.Clone();
                    }
                    else if (char.IsDigit(currentChar))
                    {
                        state = LexerStates.InInt;
                        previous = position.Clone();
                        position.Index++;
                    }
                    else if (char.IsWhiteSpace(currentChar))
                    {
                        position.Index++;
                    }
                    else if (currentChar == (char)0)
                    {
                        position.Index++;
                    }
                    else
                    {
                        return $"Unexpected character '{currentChar}' at position {position.Index}" ;
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
                        tokens.Add(new Token<ExpressionToken>(ExpressionToken.INT, intValue, previous.Clone()));
                        if (currentChar == (char)0)
                        {
                            position.Index++;                            
                        }
                        state = LexerStates.Start;
                    }                    
                }                
            }
            tokens.Add(new Token<ExpressionToken>() { IsEOS = true, Position = position });
            return tokens;
        }
    }
}
