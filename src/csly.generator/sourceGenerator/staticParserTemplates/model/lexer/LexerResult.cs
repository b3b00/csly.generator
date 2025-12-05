using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace <#NS#>;

[DebuggerDisplay("{TokenID} : {Value} - {IsExplicit}")]

public class LexerResult<T> where T:struct, Enum
{
    public List<Token<T>> Tokens { get; set; } = new List<Token<T>>();
    
    public LexicalError Error { get; set; } = null;

    private bool _isOk = false;


    public bool IsOk => _isOk;

    public bool IsError => !_isOk;

    public static implicit operator LexerResult<T>(LexicalError error)
    {
        var result = new LexerResult<T>()
        {
            Error = error,
            _isOk = false
        };
        return result;
    }

    public static implicit operator LexerResult<T>(List<Token<T>> tokens)
    {
        var result = new LexerResult<T>()
        {
            Tokens = tokens,
            _isOk = true
        };
        return result;
    }
}
