
using System.Collections.Generic;

namespace <#NAMESPACE#>;

public class <#LEXER#>_FsmLexer
{

    private int _currentState = 0;

    private FsmMatch<<#LEXER#>> _currentMatch = null;

    private LexerPosition _currentPosition { get; set; }

    private LexerPosition _startPosition { get; set; }

    public <#LEXER#>_FsmLexer()
    {
    }

    public LexerResult<<#LEXER#>> Scan(ReadOnlySpan<char> source) {
        _currentPosition = new LexerPosition(0,0,0);
        _startPosition = new LexerPosition(0,0,0);
        List<Token<Toky>> tokens = new List<Token<Toky>>();
        while (_currentPosition.Index < source.Length)
        {
            <#STATE_CALLS#>
        }
        return tokens;
    }


    <#STATES#>
}